using UnityEngine;
using UI;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Shared;
using Assets.Scripts.Space;

namespace Assets.Blackboard
{
    public class BlackBoardControlPanel : ViewPanel
    {
        [SerializeField] private ButtonUI BrushColorButton;
        [SerializeField] private ButtonUI SaveSnapshotButton;
        [SerializeField] private ButtonUI DeleteBlackBoardContentButton;
        [SerializeField] private ButtonUI EraserButton;
        [SerializeField] private ButtonUI ShapeSelectButton;
        [SerializeField] private ButtonUI LoadDataButton;

        [SerializeField] private BrushColorPanel BrushColorPanel;
        [SerializeField] private ShapePanel ShapePanel;
        [SerializeField] private SnapshotPanel SnapshotPanel;
        [SerializeField] private DeleteBlackBoardContentPanel DeleteBlackBoardContentPanel;
        [SerializeField] private EraserPanel EraserPanel;
        [SerializeField] private LoadDataPanel LoadDataPanel;

        [SerializeField] private BlackboardContentSnapshotMaker SnapshotMaker;
        [SerializeField] private BlackBoardZone BlackBoardZone;
        [SerializeField] private Transform PlayerEyesPoint;

        private Plane _blackboardPlane;
        private BoxCollider _collider;
        private Transform _playerCameraTransform;
        private Coroutine _followingPlayerRoutine;
        private Dictionary<ViewPanel, ButtonUI> _panelsButtonsMap = new();

        private Transform _balckboardZoneTransform;
        private Vector3 _newPositionControlPanel;
        public float _shift;
        private bool _isFirstFrame = true;

        private ISpaceManager _spaceManager;

        private void Awake()
        {
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _spaceManager = commonDIInstaller.Resolve<ISpaceManager>();
            }

            _panelsButtonsMap[BrushColorPanel] = BrushColorButton;
            _panelsButtonsMap[SnapshotPanel] = SaveSnapshotButton;
            _panelsButtonsMap[ShapePanel] = ShapeSelectButton;
            _panelsButtonsMap[DeleteBlackBoardContentPanel] = DeleteBlackBoardContentButton;
            _panelsButtonsMap[EraserPanel] = EraserButton;
            _panelsButtonsMap[LoadDataPanel] = LoadDataButton;

            HidePanel();
        }

        private void OnEnable()
        {
            BlackBoardZone.BlackBoard.OnGetServerScale += OnBlackBoardGetScale;
            BlackBoardZone.LocalPlayerEnteredZone += OnLocalPlayerEnteredBlackBoardZone;
            BlackBoardZone.LocalPlayerLeftZone += OnLocalPlayerLeftBlackBoardZone;

            BrushColorButton.InteractElement.onClick.AddListener(ToggleBrushColorPanel);
            ShapeSelectButton.InteractElement.onClick.AddListener(ToggleShapePanel);
            SaveSnapshotButton.InteractElement.onClick.AddListener(SaveSnapshot);
            DeleteBlackBoardContentButton.InteractElement.onClick.AddListener(ToggleDeleteAllPanel);
            EraserButton.InteractElement.onClick.AddListener(OnEraserButtonClick);
            LoadDataButton.InteractElement.onClick.AddListener(ToggleLoadDataPanel);

            DeleteBlackBoardContentPanel.DeleteButtonClicked += OnDeleteAllContentButtonClick;
            DeleteBlackBoardContentPanel.CancelButtonClicked += OnCancelDeleteAllContentButtonClick;

            SnapshotMaker.SavedToFile += OnSnapshotSavedToFile;
        }

        private void OnBlackBoardGetScale()
        {
            _blackboardPlane = BlackBoardZone.BlackBoard.BlackboardPlane;
            _collider = BlackBoardZone.GetComponent<BoxCollider>();
            _balckboardZoneTransform = BlackBoardZone.transform;

            BrushColorPanel.Init(BlackBoardZone);
            ShapePanel.Init(BlackBoardZone);
            SnapshotMaker.Init(BlackBoardZone.BlackBoard, _spaceManager);
            LoadDataPanel.Init(BlackBoardZone.BlackBoard, _spaceManager);
        }

        private void OnDisable()
        {
            BlackBoardZone.BlackBoard.OnGetServerScale -= OnBlackBoardGetScale;

            BlackBoardZone.LocalPlayerEnteredZone -= OnLocalPlayerEnteredBlackBoardZone;
            BlackBoardZone.LocalPlayerLeftZone -= OnLocalPlayerLeftBlackBoardZone;

            BrushColorButton.InteractElement.onClick.RemoveListener(ToggleBrushColorPanel);
            ShapeSelectButton.InteractElement.onClick.RemoveListener(ToggleShapePanel);
            SaveSnapshotButton.InteractElement.onClick.RemoveListener(SaveSnapshot);
            DeleteBlackBoardContentButton.InteractElement.onClick.RemoveListener(ToggleDeleteAllPanel);

            DeleteBlackBoardContentPanel.DeleteButtonClicked -= OnDeleteAllContentButtonClick;
            DeleteBlackBoardContentPanel.CancelButtonClicked -= OnCancelDeleteAllContentButtonClick;

            SnapshotMaker.SavedToFile -= OnSnapshotSavedToFile;

            _followingPlayerRoutine = null;
        }

        public override void ShowPanel()
        {
            if (_playerCameraTransform == null)
            {
                return;
            }

            if (_followingPlayerRoutine != null)
            {
                StopCoroutine(_followingPlayerRoutine);
            }

            _followingPlayerRoutine = StartCoroutine(FollowingPlayerRoutine());

            base.ShowPanel();
        }

        public override void HidePanel()
        {
            HideAllPanels();

            if (_followingPlayerRoutine != null)
            {
                StopCoroutine(_followingPlayerRoutine);
                _followingPlayerRoutine = null;
            }

            base.HidePanel();
        }

        private void OnLocalPlayerEnteredBlackBoardZone(PlayerLogic player)
        {
            _playerCameraTransform = player.CameraTransform;
            ShowPanel();
        }

        private void OnLocalPlayerLeftBlackBoardZone(PlayerLogic player)
        {
            HidePanel();
        }

        private void OnSnapshotSavedToFile(string pathToSnapshot)
        {
            SnapshotPanel.ShowPanel(pathToSnapshot);
        }

        private void OnDeleteAllContentButtonClick()
        {
            BlackBoardZone.BlackBoard.ClearAll();
            ToggleDeleteAllPanel();
        }

        private void OnCancelDeleteAllContentButtonClick()
        {
            ToggleDeleteAllPanel();
        }

        private void OnEraserButtonClick()
        {
            ToggleViewPanel(EraserPanel);
        }

        private void ToggleBrushColorPanel()
        {
            ToggleViewPanel(BrushColorPanel);
        }

        private void ToggleShapePanel()
        {
            ToggleViewPanel(ShapePanel);
        }

        private void ToggleDeleteAllPanel()
        {
            ToggleViewPanel(DeleteBlackBoardContentPanel);
        }

        private void ToggleLoadDataPanel()
        {
            ToggleViewPanel(LoadDataPanel);
        }

        private void ToggleViewPanel(ViewPanel viewPanel)
        {
            bool newVisible = !viewPanel.IsVisible();

            HideAllPanels();

            if (newVisible)
            {
                viewPanel.ShowPanel();
            }
            else
            {
                viewPanel.HidePanel();
            }

            if (_panelsButtonsMap.TryGetValue(viewPanel, out ButtonUI button))
            {
                button.SetButtonActive(newVisible);
            }

            SetActiveEraser(EraserPanel.IsVisible());
        }

        private void SetActiveEraser(bool active)
        {
            BlackBoardZone.SetActiveEraser(active);
        }

        private void HideAllPanels()
        {
            foreach (var panel in _panelsButtonsMap)
            {
                panel.Key.HidePanel();
                if (panel.Value != null)
                {
                    panel.Value.SetButtonActive(false);
                }
            }
        }

        private void SaveSnapshot()
        {
            StartCoroutine(SaveSnapshotRoutine());
        }

        private IEnumerator SaveSnapshotRoutine()
        {
            HideAllPanels();
            SaveSnapshotButton.SetButtonActive(true);

            yield return null;

            SnapshotMaker.SaveSnapshot();
            SaveSnapshotButton.SetButtonActive(false);
        }

        private IEnumerator FollowingPlayerRoutine()
        {
            while (true)
            {
                SetCorrectedPanelPosition(_blackboardPlane, _playerCameraTransform.position);
                yield return null;
            }
        }

        private void SetCorrectedPanelPosition(Plane blackboardPlane, Vector3 playerCameraPosition)
        {
            if (_collider != null)
            {
                var leftButtonShift = transform.position - BrushColorButton.transform.position;
                var rightButtonShift = transform.position - LoadDataButton.transform.position;

                _newPositionControlPanel = blackboardPlane.ClosestPointOnPlane(
                                               playerCameraPosition + transform.position - PlayerEyesPoint.position) +
                                           blackboardPlane.normal * 0.05f;

                if (!_isFirstFrame && !_collider.bounds.Contains(_newPositionControlPanel))
                    return;

                if (!_isFirstFrame && !_collider.bounds.Contains(_newPositionControlPanel + leftButtonShift)
                    || !_isFirstFrame &&
                    !_collider.bounds.Contains(_newPositionControlPanel + rightButtonShift)) return;

                transform.position = _newPositionControlPanel;

                _isFirstFrame = false;
            }
        }
    }
}