using Assets.Mechanics.Physics;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Blackboard
{
    public class BlackBoardMarker : MonoBehaviour
    {
        private const float MinLinePointSqrDist = 0.003f * 0.003f;

        [SerializeField] private LayerMask _blackBoardLayerMask;
        [SerializeField] private GameObject _markerContent;
        [SerializeField] private Transform _tip;
        [SerializeField] private Collider _collider;
        [SerializeField] private int _penSize = 5;
        [SerializeField] [ColorUsage(true, true)] private Color32 _color;
        
        private BlackBoard _blackBoard;

        private Color32[] _colors;
        private float _tipHeight;
        private RaycastHit _touch;
        private Vector2 _touchPos, _lastTouchPos;
        private bool _touchedLastFrame;
        private Quaternion _lastTouchRot;
        private bool _isDrawing = false;
        private Vector3 _localPosition;

        private Vector3 _currentPos = Vector3.zero;
        private int _brushId = 0;
        private int _previousPointBrushId = -1;

        private PlayerController _playerController;
        private MarkerMovement _markerMovement;
        private RaycastNonAllocMaker _raycastMaker;

        public bool IsDrawing => _isDrawing;
        public PlayerController PlayerController => _playerController;
        public Transform Tip => _tip;

        private void OnEnable()
        {
            RegisterEventsListeners();
        }

        private void OnDisable()
        {
            UnregisterEventsListeners();
        }

        private void Start()
        {
            _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
            _tipHeight = 0.5f * _tip.lossyScale.y;
            _localPosition = transform.localPosition;
            _raycastMaker = new();
        }

        private void Update()
        {
            if (_markerMovement == null)
            {
                return;
            }

            _markerMovement.RefreshPosition();

            DrawExperimental();
            if (_isDrawing)
            {
                AddPoint(_blackBoard.GetPointOnBlackBoard(_touch.point));
            }
            else 
            {
                CompleteDraw();
            }
        }

        public void Init(BlackBoard blackBoard, PlayerController controller)
        {
            _blackBoard = blackBoard;
            _playerController = controller;

            int? initialBrushId = _blackBoard.GetInitialLocalClientBrushId();
            if (initialBrushId.HasValue)
            {
                _brushId = initialBrushId.Value;
            }

            //AddDebugPoints();

            _markerMovement = new MarkerMovement();
            _markerMovement.Init(this, controller.MarkerAnchor, blackBoard);

            RegisterEventsListeners();
        }

        private void OnControllerEnabled(PlayerController controller)
        {
            _markerContent.SetActive(true);
        }

        private void OnControllerDisabled(PlayerController controller)
        {
            _markerContent.SetActive(false);
        }

        private void RegisterEventsListeners()
        {
            UnregisterEventsListeners();
            if (_playerController == null)
            {
                return;
            }
            _playerController.Enabled += OnControllerEnabled;
            _playerController.Disabled += OnControllerDisabled;
        }

        private void UnregisterEventsListeners()
        {
            if (_playerController == null)
            {
                return;
            }
            _playerController.Enabled -= OnControllerEnabled;
            _playerController.Disabled -= OnControllerDisabled;
        }

        private void AddDebugPoints()
        {
            List<Color> colors = new();
            colors.Add(Color.red);
            colors.Add(Color.green);
            colors.Add(Color.blue);
            colors.Add(Color.white);

            float xMin = 0.8f;
            float yMin = 0;
            float x = xMin;
            float y = yMin;
            for (int i = 0; i < 1000; i++)
            {
                x += 0.03f;
                if (x > 4.5)
                {
                    x = xMin;
                    yMin = y;
                }
                SetColor(colors[Random.Range(0, colors.Count)]);

                //Vector3 center = new Vector3(Random.Range(0.8f, 5), Random.Range(0, 6.4f), -0.61999f);
                y = yMin;
                for (int j = 0; j < 100; j++)
                {
                    y += 0.004f;
                    //AddPoint(center + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0));
                    AddPoint(new Vector3(x, y, -0.61999f));
                }
                //AddPoint(new Vector3(5, 0, -0.61999f));
                //AddPoint(new Vector3(4, 3, -0.61999f));
                //AddPoint(new Vector3(2, 1, -0.61999f));
                _brushId++;
            }
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public Color GetColor()
        {
            return _color;
        }

        private void DrawExperimental() 
        {
            _raycastMaker.Raycast(_tip.position - 0.5f * _tipHeight * transform.up, transform.up,
                0.005f + 0.5f * _tipHeight, _blackBoardLayerMask);
            
            bool isTouchingBlackboard = false;
            
            for (int i = 0; i < _raycastMaker.LastHitCount; i++)
            {
                if (_raycastMaker.LastHits[i].transform.CompareTag("Blackboard"))
                {
                    isTouchingBlackboard = true;
                    _touch = _raycastMaker.LastHits[i];
                    break;
                }
            }

            if (isTouchingBlackboard)
            {
                _isDrawing = true;
            }
            else
            {
                if (_isDrawing)
                {
                    _brushId++;
                    _isDrawing = false;
                }
            }
        }

        private void Draw()
        {
            if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
            {
                if (_touch.transform.CompareTag("Blackboard"))
                {
                    Debug.Log("touch BlackBoard");
                    _isDrawing = true;
                    
                    _blackBoard.IsMarkerOwner = true;

                    _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                    var x = (int)(_touchPos.x * _blackBoard.TextureSize.x - (_penSize / 2));
                    var y = (int)(_touchPos.y * _blackBoard.TextureSize.y - (_penSize / 2));

                    if (y < 0 || y > _blackBoard.TextureSize.y || x < 0 || x > _blackBoard.TextureSize.x) return;

                    if (_touchedLastFrame)
                    {
                        _blackBoard.BlackboardTexture.SetPixels32(x, y, _penSize, _penSize, _colors);

                        for (float f = 0; f < 1.00f; f += 0.1f)
                        {
                            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                            _blackBoard.BlackboardTexture.SetPixels32(lerpX, lerpY, _penSize, _penSize, _colors);
                        }

                        //transform.rotation = _lastTouchRot;
                        _blackBoard.BlackboardTexture.Apply();
                    }

                    _lastTouchPos = new Vector2(x, y);
                    //_lastTouchRot = transform.rotation;
                    _touchedLastFrame = true;
                    return;
                }
            }
            if (_isDrawing) _blackBoard.SendTextureToServer(); //sending data when detached from the board
            transform.localPosition = _localPosition;
            _isDrawing = false;
            _touchedLastFrame = false;
        }

        public void AddPoint(Vector3 pointPos)
        {
            if (_previousPointBrushId != _brushId || Vector3.SqrMagnitude(_currentPos - pointPos) > MinLinePointSqrDist)
            {
                _currentPos = pointPos;
                _blackBoard.AddPoint(NetworkManager.Singleton.LocalClientId, _brushId, _currentPos, _color);

                _previousPointBrushId = _brushId;
            }
        }

        private void CompleteDraw()
        {
            _blackBoard.CompleteDraw();
        }

    }
}
