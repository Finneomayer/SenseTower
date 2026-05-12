using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using static Data.Enumenators;
using Debug = UnityEngine.Debug;

namespace Assets.Blackboard
{
    public class BlackBoard : NetworkBehaviour
    {
        [SerializeField] private ShapeSelectCanvas _shapeSelectCanvas;

        private const float MinPointDistToEraseSqr = 0.03f * 0.03f;

        [HideInInspector] public Texture2D BlackboardTexture;
        [HideInInspector] public bool IsMarkerOwner;
        [SerializeField] private LineRenderer _brushPrefab;
        [field: SerializeField] public Renderer BlackboardRenderTexture { get; private set; }
        public List<PointData> _drawingPoints;
        public List<(ulong, int, Vector3, Color)> _shapePoints;
        public Vector2 TextureSize = new Vector2(64, 64);

        private BlackboardTextureSerializable _blackboardTextureSerializable = new();

        private List<BrushData> _currentBrushes = new();
        private int? _initialBrushIdForLocalUser;

        private int _startPointPart = 0;
        private int _endPointPart = 500;
        private int _pointOffset = 500;

        private Plane _drawingPlane;
        private ShapeType _currentShapeType = ShapeType.Line;
        private Coroutine _clientSnapshotTransmittingRoutine;
        private Guid _blackboardId;

        public bool IsInitialized => _initialBrushIdForLocalUser.HasValue;
        public Plane BlackboardPlane => _drawingPlane;
        public Guid BlackboardId => _blackboardId;

        public event Action OnGetServerScale;
        partial class BrushData
        {
            public ulong ClientId;
            public int BrushId;
            public LineRenderer Brush;
        }

        private void Awake()
        {
            _drawingPoints = new List<PointData>();
            _shapePoints = new();
        }

        public ShapeType GetShapeType()
        {
            return _currentShapeType;
        }

        public void SetShapeType(ShapeType shapeType)
        {
            _currentShapeType = shapeType;
        }

        public int? GetInitialLocalClientBrushId()
        {
            return _initialBrushIdForLocalUser;
        }

        public void ClearAll()
        {
            ClearAllPointsServerRpc();
        }

        public void ClearAllOnServerSide()
        {
            _drawingPoints.Clear();
            ClearAllPointsClientRpc();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;

            if (_shapeSelectCanvas is null)
                _shapeSelectCanvas = FindObjectOfType<ShapeSelectCanvas>();

            RequestBlackboardIdServerRpc(NetworkManager.Singleton.LocalClientId);
            GetBlackboardScaleServerRpc(NetworkManager.Singleton.LocalClientId);
            RequestInitialClientBrushIdServerRpc(NetworkManager.Singleton.LocalClientId);
            GetPartPointsServerRpc(NetworkManager.Singleton.LocalClientId, _startPointPart, _endPointPart);
        }

        public void SetId(Guid blackboardId)
        {
            _blackboardId = blackboardId;
        }

        public void SetDrawingPointsServer(List<PointData> drawingPoints)
        {
            if (drawingPoints != null)
            {
                _drawingPoints = drawingPoints;
            }
        }

        public List<PointData> GetDrawingPointsServer()
        {
            return _drawingPoints.ToList();
        }

        private BrushData InstantiateBrush(PointData pointData)
        {
            LineRenderer _currentBrush = Instantiate(_brushPrefab, transform.position, transform.rotation, transform);
            _currentBrush.transform.Rotate(90, 0, 0);
            _currentBrush.SetPosition(0, pointData.Point);
            _currentBrush.SetPosition(1, pointData.Point);
            _currentBrush.sortingOrder = _currentBrushes.Count + 1;
            BrushData brushData = new BrushData();
            brushData.ClientId = pointData.ClientId;
            brushData.BrushId = pointData.BrushId;
            brushData.Brush = _currentBrush;
            brushData.Brush.startColor = pointData.Color;
            brushData.Brush.endColor = pointData.Color;

            _currentBrushes.Add(brushData);
            return brushData;
        }

        public void AddPoint(ulong clientId, int brushId, Vector3 pointPos, Color color)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (_currentShapeType != ShapeType.Line)
            {
                SaveShapePoint(clientId, brushId, pointPos, color);
                return;
            }

            SendPointToServer(clientId, brushId, pointPos, color);
        }

        private void SendPointToServer(ulong clientId, int brushId, Vector3 pointPos, Color color)
        {
            PointData brushData = new();
            brushData.ClientId = clientId;
            brushData.BrushId = brushId;
            brushData.Point = GetLocalPointOnBlackboard(pointPos);
            brushData.Color = color;

            SetPointOnBlackBoard(brushData);
            SendNewPointServerRpc(brushData);

            _initialBrushIdForLocalUser = brushId + 1;
        }

        private void SaveShapePoint(ulong clientId, int brushId, Vector3 pointPos, Color color)
        {
            //var point = _shapePoints.Find(e => e.Item1 == clientId && e.Item2 == brushId);

            if (_shapePoints.Count == 0)
            {
                SetShape(pointPos, true);
            }
            else
            {
                SetShape(pointPos, false);
            }

            _shapePoints.Add((clientId, brushId, pointPos, color));
        }

        public void CompleteDraw()
        {
            if (_shapePoints.Count > 0)
            {
                Vector3[] ShapePoint = _shapeSelectCanvas.Complete();

                for (int i = 0; i < ShapePoint.Length; i++)
                {
                    var currentPointInfo = _shapePoints[0];
                    var pointOnDashboard =
                        GetPointOnBlackBoard(new Vector3(ShapePoint[i].x, ShapePoint[i].y, ShapePoint[i].z));
                    SendPointToServer(currentPointInfo.Item1, currentPointInfo.Item2, pointOnDashboard,
                        currentPointInfo.Item4);
                }

                _shapePoints.Clear();
            }
        }

        private void SetShape(Vector3 ShapePoint, bool firstPoint)
        {
            _shapeSelectCanvas.SetShapePoint(_currentShapeType, ShapePoint, firstPoint);
        }

        public void Erase(ulong clientId, Vector3 eraserCenter)
        {
            if (!IsInitialized)
            {
                return;
            }

            List<ClientBrushId> brushIdsToErase = GetBrushIdsToErase(eraserCenter);

            SerializableBrushIdList serializableBrushIdList = new() {ClientBrushIds = brushIdsToErase};

            DeleteBrushesServerRpc(clientId, serializableBrushIdList);
            DeleteBrushes(brushIdsToErase);
        }

        public Vector3 GetPointOnBlackBoard(Vector3 point)
        {
            return _drawingPlane.ClosestPointOnPlane(point) + _drawingPlane.normal * 0.00001f;
        }

        public BlackboardSerializableData GetSerializableData()
        {
            BlackboardSerializableData serializableData = new();

            if (_currentBrushes == null)
            {
                return serializableData;
            }

            int maxPositionCount = 0;
            foreach (var brushData in _currentBrushes)
            {
                if (maxPositionCount < brushData.Brush.positionCount)
                {
                    maxPositionCount = brushData.Brush.positionCount;
                }
            }

            if (maxPositionCount == 0)
            {
                return serializableData;
            }

            Vector3[] points = new Vector3[maxPositionCount];

            List<BlackboardLineData> blackboardLines = new();

            foreach (var brushData in _currentBrushes)
            {
                int pointCount = brushData.Brush.GetPositions(points);
                if (pointCount == 0)
                {
                    continue;
                }

                BlackboardLineData data = new();
                data.Color = brushData.Brush.startColor;
                data.Points = new Vector3[pointCount];
                Array.Copy(points, data.Points, pointCount);
                blackboardLines.Add(data);
            }

            serializableData.BrushLinesData = blackboardLines.ToArray();

            return serializableData;
        }

        public void SetData(BlackboardSerializableData data)
        {
            ClearAllPointsServerRpc();

            if (data == null)
            {
                return;
            }

            if (data.BrushLinesData == null || data.BrushLinesData.Length == 0)
            {
                return;
            }

            if (_clientSnapshotTransmittingRoutine != null)
            {
                StopCoroutine(_clientSnapshotTransmittingRoutine);
            }

            _clientSnapshotTransmittingRoutine = StartCoroutine(ClientSnapshotTransmittingRoutine(data));
        }

        private IEnumerator ClientSnapshotTransmittingRoutine(BlackboardSerializableData data)
        {
            WaitForSeconds waitForNextNetworkFrame = new(0.001f * NetworkManager.NetworkTickSystem.TickRate);

            int pointCountTransmittedInNetworkFrame = 0;
            for (int i = 0; i < data.BrushLinesData.Length; i++)
            {
                if (data.BrushLinesData[i].Points == null)
                {
                    continue;
                }

                for (int j = 0; j < data.BrushLinesData[i].Points.Length; j++)
                {
                    if (pointCountTransmittedInNetworkFrame >= _pointOffset)
                    {
                        yield return waitForNextNetworkFrame;
                        pointCountTransmittedInNetworkFrame = 0;
                    }

                    PointData brushData = new();
                    // To not intersect with simultaneous uploads of multiple clients
                    brushData.ClientId = ulong.MaxValue - NetworkManager.LocalClientId;
                    brushData.BrushId = i;
                    brushData.Point = data.BrushLinesData[i].Points[j];
                    brushData.Color = data.BrushLinesData[i].Color;

                    SendNewPointServerRpc(brushData);
                    pointCountTransmittedInNetworkFrame++;
                }
            }

            _clientSnapshotTransmittingRoutine = null;
        }

        private void SetPointOnBlackBoard(PointData pointData)
        {
            var tempbrush =
                _currentBrushes.Find((x) => x.BrushId == pointData.BrushId && x.ClientId == pointData.ClientId);
            if (tempbrush == null)
                tempbrush = InstantiateBrush(pointData);
            else
            {
                tempbrush.Brush.positionCount++;
                int positionIndex = tempbrush.Brush.positionCount - 1;
                tempbrush.Brush.SetPosition(positionIndex, pointData.Point);
            }
        }

        private Vector3 GetLocalPointOnBlackboard(Vector3 worldPoint)
        {
            Vector3 brushPosition = transform.position;
            Quaternion brushRotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
            Vector3 localPoint = Quaternion.Inverse(brushRotation) * (worldPoint - brushPosition);
            return localPoint;
        }

        private void ClearAllPoints()
        {
            for (int i = 0; i < _currentBrushes.Count; i++)
            {
                var tempBrush = _currentBrushes[i];
                Destroy(tempBrush.Brush.gameObject);
            }

            _currentBrushes.Clear();
        }

        private void DeleteBrushes(List<ClientBrushId> brushIds)
        {
            for (int i = _currentBrushes.Count - 1; i >= 0; i--)
            {
                if (brushIds.FirstOrDefault((b) => b.ClientId == _currentBrushes[i].ClientId
                                                   && b.BrushId == _currentBrushes[i].BrushId) != null)
                {
                    Destroy(_currentBrushes[i].Brush.gameObject);
                    _currentBrushes.RemoveAt(i);
                }
            }
        }

        private void DeletePointsServer(List<ClientBrushId> brushIds)
        {
            for (int i = _drawingPoints.Count - 1; i >= 0; i--)
            {
                if (brushIds.FirstOrDefault((b) => b.ClientId == _drawingPoints[i].ClientId
                                                   && b.BrushId == _drawingPoints[i].BrushId) != null)
                {
                    _drawingPoints.RemoveAt(i);
                }
            }
        }

        private List<ClientBrushId> GetBrushIdsToErase(Vector3 worldEraserCenter)
        {
            List<ClientBrushId> brushIdsToErase = new();
            int maxPositionCount = 0;
            foreach (var brushData in _currentBrushes)
            {
                if (maxPositionCount < brushData.Brush.positionCount)
                {
                    maxPositionCount = brushData.Brush.positionCount;
                }
            }

            Vector3 eraserCenter = GetLocalPointOnBlackboard(worldEraserCenter);
            Vector3[] tempPointPosArray = new Vector3[maxPositionCount];
            foreach (var brushData in _currentBrushes)
            {
                brushData.Brush.GetPositions(tempPointPosArray);

                if (brushData.Brush.positionCount == 0)
                {
                    continue;
                }

                if (brushData.Brush.positionCount == 1)
                {
                    if (Vector3.SqrMagnitude(tempPointPosArray[0] - eraserCenter) < MinPointDistToEraseSqr)
                    {
                        brushIdsToErase.Add(new ClientBrushId(brushData.ClientId, brushData.BrushId));
                    }

                    continue;
                }

                for (int i = 0; i < brushData.Brush.positionCount - 1; i++)
                {                  
                    Vector3 point1 = tempPointPosArray[i];
                    Vector3 point2 = tempPointPosArray[i + 1];

                    Vector3 v = point2 - point1;
                    Vector3 w1 = eraserCenter - point1;
                    Vector3 w2 = eraserCenter - point2;

                    if (Vector3.Dot(w1, v) <= 0)
                    {
                        if (w1.sqrMagnitude < MinPointDistToEraseSqr)
                        {
                            brushIdsToErase.Add(new ClientBrushId(brushData.ClientId, brushData.BrushId));
                            ;
                            break;
                        }

                        continue;
                    }

                    if (Vector3.Dot(w2, v) >= 0)
                    {
                        if (w2.sqrMagnitude < MinPointDistToEraseSqr)
                        {
                            brushIdsToErase.Add(new ClientBrushId(brushData.ClientId, brushData.BrushId));
                            break;
                        }

                        continue;
                    }

                    Vector3 segmentDirection = v.normalized;
                    Vector3 eraserCenterProjection = point1 + Vector3.Dot(w1, segmentDirection) * segmentDirection;

                    if (Vector3.SqrMagnitude(eraserCenterProjection - eraserCenter) < MinPointDistToEraseSqr)
                    {
                        brushIdsToErase.Add(new ClientBrushId(brushData.ClientId, brushData.BrushId));
                        break;
                    }
                }
            }

            return brushIdsToErase;
        }

        #region ClientPart

        [ClientRpc]
        public void SendBlackboardIdClientRpc(string blackboardId, ClientRpcParams clientRpcParams = default)
        {
            _blackboardId = new Guid(blackboardId);
        }

        [ClientRpc]
        private void SetNewPointClientRpc(PointData pointData, ClientRpcParams clientRpcParams = default)
        {
            if (pointData.ClientId == NetworkManager.LocalClientId)
            {
                return;
            }

            SetPointOnBlackBoard(pointData);
        }

        [ClientRpc]
        private void SendPartPackageClientRpc(PointData pointData, int currentPoint, int pointCount,
            ClientRpcParams clientRpcParams = default)
        {
            SetPointOnBlackBoard(pointData);

            if (currentPoint + 1 == _endPointPart)
            {
                _startPointPart = _endPointPart;
                _endPointPart = _endPointPart + _pointOffset;
                if (_endPointPart > pointCount)
                    _endPointPart = pointCount;

                GetPartPointsServerRpc(NetworkManager.Singleton.LocalClientId, _startPointPart, _endPointPart);
            }
        }

        [ClientRpc]
        private void SendInitialClientBrushIdClientRpc(int initialBrushId, ClientRpcParams clientRpcParams = default)
        {
            _initialBrushIdForLocalUser = initialBrushId;
        }

        [ClientRpc]
        private void ClearAllPointsClientRpc()
        {
            ClearAllPoints();
        }

        [ClientRpc]
        private void DeleteBrushesClientRpc(ulong clientId, SerializableBrushIdList brushIds,
            ClientRpcParams clientRpcParams = default)
        {
            if (NetworkManager.LocalClientId == clientId)
            {
                return;
            }

            DeleteBrushes(brushIds.ClientBrushIds);
        }

        #endregion

        #region ServerPart

        [ServerRpc(RequireOwnership = false)]
        public void RequestBlackboardIdServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { clientId };

            SendBlackboardIdClientRpc(_blackboardId.ToString(), clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetBlackboardScaleServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] {clientId};

            SetBlackboardScaleClientRpc(BlackboardRenderTexture.transform.localScale, clientRpcParams);
        }

        [ClientRpc]
        private void SetBlackboardScaleClientRpc(Vector3 blackboardScale, ClientRpcParams rpcParams = default)
        {
            BlackboardRenderTexture.transform.localScale = blackboardScale;
            
            BlackboardTexture = new Texture2D((int) TextureSize.x, (int) TextureSize.y);
            BlackboardRenderTexture.material.mainTexture = BlackboardTexture;

            _drawingPlane = new Plane(transform.up, transform.position);

            OnGetServerScale?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendNewPointServerRpc(PointData pointData)
        {
            _drawingPoints.Add(pointData);
            SetNewPointClientRpc(pointData);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetPartPointsServerRpc(ulong clientId, int startPoint, int endPoint)
        {
            if (_drawingPoints.Count == 0) return;
            if (_drawingPoints.Count < endPoint) endPoint = _drawingPoints.Count;

            ClientRpcParams clientRpcParams = new ClientRpcParams();
            clientRpcParams.Send.TargetClientIds = new ulong[1] {clientId};

            for (int i = startPoint; i < endPoint; i++)
            {
                var tempitem = _drawingPoints[i];
                SendPartPackageClientRpc(tempitem, i, _drawingPoints.Count, clientRpcParams);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestInitialClientBrushIdServerRpc(ulong clientId)
        {
            int? lastClientBrushId = null;
            foreach (var item in _drawingPoints)
            {
                if (item.ClientId == clientId)
                {
                    if (!lastClientBrushId.HasValue || item.BrushId > lastClientBrushId)
                    {
                        lastClientBrushId = item.BrushId;
                    }
                }
            }

            int initialClientBrushId = lastClientBrushId.HasValue ? lastClientBrushId.Value + 1 : 0;

            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new ulong[1] {clientId};

            SendInitialClientBrushIdClientRpc(initialClientBrushId, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClearAllPointsServerRpc()
        {
            _drawingPoints.Clear();
            ClearAllPointsClientRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DeleteBrushesServerRpc(ulong clientId, SerializableBrushIdList brushIdList)
        {
            DeletePointsServer(brushIdList.ClientBrushIds);
            DeleteBrushesClientRpc(clientId, brushIdList);
        }

        #endregion

        #region Deprecated

        private void SetNewPoint(NetworkListEvent<PointData> changeEvent)
        {
            SetPointOnBlackBoard(changeEvent.Value);
        }

        public void SendTextureToServer()
        {
            _blackboardTextureSerializable.NetworkBlackboard.TexturePicture = BlackboardTexture;
            _blackboardTextureSerializable.NetworkBlackboard.Width = 2;
            _blackboardTextureSerializable.NetworkBlackboard.Height = 1;
            TransferBlackboardDataServerRpc(_blackboardTextureSerializable);
        }

        [ServerRpc(RequireOwnership = false)] //, Delivery = RpcDelivery.Unreliable
        private void TransferBlackboardDataServerRpc(BlackboardTextureSerializable blackboardTextureSerializable)
        {
            DataClientRpc(blackboardTextureSerializable);
        }

        [ClientRpc]
        private void DataClientRpc(BlackboardTextureSerializable blackboardTextureSerializable)
        {
            BlackboardRenderTexture.material.mainTexture = blackboardTextureSerializable.NetworkBlackboard.TexturePicture;
            BlackboardTexture = blackboardTextureSerializable.NetworkBlackboard.TexturePicture;
            BlackboardRenderTexture.material.mainTexture = BlackboardTexture;
        }

        #endregion
    }

    public struct PointData : INetworkSerializable, IEquatable<PointData>
    {
        public ulong ClientId;
        public int BrushId;
        public Vector3 Point;
        public Color Color;

        public bool Equals(PointData other)
        {
            return ClientId == other.ClientId && BrushId == other.BrushId && Point == other.Point;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref BrushId);
            serializer.SerializeValue(ref Point);
            serializer.SerializeValue(ref Color);
        }
    }

    public class ClientBrushId
    {
        public ulong ClientId { get; }
        public int BrushId { get; }

        public ClientBrushId(ulong clientId, int brushId)
        {
            ClientId = clientId;
            BrushId = brushId;
        }
    }

    public struct SerializableBrushIdList : INetworkSerializable
    {
        public List<ClientBrushId> ClientBrushIds;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                int count = 0;
                if (ClientBrushIds == null)
                {
                    serializer.SerializeValue(ref count);
                    return;
                }

                count = ClientBrushIds.Count;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    ulong clientId = ClientBrushIds[i].ClientId;
                    int brushId = ClientBrushIds[i].BrushId;
                    serializer.SerializeValue(ref clientId);
                    serializer.SerializeValue(ref brushId);
                }
            }
            else
            {
                ClientBrushIds = new();
                int count = 0;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    ulong clientId = 0;
                    int brushId = 0;
                    serializer.SerializeValue(ref clientId);
                    serializer.SerializeValue(ref brushId);
                    ClientBrushIds.Add(new ClientBrushId(clientId, brushId));
                }
            }
        }
    }
}