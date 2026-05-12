using Assets.Mechanics.Physics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Blackboard
{
    public class BlackBoardEraser: MonoBehaviour
    {
        private const float MinLinePointSqrDist = 0.003f * 0.003f;

        [SerializeField] private LayerMask _blackBoardLayerMask;
        [SerializeField] private Transform _tip;
        [SerializeField] private Collider _collider;

        private BlackBoard _blackBoard;

        private float _tipHeight;
        private RaycastHit _touch;
        private bool _isDrawing = false;

        private Vector3 _currentPos = Vector3.zero;

        private XRBaseControllerInteractor _playerControllerInteractor;
        private RaycastNonAllocMaker _raycastMaker;

        public bool IsDrawing => _isDrawing;
        public XRBaseControllerInteractor PlayerControllerInteractor => _playerControllerInteractor;

        private void Start()
        {
            _tipHeight = 0.5f * _tip.lossyScale.y;
            _raycastMaker = new();
        }

        private void Update()
        {
            _raycastMaker.Raycast(_tip.position, transform.up, _tipHeight, _blackBoardLayerMask);

            _isDrawing = false;

            for (int i = 0; i < _raycastMaker.LastHitCount; i++)
            {
                if (_raycastMaker.LastHits[i].transform.CompareTag("Blackboard"))
                {
                    _isDrawing = true;
                    _touch = _raycastMaker.LastHits[i];
                    break;
                }
            }

            if (_isDrawing)
            {
                Erase(_blackBoard.GetPointOnBlackBoard(_touch.point));
            }
        }

        public void Init(BlackBoard blackBoard, XRBaseControllerInteractor playerControllerInteractor)
        {
            _blackBoard = blackBoard;
            _playerControllerInteractor = playerControllerInteractor;
        }

        private void Erase(Vector3 pointPos)
        {
            if (Vector3.SqrMagnitude(_currentPos - pointPos) > MinLinePointSqrDist)
            {
                _currentPos = pointPos;
                _blackBoard.Erase(NetworkManager.Singleton.LocalClientId, _currentPos);
            }
        }
    }
}
