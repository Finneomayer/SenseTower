using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Avatar2;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

using StreamLOD = Oculus.Avatar2.OvrAvatarEntity.StreamLOD;
using System.Linq;
using Unity.Collections;
using System.IO;
using Assets.Mechanics.Compression;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using Assets.Scripts.Shared;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class MetaAvatarNetworkBehaviour : NetworkBehaviour
    {
        #region Internal Classes

        class LoopbackState
        {
            public List<PacketData> packetQueue = new List<PacketData>(64);
            public StreamLOD requestedLod = StreamLOD.Low;
            public float smoothedPlaybackDelay = 0f;
        };

        [System.Serializable]
        public class SimulatedLatencySettings
        {
            [Range(0.0f, 0.5f)]
            public float fakeLatencyMax = 0.25f; //250 ms max latency

            [Range(0.0f, 0.5f)]
            public float fakeLatencyMin = 0.02f; //20ms min latency

            [Range(0.0f, 1.0f)]
            public float latencyWeight = 0.25f; // How much the latest sample impacts the current latency

            [Range(0, 10)]
            public int maxSamples = 4; //How many samples in our window

            internal float averageWindow = 0f;
            internal float latencySum = 0f;
            internal List<float> latencyValues = new List<float>();

            public float NextValue()
            {
                averageWindow = latencySum / (float)latencyValues.Count;
                float randomLatency = UnityEngine.Random.Range(fakeLatencyMin, fakeLatencyMax);
                float fakeLatency = averageWindow * (1f - latencyWeight) + latencyWeight * randomLatency;

                if (latencyValues.Count >= maxSamples)
                {
                    latencySum -= latencyValues.First().Value;
                    latencyValues.RemoveFirst();
                }

                latencySum += fakeLatency;
                latencyValues.AddLast(fakeLatency);

                return fakeLatency;
            }
        };

        #endregion

        private const string logScope = "SampleRemoteLoopbackManager";
        private const float PLAYBACK_SMOOTH_FACTOR = 0.25f;
        private const int MAX_PACKETS_PER_FRAME = 1;

        private const int MaxUnreceivedByServerPoseDataCount = 10;
        private const int MaxUnperformedRequestCount = 10;

        private static readonly float[] StreamLodSnapshotIntervalSeconds = new float[OvrAvatarEntity.StreamLODCount] { 1f / 72, 2f / 72, 3f / 72, 4f / 72 };

        [SerializeField]
        private SampleAvatarEntity _remoteAvatar;
        [SerializeField]
        private SampleAvatarEntity _defaultAssetAvatarEntity;
        [SerializeField]
        private SimulatedLatencySettings _simulatedLatencySettings = new SimulatedLatencySettings();
        [SerializeField]
        private LayerMask _mirrorRenderingLayerMask;
        [SerializeField]
        private LayerMask _localBodyLayerMask;
        [SerializeField]
        private NetworkVariable<bool> _isVisibleToOthers;

        private SampleAvatarEntity _localAvatar;
        private List<OvrAvatarEntity> _loopbackAvatars;
        private BaseSerializableAvatarData _baseAvatarData;
        private SerializableAvatarData _poseAvatarData;

        private int _unreceivedByServerPoseDataCount;
        private IncreasingValue _sendTimeout;
        private float _currentSendDelay;

        private int _unreceivedRequestedPoseDataCount;
        private IncreasingValue _requestTimeout;
        private float _currentRequestDelay;

        private List<ulong> _sendedToClients = new();

        private Dictionary<OvrAvatarEntity, LoopbackState> _loopbackStates =
            new Dictionary<OvrAvatarEntity, LoopbackState>();

        private readonly List<PacketData> _packetPool = new List<PacketData>(32);
        private readonly List<PacketData> _deadList = new List<PacketData>(16);

        private readonly float[] _streamLodSnapshotElapsedTime = new float[OvrAvatarEntity.StreamLODCount];

        private byte[] _packetBuffer = new byte[16 * 1024];
        private GCHandle _pinnedBuffer;

        private IOculusAuthService _authService;
        private bool _isCdnAvatarAvailable;
        private OvrAvatarEntity _nextAvatarToSend;

        public bool IsInitialized { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _remoteAvatar.gameObject.SetActive(IsClient);
            if (!IsClient)
            {
                return;
            }

            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _authService = commonDIInstaller.Resolve<IOculusAuthService>();
            }

            if (IsOwner)
            {
                //_remoteAvatar.AvatarStubVisibilityChanged += OnRemoteAvatarStubVisibilityChanged;
                //_remoteAvatar.OnDefaultAvatarLoadedEvent.AddListener(OnRemoteOnDefaultAvatarLoadedEvent);
            }
            else
            {
                _remoteAvatar.OnDefaultAvatarLoadedEvent.AddListener(OnRemoteAvatarDefaultCreat);
                _remoteAvatar.CreateDefaultAvatar();
                //_remoteAvatar.OnDefaultAvatarLoadedEvent.AddListener(OnRemoteAvatarDefaultCreat);
                RequestBaseDataServerRPC(NetworkManager.LocalClientId);
                StartCoroutine(RequestingRemoteStateRoutine());
            }
        }

        public override void OnNetworkDespawn()
        {
            _remoteAvatar.OnDefaultAvatarLoadedEvent.RemoveListener(OnRemoteAvatarDefaultCreat);
            if (IsOwner)
            {
                //_remoteAvatar.AvatarStubVisibilityChanged -= OnRemoteAvatarStubVisibilityChanged;
                //_remoteAvatar.OnDefaultAvatarLoadedEvent -= OnRemoteOnDefaultAvatarLoadedEvent;
            }
            else
            {
                _remoteAvatar.OnDefaultAvatarLoadedEvent.RemoveListener(OnRemoteAvatarDefaultCreat);
            }
            base.OnNetworkDespawn();
        }

        public void InitAsLocal(SampleAvatarEntity playerAvatar, bool isUserCustomAvatar, int assetId, ulong userId)
        {
            _localAvatar = playerAvatar;

            LoadAvatar(isUserCustomAvatar, assetId, userId);

            if (!IsInitialized)
            {
                StartCoroutine(UpdatingRemoteStateRoutine());
                StartCoroutine(WaitingForTrackingPoseValid());
                IsInitialized = true;
            }
        }

        public void SetVisibleToOthers(bool isVisible)
        {
            SetVisibleToOthersServerRpc(isVisible);
        }

        public void HideDefaultRemoteAvatar()
        {
            //_remoteAvatar.gameObject.SetActive(false);
            _remoteAvatar.TooggleVisibleDefaultAvatar(false);
        }

        public void LoadAvatar(bool isUserCustomAvatar, int assetId, ulong userId)
        {
            //if (isUserCustomAvatar && _isUserCustomAvatarInitialized)
            //{
            //    return;
            //}
            _remoteAvatar.gameObject.SetActive(true);
            //_remoteAvatar.CreateDefaultAvatar();
            _remoteAvatar.TooggleVisibleDefaultAvatar(false);
            if (isUserCustomAvatar)
            {
                //_isCdnAvatarAvailable = true;
                if (!_isCdnAvatarAvailable)
                {
                    _remoteAvatar.OnUserAvatarLoadedEvent.AddListener(RemoteAvatar_OnUserAvatarLoadedEvent);
                    _remoteAvatar.LoadRemoteUserCdnAvatar(userId);
                }
            }
            else
            {
                LoadDefaultRemoteAvatar(assetId, true);
            }

            StartCoroutine(SetAvatarLayer(_localAvatar, _localBodyLayerMask));

            _remoteAvatar.transform.SetPositionAndRotation(_localAvatar.transform.position, _localAvatar.transform.rotation);

            _baseAvatarData = new BaseSerializableAvatarData();
            _baseAvatarData.position = _remoteAvatar.transform.localPosition;
            _baseAvatarData.rotation = _remoteAvatar.transform.localRotation;
            _baseAvatarData.isUserCustomAvatar = isUserCustomAvatar;
            _baseAvatarData.assetId = assetId;
            _baseAvatarData.userId = userId;

            TransferBaseDataServerRpc(_baseAvatarData);
        }

        private void OnRemoteAvatarStubVisibilityChanged(bool isVisible)
        {
            if (isVisible)
            {
                SetLayerHierarchically(_remoteAvatar.transform, _mirrorRenderingLayerMask);
            }
        }

        private void OnRemoteAvatarDefaultCreat(OvrAvatarEntity avatarEntity)
        {
            //_remoteAvatar.TooggleVisibleDefaultAvatar(true);
            if (IsOwner)
            {
                SetLayerHierarchically(_remoteAvatar.transform, _mirrorRenderingLayerMask);
            }
        }

        private void RemoteAvatar_OnDefaultAvatarLoadedEvent(OvrAvatarEntity avatar)
        {
            _remoteAvatar.OnDefaultAvatarLoadedEvent.RemoveListener(RemoteAvatar_OnDefaultAvatarLoadedEvent);
            if (AvatarSessionData.AvatarAssetId.Value == AvatarSessionData.DefaultAvatarAssetId)
            {
                _defaultAssetAvatarEntity.CreateDefaultAvatar();
                OnRemoteAvatarDefaultCreat(_remoteAvatar);
                //_remoteAvatar.TooggleVisibleDefaultAvatar(true);
            }
            else
            {
                _defaultAssetAvatarEntity.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.Value.ToString(),
                    SampleAvatarEntity.AssetSource.StreamingAssets);
            }
            //_remoteAvatar.TooggleVisibleDefaultAvatar(true);
            StartCoroutine(SetAvatarLayer(_remoteAvatar, _mirrorRenderingLayerMask));
        }

        private void RemoteAvatar_OnUserAvatarLoadedEvent(OvrAvatarEntity avatar)
        {
            _isCdnAvatarAvailable = true;
            _remoteAvatar.OnUserAvatarLoadedEvent.RemoveListener(RemoteAvatar_OnUserAvatarLoadedEvent);
            if (AvatarSessionData.AvatarAssetId.Value == AvatarSessionData.DefaultAvatarAssetId)
            {
                _defaultAssetAvatarEntity.CreateDefaultAvatar();
                OnRemoteAvatarDefaultCreat(_remoteAvatar);
                //_remoteAvatar.TooggleVisibleDefaultAvatar(true);
            }
            else
            {
                _defaultAssetAvatarEntity.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.Value.ToString(),
                    SampleAvatarEntity.AssetSource.StreamingAssets);
            }
            //_remoteAvatar.TooggleVisibleDefaultAvatar(true);
            //_defaultAssetAvatarEntity.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.Value.ToString(),
            //    SampleAvatarEntity.AssetSource.StreamingAssets);
            StartCoroutine(SetAvatarLayer(_remoteAvatar, _mirrorRenderingLayerMask));
        }

        private IEnumerator SetAvatarLayer(OvrAvatarEntity avatar, LayerMask layerMask)
        {
            while (!avatar.IsCreated || avatar.SkeletonJointCount == 0)
            {
                yield return null;
            }

            SetLayerHierarchically(avatar.transform, layerMask);

            while (avatar.IsPendingAvatar || avatar.transform.childCount < 4)
            {
                yield return null;
            }

            SetLayerHierarchically(avatar.transform, layerMask);
        }

        private void SetLayerHierarchically(Transform transform, LayerMask layerMask)
        {
            int layerValue = (int)Mathf.Log(layerMask.value, 2);
            transform.gameObject.layer = layerValue;
            SetChildrensLayer(transform, layerValue);
        }

        private void SetChildrensLayer(Transform parent, int layerValue)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.layer = layerValue;
                SetChildrensLayer(child, layerValue);
            }
        }

        private void SetRemoteAvatarPoseData(List<PacketData> packets)
        {
            if (!_remoteAvatar.isActiveAndEnabled || _remoteAvatar.SkeletonJointCount == 0)
            {
                return;
            }

            foreach (var loopbackState in _loopbackStates.Values)
            {
                foreach (var packet in packets)
                {
                    loopbackState.packetQueue.Add(packet);
                }
            }
        }

        private void Awake()
        {
            _loopbackAvatars = new List<OvrAvatarEntity>();
            _loopbackAvatars.Add(_remoteAvatar);

            _isVisibleToOthers = new(true);
        }

        private void OnEnable()
        {
            _isVisibleToOthers.OnValueChanged += OnVisibleToOthersValueChanged;
        }

        private void OnDisable()
        {
            _isVisibleToOthers.OnValueChanged -= OnVisibleToOthersValueChanged;
        }

        protected void Start()
        {
            if ((!IsClient && NetworkManager != null) || AvatarLODManager.Instance == null)
            //  if (!IsClient  || AvatarLODManager.Instance == null)
            {
                enabled = false;
                return;
            }

            // TODO: Bring back deleted check for other LoopbackManagers in the current scene

            AvatarLODManager.Instance.enableDynamicStreaming = true;

            float firstValue = UnityEngine.Random.Range(_simulatedLatencySettings.fakeLatencyMin, _simulatedLatencySettings.fakeLatencyMax);
            _simulatedLatencySettings.latencyValues.Insert(0, firstValue);
            _simulatedLatencySettings.latencySum += firstValue;

            _pinnedBuffer = GCHandle.Alloc(_packetBuffer, GCHandleType.Pinned);

            CreateStates();
        }

        private void CreateStates()
        {
            foreach (var item in _loopbackStates)
            {
                foreach (var packet in item.Value.packetQueue)
                {
                    if (packet.Release())
                    {
                        ReturnPacket(packet);
                    }
                }
            }
            _loopbackStates.Clear();

            foreach (var loopbackAvatar in _loopbackAvatars)
            {
                LoopbackState loopbackState = new();
                loopbackState.requestedLod = StreamLOD.Medium;
                _loopbackStates.Add(loopbackAvatar, new LoopbackState());
            }
        }

        public override void OnDestroy()
        {
            if (_pinnedBuffer.IsAllocated)
            {
                _pinnedBuffer.Free();
            }

            foreach (var item in _loopbackStates)
            {
                foreach (var packet in item.Value.packetQueue)
                {
                    if (packet.Release())
                    {
                        ReturnPacket(packet);
                    }
                }
            }

            foreach (var packet in _packetPool)
            {
                packet.Dispose();
            }
            _packetPool.Clear();

            base.OnDestroy();
        }

        private void Update()
        {
            if (_remoteAvatar == null)
            {
                return;
            }

            if (!_remoteAvatar.isActiveAndEnabled || _remoteAvatar.SkeletonJointCount == 0)
            {
                return;
            }

            if (IsOwner || NetworkManager == null)
            {
                if (_localAvatar == null || _localAvatar.SkeletonJointCount == 0)
                {
                    return;
                }

                for (int i = 0; i < OvrAvatarEntity.StreamLODCount; ++i)
                {
                    _remoteAvatar.ApplyStreamData(_localAvatar.RecordStreamData((StreamLOD)i));

                    // Assume remote Avatar StreamLOD sizes are the same
                    float streamBytesPerSecond = _localAvatar.GetLastByteSizeForLodIndex(i) / StreamLodSnapshotIntervalSeconds[i];
                    AvatarLODManager.Instance.dynamicStreamLodBitsPerSecond[i] = (long)(streamBytesPerSecond * 8);
                }
            }

            if (!IsOwner || IsHost)
            {
                foreach (var item in _loopbackStates)
                {
                    var loopbackAvatar = item.Key;
                    var loopbackState = item.Value;

                    if (!loopbackAvatar.IsCreated)
                    {
                        continue;
                    }

                    UpdatePlaybackTimeDelay(loopbackAvatar, loopbackState);

                    // "Remote" avatar receives incoming data and applies if it is the correct lod
                    if (loopbackState.packetQueue.Count > 0)
                    {
                        foreach (var packet in loopbackState.packetQueue)
                        {
                            if (_isCdnAvatarAvailable)
                            {
                                if (packet.isCdnAvatar)
                                {
                                    loopbackAvatar.ApplyStreamData(packet.data);
                                }
                            }
                            else
                            {
                                if (!packet.isCdnAvatar)
                                {
                                    loopbackAvatar.ApplyStreamData(packet.data);
                                }
                            }

                            _deadList.Add(packet);
                        }

                        foreach (var packet in _deadList)
                        {
                            loopbackState.packetQueue.Remove(packet);
                        }
                        _deadList.Clear();
                    }

                    // "Send" the lod that "remote" avatar wants to use back over the network
                    // TODO delay this reception for an accurate test
                    //loopbackState.requestedLod = loopbackAvatar.activeStreamLod;
                    //loopbackState.requestedLod = StreamLOD.Medium;
                }
            }
        }

        private PacketData GetPacketForEntityAtLOD(OvrAvatarEntity entity, StreamLOD lod, bool isCdnAvatar)
        {
            PacketData packet;
            int poolCount = _packetPool.Count;
            if (poolCount > 0)
            {
                var lastIdx = poolCount - 1;
                packet = _packetPool[lastIdx];
                _packetPool.RemoveAt(lastIdx);
            }
            else
            {
                packet = new PacketData();
            }

            packet.isCdnAvatar = isCdnAvatar;
            packet.lod = lod;
            return packet.Retain();
        }
        private void ReturnPacket(PacketData packet)
        {
            Debug.Assert(packet.Unretained);
            _packetPool.Add(packet);
        }

        #region Local Avatar

        private List<PacketData> SendSnapshot()
        {
            List<PacketData> packetQueue = new();

            if (!_localAvatar.HasJoints) { return packetQueue; }

            AddAvatarPacketToQueue(packetQueue, _loopbackStates[_remoteAvatar].requestedLod);

            return packetQueue;
        }

        private void AddAvatarPacketToQueue(List<PacketData> packetQueue, StreamLOD streamLod)
        {
            if (_nextAvatarToSend == null)
            {
                _nextAvatarToSend = _defaultAssetAvatarEntity;
            }

            if (_nextAvatarToSend == _localAvatar)
            {
                packetQueue.AddRange(SendPacket(_localAvatar, streamLod, true));
                _nextAvatarToSend = _defaultAssetAvatarEntity;
            }
            else
            {
                packetQueue.AddRange(SendPacket(_defaultAssetAvatarEntity, streamLod, false));
                if (_isCdnAvatarAvailable)
                {
                    _nextAvatarToSend = _localAvatar;
                }
            }
        }

        private List<PacketData> SendPacket(SampleAvatarEntity recipientAvatar, StreamLOD lod, bool isCdnAvatar)
        {
            PacketData packet = GetPacketForEntityAtLOD(recipientAvatar, lod, isCdnAvatar);

            packet.data = recipientAvatar.RecordStreamData(lod);

            Debug.Assert(packet.data.Length > 0);

            List<PacketData> packetQueue = new();
            foreach (var loopbackState in _loopbackStates.Values)
            {
                if (loopbackState.requestedLod == lod)
                {
                    packet.fakeLatency = _simulatedLatencySettings.NextValue();

                    packetQueue.Add(packet.Retain());
                }
            }

            if (packet.Release())
            {
                ReturnPacket(packet);
            }
            return packetQueue;
        }

        private IEnumerator UpdatingRemoteStateRoutine()
        {
            while (_localAvatar == null || !_remoteAvatar.isActiveAndEnabled || _remoteAvatar.SkeletonJointCount == 0)
            {
                yield return null;
            }

            if (NetworkManager == null)
            {
                yield break;
            }

            _poseAvatarData = new();
            _sendTimeout = new(0.2f, 5, 2);

            // Can be increased for saving network bandwidth
            float sendingDelay = 2f / NetworkManager.NetworkConfig.TickRate;

            while (true)
            {
                yield return new WaitForSeconds(sendingDelay);
                yield return new WaitForEndOfFrame();

                if (_defaultAssetAvatarEntity.SkeletonJointCount == 0)
                {
                    continue;
                }

                if (NetworkManager == null)
                {
                    yield break;
                }

                if (_unreceivedByServerPoseDataCount < MaxUnreceivedByServerPoseDataCount)
                {
                    List<PacketData> packetsToSend = SendSnapshot();
                    if (packetsToSend.Count > 0)
                    {
                        ++_unreceivedByServerPoseDataCount;
                        _poseAvatarData.packets = packetsToSend;

                        TransferPoseDataServerRpc(NetworkManager.LocalClientId, _poseAvatarData);

                        foreach (var packet in packetsToSend)
                        {
                            if (packet.Release())
                            {
                                ReturnPacket(packet);
                            }
                        }
                        packetsToSend.Clear();
                    }
                }
                else
                {
                    _currentSendDelay += sendingDelay + Time.deltaTime;
                    if (_currentSendDelay > _sendTimeout.CurrentValue)
                    {
                        _currentSendDelay = 0;
                        _unreceivedByServerPoseDataCount = 0;
                        _sendTimeout.Increase();
                    }
                }
            }
        }

        private IEnumerator RequestingRemoteStateRoutine()
        {
            // Can be increased for saving network bandwidth
            float requestingDelay = 2f / NetworkManager.NetworkConfig.TickRate;

            _requestTimeout = new(0.2f, 5, 2);

            while (true)
            {
                yield return new WaitForSeconds(requestingDelay);

                if (NetworkManager == null)
                {
                    yield break;
                }

                if (_unreceivedRequestedPoseDataCount < MaxUnperformedRequestCount)
                {
                    RequestPoseDataServerRPC(NetworkManager.LocalClientId);
                    ++_unreceivedRequestedPoseDataCount;
                }
                else
                {
                    _currentRequestDelay += requestingDelay;
                    if (_currentRequestDelay > _requestTimeout.CurrentValue)
                    {
                        _currentRequestDelay = 0;
                        _unreceivedRequestedPoseDataCount = 0;
                        _requestTimeout.Increase();
                    }
                }
            }
        }

        private IEnumerator WaitingForTrackingPoseValid()
        {
            // Avatar will have default pose, rotation and position until HMD mounted (or in PC mode)
            while (_localAvatar == null || _remoteAvatar == null)
            {
                yield return null;
            }
            SetAvatarPositionAndForward(_localAvatar.transform.localPosition, -_localAvatar.transform.forward);

            while (!_localAvatar.TrackingPoseValid)
            {
                yield return null;
                if (_localAvatar == null)
                {
                    yield break;
                }
            }

            SetAvatarPositionAndForward(Vector3.zero, -_localAvatar.transform.forward);
        }

        private void SetAvatarPositionAndForward(Vector3 position, Vector3 forward)
        {
            _localAvatar.transform.localPosition = position;
            _localAvatar.transform.forward = forward;
            _remoteAvatar.transform.SetPositionAndRotation(_localAvatar.transform.position, _localAvatar.transform.rotation);

            _baseAvatarData.position = _remoteAvatar.transform.localPosition;
            _baseAvatarData.rotation = _remoteAvatar.transform.localRotation;
            TransferBaseDataServerRpc(_baseAvatarData);
        }

        #endregion

        #region "Remote" Loopback Avatar

        private void UpdatePlaybackTimeDelay(OvrAvatarEntity loopbackAvatar, LoopbackState loopbackState)
        {
            // In a real network, maximum packet variation should be computed from the network jitter
            float latencyVariationS = (_simulatedLatencySettings.fakeLatencyMax - _simulatedLatencySettings.fakeLatencyMin);

            // Push back the playback time by the snapshot interval
            float snapshotIntervalS = StreamLodSnapshotIntervalSeconds[(int)loopbackAvatar.activeStreamLod];

            // Sum the latency variation and snapshot rate to determine the playback position
            float playbackDelayS = latencyVariationS + snapshotIntervalS;

            // blend to the target using PLAYBACK_SMOOTH_FACTOR
            loopbackState.smoothedPlaybackDelay = Mathf.Lerp(loopbackState.smoothedPlaybackDelay, playbackDelayS, PLAYBACK_SMOOTH_FACTOR);

            loopbackAvatar.SetPlaybackTimeDelay(loopbackState.smoothedPlaybackDelay);
        }

        #endregion

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void TransferPoseDataServerRpc(ulong clientId, SerializableAvatarData serializableAvatarData)
        {
            // TODO: Save all avatar lod data on server, give specific lod depending on client

            _poseAvatarData = serializableAvatarData;
            _sendedToClients.Clear();

            if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                return;
            }

            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            ConfirmPoseDataReceivedByServerClientRpc(clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
        private void RequestPoseDataServerRPC(ulong clientId)
        {
            if (_poseAvatarData == null || _poseAvatarData.packets == null || _poseAvatarData.packets.Count == 0)
            {
                return;
            }

            if (!NetworkManager.ConnectedClientsIds.Contains(clientId))
            {
                return;
            }

            if (_sendedToClients.Contains(clientId))
            {
                return;
            }
            _sendedToClients.Add(clientId);

            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            TransferPoseDataClientRpc(_poseAvatarData, clientRpcParams);
        }

        [ServerRpc]
        private void SetVisibleToOthersServerRpc(bool isVisible)
        {
            _isVisibleToOthers.Value = isVisible;
        }

        [ServerRpc]
        private void TransferBaseDataServerRpc(BaseSerializableAvatarData baseSerializableAvatarData)
        {
            if (IsHost)
            {
                SetRemoteAvatarBaseData(baseSerializableAvatarData);
            }
            else
            {
                _baseAvatarData = baseSerializableAvatarData;
            }

            TransferBaseDataClientRpc(baseSerializableAvatarData);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestBaseDataServerRPC(ulong clientId)
        {
            if (_baseAvatarData != null)
            {
                ClientRpcParams clientRpcParams = new()
                {
                    Send = new ClientRpcSendParams()
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                };

                TransferBaseDataClientRpc(_baseAvatarData, clientRpcParams);
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void TransferPoseDataClientRpc(SerializableAvatarData serializableAvatarData, ClientRpcParams clientRpcParams = default)
        {
            SetRemoteAvatarPoseData(serializableAvatarData.packets);
            _currentRequestDelay = 0;
            _unreceivedRequestedPoseDataCount = 0;
            _requestTimeout.Reset();
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void ConfirmPoseDataReceivedByServerClientRpc(ClientRpcParams clientRpcParams = default)
        {
            _currentSendDelay = 0;
            _unreceivedByServerPoseDataCount = 0;
            _sendTimeout.Reset();
        }

        [ClientRpc]
        private void TransferBaseDataClientRpc(BaseSerializableAvatarData newBaseData,
            ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner)
            {
                return;
            }

            SetRemoteAvatarBaseData(newBaseData);
        }

        private void SetRemoteAvatarBaseData(BaseSerializableAvatarData newBaseData)
        {
            _baseAvatarData = newBaseData;

            if (_baseAvatarData.isUserCustomAvatar)
            {
                LoadCdnRemoteAvatar(_baseAvatarData.userId, _baseAvatarData.assetId).Forget();
            }
            else
            {
                LoadDefaultRemoteAvatar(_baseAvatarData.assetId);
            }

            _remoteAvatar.transform.localPosition = newBaseData.position;
            _remoteAvatar.transform.localRotation = newBaseData.rotation;
            RefreshByVisibleToOthersValue();
        }

        private async UniTask LoadCdnRemoteAvatar(ulong userId, int assetId)
        {
            if (!await _authService.IsCdnAvailable())
            {
                LoadDefaultRemoteAvatar(assetId);
                return;
            }

            if (_remoteAvatar == null)
            {
                return;
            }
            ulong localUserId = await _authService.LogInUser();

            if (localUserId == 0 || !await _authService.IsUserHasAvatar(userId))
            {
                LoadDefaultRemoteAvatar(assetId);
                return;
            }

            if (_remoteAvatar == null)
            {
                return;
            }

            _isCdnAvatarAvailable = true;

            _remoteAvatar.OnLoadFailedEvent.RemoveListener(OnRemoteAvatarLoadFailed);
            _remoteAvatar.OnLoadFailedEvent.AddListener(OnRemoteAvatarLoadFailed);
            _remoteAvatar.LoadRemoteUserCdnAvatar(userId);
        }

        private void LoadDefaultRemoteAvatar(int assetId, bool raiseOnLoadCallback = false)
        {
            if (_remoteAvatar == null)
            {
                return;
            }

            if (raiseOnLoadCallback)
            {
                _remoteAvatar.OnDefaultAvatarLoadedEvent.AddListener(RemoteAvatar_OnDefaultAvatarLoadedEvent);
            }

            if (assetId == AvatarSessionData.DefaultAvatarAssetId)
            {
                _remoteAvatar.CreateDefaultAvatar();
            }
            else
                _remoteAvatar.ReloadAvatarManually(assetId.ToString(), SampleAvatarEntity.AssetSource.StreamingAssets);
        }

        private void OnRemoteAvatarLoadFailed(OvrAvatarEntity avatar, CAPI.ovrAvatar2LoadRequestInfo loadInfo)
        {
            _isCdnAvatarAvailable = false;
            _remoteAvatar.OnLoadFailedEvent.RemoveListener(OnRemoteAvatarLoadFailed);
            LoadDefaultRemoteAvatar(_baseAvatarData.assetId);
        }

        private void OnVisibleToOthersValueChanged(bool previousValue, bool newValue)
        {
            RefreshByVisibleToOthersValue();
        }

        private void RefreshByVisibleToOthersValue()
        {
            if (IsOwner)
            {
                return;
            }

            _remoteAvatar.transform.localPosition = _isVisibleToOthers.Value ? _baseAvatarData.position : new Vector3(0, -1000, 0);
        }
    }

    [System.Serializable]
    public class PacketData : IDisposable
    {
        public byte[] data;
        public StreamLOD lod;
        public float fakeLatency;
        public bool isCdnAvatar;
        private uint refCount = 0;

        public PacketData() { }

        ~PacketData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            data = default;
        }

        public bool Unretained => refCount == 0;
        public PacketData Retain() { ++refCount; return this; }
        public bool Release()
        {
            return --refCount == 0;
        }

        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(data.Length);
                    writer.Write(data);
                    writer.Write((byte)lod);
                    writer.Write(isCdnAvatar);
                }
                return m.ToArray();
            }
        }

        public static PacketData Deserialize(byte[] data)
        {
            PacketData result = new PacketData();

            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    int dataLength = reader.ReadInt32();
                    result.data = reader.ReadBytes(dataLength);
                    result.lod = (StreamLOD)reader.ReadByte();
                    result.isCdnAvatar = reader.ReadBoolean();
                }
            }
            return result;
        }
    };

    public class BaseSerializableAvatarData : INetworkSerializable
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isUserCustomAvatar;
        public int assetId;
        public ulong userId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref isUserCustomAvatar);
            serializer.SerializeValue(ref assetId);
            serializer.SerializeValue(ref userId);
        }
    }

    [Serializable]
    public class SerializableAvatarData : INetworkSerializable
    {
        public List<PacketData> packets;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                SerializeForNetwork(serializer);
            }
            else
            {
                DeserializeForNetwork(serializer);
            }
        }

        private void SerializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            byte[] avatarBytes = SerializeData();
            serializer.SerializeValue(ref avatarBytes);
        }

        private void DeserializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            byte[] avatarBytes = null;
            serializer.SerializeValue(ref avatarBytes);
            DeserializeData(avatarBytes);
        }

        public static SerializableAvatarData Deserialize(byte[] data)
        {
            SerializableAvatarData result = new();
            result.DeserializeData(data);
            return result;
        }

        public byte[] SerializeData()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(packets.Count);
                    foreach (var packet in packets)
                    {
                        byte[] data = packet.Serialize();
                        writer.Write(data.Length);
                        writer.Write(data);
                    }
                }
                return m.ToArray();
            }
        }

        public void DeserializeData(byte[] data)
        {
            packets = new();

            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    int packetsCount = reader.ReadInt32();
                    for (int i = 0; i < packetsCount; i++)
                    {
                        int dataLength = reader.ReadInt32();
                        byte[] packetData = reader.ReadBytes(dataLength);
                        PacketData packet = PacketData.Deserialize(packetData);
                        packets.Add(packet);
                    }
                }
            }
        }
    }
}