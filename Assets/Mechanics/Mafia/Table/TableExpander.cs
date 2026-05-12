using Assets.Localization;
using Assets.Scripts.Player;
using Assets.Scripts.Zones;
using Sense.Interectable.Teleportation;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Assets.Mechanics.Mafia.UI;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Assets.Scripts.Server;
using static Assets.Scripts.Server.ServerVerification;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading.Tasks;
using Assets.Scripts.Player.WindowsMovement;
using Mechanics.LoadSceneObjects.Models;
using Assets.Scripts.Data;
using System.Globalization;

namespace Assets.Mechanics.Mafia.Table
{
    public class TableExpander : NetworkBehaviour
    {
        [SerializeField] private MafiaAdminPanelViewPanel _mafiaPanel;
        [SerializeField] private MafiaWinnerCup _mafiaWinerPanel;
        [SerializeField] private List<ZoneVariant> _zoneVariants;
        [SerializeField] private NetworkVariable<int> _zoneSize;
        [SerializeField] private NetworkVariable<ulong> _adminId;
        [SerializeField] private NetworkVariable<bool> _gameIsActive;

        private static int _lastGlobalTableNumber;

        public ZonesModel Zones;

        private PlayerLogic _player;
        private OnPlayerUI _onPlayerUI;
        private ServerVerification _serverVerification;

        private Coroutine _changeZoneObjectsRoutine;
        private TableData _tableData;
        private bool _isOccupyingAvailable = true;

        public TableData TableData => _tableData;

        public event Action AdminChanged;

        private void Awake()
        {
            _adminId = new NetworkVariable<ulong>();
            _serverVerification = FindObjectOfType<ServerVerification>();

#if UNITY_SERVER
            _tableData = new();

            ++_lastGlobalTableNumber;
            _tableData.TableNumber = _lastGlobalTableNumber;
            _tableData.TableId = GetTableId(DataExtensions.GetSpaceID(), _lastGlobalTableNumber.ToString());
#endif
        }

        private void Start()
        {
            Zones = FindObjectOfType<ZonesModel>();
            _player = Zones?.PlayerOwner;
            _mafiaPanel.GetComponent<LookAtPlayer>().SetPlayer(_player?.transform);
            _mafiaWinerPanel.SetPlayer(_player?.transform);
            _onPlayerUI = Zones?.OnPlayerUi;

            foreach (var zoneVariant in _zoneVariants)
            {
                foreach (var place in zoneVariant.Places)
                {
                    place.Init(Zones, null);
                }
            }

            if (_adminId.Value != 0 && _adminId.Value != Zones.OwnerId) _mafiaPanel.SetPanelStatus(isBlocked: true);
        }

        public void Init(StaticObject staticObjectData)
        {
            if (staticObjectData.Data == null)
            {
                return;
            }

            _tableData.CustomData = new();
            foreach (var item in staticObjectData.Data)
            {
                _tableData.CustomData[item.Key] = item.Value;
            }
        }

        public bool IsLocalClientAdmin()
        {
            return _adminId != null && _adminId.Value == NetworkManager.LocalClientId;
        }

        public void KickParticipant(Guid guid)
        {
            string guidString = guid.ToString();
            
            ClientInfo clientToKick = _serverVerification.Users.FirstOrDefault(x => x.Id == guidString);
            if (clientToKick == null || !NetworkManager.ConnectedClientsIds.Contains(clientToKick.ServerId))
            {
                return;
            }

            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientToKick.ServerId },
                }
            };
            KickParticipantClientRpc(clientRpcParams);
        }

        public void SetState(GameState gameState)
        {
            SetActiveGameStatusForNetworkTable(gameState != null);

            if (gameState == null || gameState.PlayerStates == null)
            {
                return;
            }

            string adminGuid = gameState.GameMasterId.ToString();
            ClientInfo adminClientInfo = _serverVerification.Users.FirstOrDefault(x => x.Id == adminGuid);
            if (adminClientInfo == null)
            {
                Debug.LogError("Mafia admin is absent!");
                return;
            }
            
            if (_adminId.Value != adminClientInfo.ServerId)
            {
                _adminId.Value = adminClientInfo.ServerId;
            }
        }

        public int GetChairIndexByPlace(Place place)
        {
            int chairIndex = -1;
            if (place == null)
            {
                return chairIndex;
            }

            for (int i = 0; i < _zoneVariants[_zoneSize.Value].Places.Count; i++)
            {
                if (_zoneVariants[_zoneSize.Value].Places[i] == place)
                {
                    chairIndex = i;
                    break;
                }
            }

            return chairIndex;
        }

        public bool TryGetLocalClientPlace(out Place place)
        {
            place = null;
            foreach (var zonePlace in _zoneVariants[_zoneSize.Value].Places)
            {
                if (zonePlace.IsOccupiedID.Value == NetworkManager.LocalClientId)
                {
                    place = zonePlace;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetPlaceByChairIndex(int chairIndex, out Place place)
        {
            place = null;
            if (chairIndex < 0 || chairIndex >= _zoneVariants[_zoneSize.Value].Places.Count)
            {
                return false;
            }

            place = _zoneVariants[_zoneSize.Value].Places[chairIndex];

            return place != null;
        }

        public void KickLocalClient(bool setPositionToZero = true)
        {
            if (!TryGetLocalClientPlace(out Place place))
            {
                return;
            }
            place.LeavePlace();

            if (setPositionToZero && Zones != null && Zones.PlayerOwner != null)
            {
                Zones.PlayerOwner.SetPositionToZero();
            }
        }

        public List<Place> GetOccupiedPlaces()
        {
            List<Place> occupiedPlaces = new();

            foreach (var place in _zoneVariants[_zoneSize.Value].Places)
            {
                if (place.IsOccupiedID.Value != 0)
                {
                    occupiedPlaces.Add(place);
                }
            }
            return occupiedPlaces;
        }

        public List<Place> GetOccupiedPlacesWithoutAdmin()
        {
            List<Place> occupiedPlaces = new();

            foreach (var place in _zoneVariants[_zoneSize.Value].Places)
            {
                if (place.IsOccupiedID.Value != 0)
                {
                    if (place.IsOccupiedID.Value !=  Zones.OwnerId) occupiedPlaces.Add(place);
                }
            }
            return occupiedPlaces;
        }

        public override void OnNetworkSpawn()
        {
#if !UNITY_SERVER
            SwitchZoneObjectsAtStart();
            BlockTeleports(_gameIsActive.Value);
#endif
#if UNITY_SERVER
            _gameIsActive.Value = false;
#endif

            if (IsClient)
            {
                RequestTableDataServerRpc(NetworkManager.LocalClientId);
            }
        }

        private void OnEnable()
        {
#if !UNITY_SERVER
            _mafiaPanel.SizeClicked += SetZoneSize;
            _mafiaPanel.AdminChangeRequested += _mafiaPanel_AdminChangeRequested;
            _adminId.OnValueChanged += OnAdminChanged;
            _gameIsActive.OnValueChanged += OnGameActiveChanged;
#endif
#if UNITY_SERVER
            foreach (var zone in _zoneVariants)
            {
                foreach (var place in zone.Places)
                {
                    place.IsOccupiedID.OnValueChanged += PlacesOccupationUpdate;
                }
            }
#endif
                _zoneSize.OnValueChanged += ChangeZoneObjects;
        }

        private void _mafiaPanel_AdminChangeRequested(ulong id)
        {
            SetAdminServerRpc(id);
        }

        private void OnAdminChanged(ulong previousvalue, ulong newvalue)
        {
            if (newvalue == Zones.OwnerId && newvalue != 0)
            {
                _mafiaPanel.SetPanelStatus(isBlocked:false);
            }
            else
            {
                _mafiaPanel.SetPanelStatus(isBlocked:true);
            }
            AdminChanged?.Invoke();
        }

        private void PlacesOccupationUpdate(ulong previousvalue, ulong newvalue) //server method
        {
            CheckClientsOnServerAndSendToUi(previousvalue, newvalue);
        }

        private void ChangeZoneObjects(int oldv, int newv)
        {
            _player = Zones.PlayerOwner;
#if !UNITY_SERVER

            if (_changeZoneObjectsRoutine == null) _changeZoneObjectsRoutine = StartCoroutine(ChangeZoneObjectsRoutine(oldv, newv));
#endif
        }

        private void SwitchPlaces(List<Place> places, bool flag)
        {
            if (places.Count == 0) return;
            foreach (var place in places)
            {
                place.SwitchPlace(flag);
                if (flag && Zones != null && Zones.GetTeleportationProvider() != null)
                {
                    place.GetComponent<CustomTeleportationAnchor>().teleportationProvider = Zones.GetTeleportationProvider();
                }
            }
        }
        private void SwitchModels(List<GameObject> models, bool flag)
        {
            if (models.Count == 0) return;
            foreach (var model in models)
            {
                model.SetActive(flag);
            }
        }

        private void SwitchZoneObjectsAtStart()
        {
            if (_zoneVariants == null || _zoneVariants.Count == 0) return;
            foreach (var zone in _zoneVariants)
            {
                SwitchPlaces(zone.Places, false);
                SwitchModels(zone.Models, false);
            }

            SwitchPlaces(_zoneVariants[_zoneSize.Value].Places, true);
            SwitchModels(_zoneVariants[_zoneSize.Value].Models, true);

            RefreshActivePlacesColliderStates();
        }

        private void OnDisable()
        {
#if !UNITY_SERVER
            _mafiaPanel.SizeClicked -= SetZoneSize;
            _mafiaPanel.AdminChangeRequested -= _mafiaPanel_AdminChangeRequested;
            _gameIsActive.OnValueChanged -= OnGameActiveChanged;

            foreach (var zone in _zoneVariants)
            {
                foreach (var place in zone.Places)
                {
                    place.IsOccupiedID.OnValueChanged -= PlacesOccupationUpdate;
                }
            }
#endif
            _zoneSize.OnValueChanged -= ChangeZoneObjects;
            if (_changeZoneObjectsRoutine != null) StopCoroutine(_changeZoneObjectsRoutine);
        }


        private void OnGameActiveChanged(bool oldValue, bool newValue)
        {
            BlockTeleports(newValue);
        }

        public void UpdateBlockForNonPlayers()
        {
            BlockTeleports(_gameIsActive.Value);
        }

        public void SetOccupyingAvailable(bool isOccupyingAvailable)
        {
            _isOccupyingAvailable = isOccupyingAvailable;
            RefreshActivePlacesColliderStates();
        }

        /// <summary>
        /// Works when client entered the scene while gaming,
        /// or just not gaming client at mafia scene while other are gaming
        /// </summary>
        /// <param name="isGameActive"></param>
        private void BlockTeleports(bool isGameActive)
        {
            foreach (var place in _zoneVariants[_zoneSize.Value].Places)
            {
                place.SwitchPlaceCollider(!isGameActive);
            }

            if (!isGameActive)
            {
                RefreshActivePlacesColliderStates();
            }

            //blocking second floor door
            var teleports = FindObjectsOfType<CustomTeleportationRandomAnchors>();
            foreach (var teleport in teleports)
            {
                teleport.enabled = !isGameActive;

                var winTeleport = teleport.GetComponent<WinClientTeleport>();
                if (winTeleport != null) winTeleport.enabled = !isGameActive;
            }
        }

        /// <summary>
        /// MafiaCompositionRootClient switch ALL teleports at the scene for gamers,
        /// it's too mush, cause we don't need being enabled places of other table variants
        /// </summary>
        public void BlockUnnecessaryTeleportsAfterSwitchOnAll()
        {
            for (int i = 0; i < _zoneVariants.Count; i++)
            {
                if (i == _zoneSize.Value) continue;

                foreach (var place in _zoneVariants[i].Places)
                {
                    place.SwitchPlaceCollider(false);
                }
            }
        }

        private void RefreshActivePlacesColliderStates()
        {
            foreach (var place in _zoneVariants[_zoneSize.Value].Places)
            {
                place.SwitchPlaceCollider(_isOccupyingAvailable);
            }
        }

        private IEnumerator ChangeZoneObjectsRoutine(int oldv, int newv)
        {
            Task fadingTask = _onPlayerUI.FadeToBlackDefault().AsTask();
            yield return new WaitUntil(() => fadingTask.IsCompleted);

            SwitchPlaces(_zoneVariants[newv].Places, true);
            SwitchModels(_zoneVariants[newv].Models, true);
            SwitchModels(_zoneVariants[oldv].Models, false);

            if (newv > oldv)
            {
                ReplaceMeToBiggerZone(oldv, newv);
            }
            else 
            {
                yield return StartCoroutine(ReplaceMeToSmallerZone(oldv, newv));
            }

            yield return new WaitForSeconds(0.1f);

            SwitchPlaces(_zoneVariants[oldv].Places, false);

            _onPlayerUI.FadeToTransparent().Forget();

            _changeZoneObjectsRoutine = null;

            RefreshActivePlacesColliderStates();
        }

        private void ReplaceMeToBiggerZone(int oldValue, int newValue)
        {
            var myId = Zones.OwnerId;
            int myPlaceNumber = -1;

            for (int i = 0; i < _zoneVariants[oldValue].Places.Count; i++) //search my place number in old zone
            {
                if (_zoneVariants[oldValue].Places[i].IsOccupiedID.Value == myId)
                {
                    myPlaceNumber = i;
                    break;
                }
            }

            if (myPlaceNumber != -1)
            {
                Debug.LogWarning($"put me on {myPlaceNumber} place");
                // put me on the place with the same number in bigger zone
                _zoneVariants[newValue].Places[myPlaceNumber].LeaveAndOccupyPlaceExternal(_zoneVariants[newValue].Places[myPlaceNumber]);
                ResetPlayerPosition(_zoneVariants[newValue].Places[myPlaceNumber]);
            }

            foreach (var place in _zoneVariants[newValue].Places)
            {
                place.HideSignal();
            }
        }

        private IEnumerator ReplaceMeToSmallerZone(int oldValue, int newValue)
        {
            //if (!_zoneController.IsMeAdmin()) yield return new WaitForSeconds(1f); //if I'm not an admin, I have to wait 1 sec for the admin to sit down

            var myId = Zones.OwnerId;
            int myPlaceNumber = -1;

            for (int i = 0; i < _zoneVariants[oldValue].Places.Count; i++) //search my place number in old zone
            {
                if (_zoneVariants[oldValue].Places[i].IsOccupiedID.Value == myId)
                {
                    myPlaceNumber = i;
                    break;
                }
            }

            if (myPlaceNumber != -1)
            {
                if (myPlaceNumber < _zoneVariants[newValue].Places.Count) 
                {
                    // put me on the place with the same number in smaller zone
                    _zoneVariants[newValue].Places[myPlaceNumber].LeaveAndOccupyPlaceExternal(_zoneVariants[newValue].Places[myPlaceNumber]);
                    ResetPlayerPosition(_zoneVariants[newValue].Places[myPlaceNumber]);
                }
                else
                {
                    yield return new WaitForSeconds(myPlaceNumber * 0.1f);

                    int freePlaceNumber = -1;

                    for (int i = 0; i < _zoneVariants[newValue].Places.Count; i++) //search free places
                    {
                        if (_zoneVariants[newValue].Places[i].IsOccupiedID.Value == 0)
                        {
                            freePlaceNumber = i;
                            break;
                        }
                    }

                    if (freePlaceNumber != -1)
                    {
                        // put me on the found place in smaller zone
                        _zoneVariants[newValue].Places[freePlaceNumber].LeaveAndOccupyPlaceExternal(_zoneVariants[newValue].Places[freePlaceNumber]);
                        ResetPlayerPosition(_zoneVariants[newValue].Places[freePlaceNumber]);
                    }
                    else
                    {
                        // put me to 0 place, which is occupied for use auto kick
                        _zoneVariants[newValue].Places[0].LeaveAndOccupyPlaceExternal(_zoneVariants[newValue].Places[0]);
                        ResetPlayerPosition(_zoneVariants[newValue].Places[0]);
                    }
                }
            }

            foreach (var place in _zoneVariants[newValue].Places)
            {
                place.HideSignal();
            }
            yield return null;
        }

        private void ResetPlayerPosition(Place targetPlace)
        {
            if (_player != null)
            {
                _player.transform.position = targetPlace.TeleportAnchor.TeleportAnchorTransform.position;
                _player.transform.rotation = targetPlace.TeleportAnchor.TeleportAnchorTransform.rotation;
            }
        }

        private void SetZoneSize(int size)
        {
            SetZoneSizeServerRPC(size);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetZoneSizeServerRPC(int size)
        {
            _zoneSize.Value = size;
        }

        /// <summary>
        /// Server method
        /// </summary>
        /// <param name="gameState"></param>
        private void SetActiveGameStatusForNetworkTable(bool isActiveGame)
        {
            _gameIsActive.Value = isActiveGame;
        }

        private void CheckClientsOnServerAndSendToUi(ulong previousvalue, ulong newvalue) //server method
        {
            //sending info to clients UI:

            SimpleClientsList clients = new SimpleClientsList()
            {
                Clients = new SimpleClientData[_serverVerification.Users.Count]
            };

            for (int i = 0; i < _serverVerification.Users.Count; i++) 
            {
                clients.Clients[i] = new SimpleClientData()
                {
                    Id = _serverVerification.Users[i].ServerId,
                    Name = _serverVerification.Users[i].UserName,
                    Guid = Guid.Parse(_serverVerification.Users[i].Id)
                };
            }

            //place occupation checking for admin menu:
            if (_adminId.Value == 0)
            {
                _adminId.Value = newvalue;
            }
            else if (_adminId.Value == previousvalue && newvalue == 0)
            {
                if (_gameIsActive.Value)
                {
                    _adminId.Value = 0;
                }
                else
                {
                    List<Place> occupiedPlaces = GetOccupiedPlaces();
                    if (occupiedPlaces.Count > 0)
                    {
                        _adminId.Value = occupiedPlaces[0].IsOccupiedID.Value;
                    }
                    else
                    {
                        _adminId.Value = 0;
                    }
                }
            }
            
            UpdateUiClientRpc(clients);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetAdminServerRpc(ulong id)
        {
            _adminId.Value = id;
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestTableDataServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new ulong[] { clientId };
            SendTableDataClientRpc(TableData);
        }

        [ClientRpc]
        private void KickParticipantClientRpc(ClientRpcParams clientRpcParams = default)
        {
            KickLocalClient();
        }

        [ClientRpc]
        private void UpdateUiClientRpc(SimpleClientsList clients, ClientRpcParams clientRpcParams = default)
        {
            UpdateInfoOnClient(clients).Forget();
        }

        [ClientRpc]
        private void SendTableDataClientRpc(TableData tableData, ClientRpcParams clientRpcParams = default)
        {
            _tableData = tableData;
        }

        private async UniTask UpdateInfoOnClient(SimpleClientsList clients)
        {
            await UniTask.Delay(500);

            List<ulong> participantClientIds = GetOccupiedPlacesWithoutAdmin().Select(x => x.IsOccupiedID.Value).ToList();
            List<SimpleClientData> occupatingClients = new List<SimpleClientData>();

            foreach (var clientId in participantClientIds)
            {
                SimpleClientData clientInfo = Array.Find<SimpleClientData>(clients.Clients, x => x.Id == clientId);

                occupatingClients.Add(clientInfo);
            }
            _mafiaPanel.SetParticipants(occupatingClients);
        }

        private string GetTableId(string spaceId, string tableIndex)
        {
            return $"{spaceId}_{tableIndex}";
        }
    }

    public class TableData : INetworkSerializable
    {
        public int TableNumber;
        public string TableId;
        public Dictionary<string, string> CustomData = new();

        public bool TryGetGameInProgressInfoPanelPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = default;
            rotation = default;
            if (CustomData == null)
            {
                return false;
            }

            if (!CustomData.TryGetValue("GameIsStartedNotificationPanelPosition",
                out string gameIsStartedNotificationPanelPositionString))
            {
                return false;
            }
            if (!CustomData.TryGetValue("GameIsStartedNotificationPanelRotation",
                out string gameIsStartedNotificationPanelRotationString))
            {
                return false;
            }

            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

            string[] coordinatesStrings = gameIsStartedNotificationPanelPositionString.Split("_");
            position.x = float.Parse(coordinatesStrings[0], formatter);
            position.y = float.Parse(coordinatesStrings[1], formatter);
            position.z = float.Parse(coordinatesStrings[2], formatter);

            string[] rotationStrings = gameIsStartedNotificationPanelRotationString.Split("_");
            Vector3 eulerRotation = new();
            eulerRotation.x = float.Parse(rotationStrings[0], formatter);
            eulerRotation.y = float.Parse(rotationStrings[1], formatter);
            eulerRotation.z = float.Parse(rotationStrings[2], formatter);

            rotation = Quaternion.Euler(eulerRotation);

            return true;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref TableNumber);
            serializer.SerializeValue(ref TableId);

            if (serializer.IsWriter)
            {
                int customDataCount = CustomData.Count;
                serializer.SerializeValue(ref customDataCount);

                foreach (var item in CustomData)
                {
                    string key = item.Key;
                    string value = item.Value;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                }
            }
            else
            {
                int customDataCount = 0;
                serializer.SerializeValue(ref customDataCount);

                string key = string.Empty;
                string value = string.Empty;
                CustomData = new();
                for (int i = 0; i < customDataCount; i++)
                {
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                    CustomData.Add(key, value);
                }
            }
        }
    }

    [Serializable]
    public class SimpleClientsList : INetworkSerializable
    {
        public SimpleClientData[] Clients;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int clientsCount = 0;
            if (serializer.IsWriter)
            {
                clientsCount = Clients.Length;
                serializer.SerializeValue(ref clientsCount);
                foreach (SimpleClientData client in Clients)
                {
                    client.NetworkSerialize(serializer);
                }
            }
            else
            {
                serializer.SerializeValue(ref clientsCount);
                Clients = new SimpleClientData[clientsCount];
                for (int i = 0; i < Clients.Length; i++)
                {
                    Clients[i] = new SimpleClientData();
                    Clients[i].NetworkSerialize(serializer);
                }
            }
        }
    }

    [Serializable]
    public class SimpleClientData : INetworkSerializable
    {
        public ulong Id;
        public Guid Guid;
        public string Name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Name);

            string guidString = null;
            if (serializer.IsWriter)
            {
                guidString = Guid.ToString();
                serializer.SerializeValue(ref guidString);
            }
            else
            {
                serializer.SerializeValue(ref guidString);
                Guid = Guid.Parse(guidString);
            }
        }
    }
}
