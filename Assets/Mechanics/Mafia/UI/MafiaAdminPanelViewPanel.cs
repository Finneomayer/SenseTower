using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Mafia.Table;
using Assets.Scripts.Server;
using Assets.UI.Pad;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Mafia.UI
{
    public class MafiaAdminPanelViewPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private ViewPanel _startViewPanel;
        [SerializeField] private List<ViewPanelAndActivateButton> _viewPanels;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _nextStageButton;
        [SerializeField] private Button _endButton;
        [SerializeField] private Button _smallSize;
        [SerializeField] private Button _middleSize;
        [SerializeField] private Button _bigSize;
        [SerializeField] private VisitorAdminViewElement _participantPrefab;
        [SerializeField] private Transform _participantParent;
        [SerializeField] private MafiaPresetsPanel _presetsPanel;

        public Action<int> SizeClicked;
        public event Action<ulong> AdminChangeRequested;

        #endregion

        private MafiaEventMediatorClient _mafiaEventMediator;
        private List<VisitorAdminViewElement> _participantViews = new();
        private List<SimpleClientData> _currentOccupatingClients = new();
        private GameState _currentGameState;
        private MafiaLocalizationResultDto _mafiaLocalizationData;

        protected override void Awake()
        {
            base.Awake();
            ShowPanel();
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _nextStageButton.onClick.AddListener(OnNextStageButtonClick);
            _endButton.onClick.AddListener(OnEndButtonClick);
            
            _smallSize.onClick.AddListener(() => OnSizeClicked(0));
            _middleSize.onClick.AddListener(() => OnSizeClicked(1));
            _bigSize.onClick.AddListener(() => OnSizeClicked(2));

            _presetsPanel.OnUpdatePreset += _presetsPanel_OnUpdatePreset;

            foreach (ViewPanelAndActivateButton viewPanel in _viewPanels)
            {
                viewPanel.ActivateButton.onClick.AddListener(() => OnViewActivate(viewPanel.ViewPanel));
            }
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClick);
            _nextStageButton.onClick.RemoveListener(OnNextStageButtonClick);
            _endButton.onClick.RemoveListener(OnEndButtonClick);
            _bigSize.onClick.RemoveAllListeners();
            _middleSize.onClick.RemoveAllListeners();
            _smallSize.onClick.RemoveAllListeners();

            _presetsPanel.OnUpdatePreset -= _presetsPanel_OnUpdatePreset;

            foreach (ViewPanelAndActivateButton viewPanel in _viewPanels)
            {
                viewPanel.ActivateButton.onClick.RemoveListener(() => OnViewActivate(viewPanel.ViewPanel));
            }
        }

        public void Init(MafiaEventMediatorClient mafiaEventMediator, MafiaLocalizationResultDto mafiaLocalizationData)
        {
            _mafiaEventMediator = mafiaEventMediator;
            _mafiaLocalizationData = mafiaLocalizationData;
            SetNotEnoughPlayers();
        }

        private void _presetsPanel_OnUpdatePreset(MafiaPlayerRole[] preset)
        {
            _mafiaEventMediator.UpdateRolePreset(preset);
        }

        private async UniTask TemporaryDisableNextButton()
        {
            _nextStageButton.gameObject.SetActive(false);
            await UniTask.Delay(1000);
            _nextStageButton.gameObject.SetActive(true);
        }

        public void SetGameState(GameState gameState)
        {
            _currentGameState = gameState;

            if (gameState == null)
            {
                SetInitialState();
            }
            else
            {
                if (gameState.GameStatus == MafiaGameStatus.InProgress)
                {
                    _mafiaLocalizationData.StageNames.TryGetValue(gameState.GameStage, out string stageString);
                    _statusText.text = stageString;
                    _startButton.gameObject.SetActive(false);
                    TemporaryDisableNextButton().Forget();
                    _endButton.gameObject.SetActive(true);
                }
                else
                {
                    _statusText.text = $"Поздравляем!\n{gameState.GameStatus}";
                    _endButton.gameObject.SetActive(true);
                }
            }

            RefreshParticipants();
        }

        public void SetInitialState()
        {
            _statusText.text = "Ждём начала игры";
            _nextStageButton.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(true);
            _endButton.gameObject.SetActive(false);
        }

        public void SetNotEnoughPlayers()
        {
            _statusText.text = "Недостаточно игроков";
            _nextStageButton.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(false);
            _endButton.gameObject.SetActive(false);
        }

        private void AdminChangeClicked(ulong newAdminId)
        {
            AdminChangeRequested?.Invoke(newAdminId);
        }

        public void ShowPreset(MafiaPlayerRole[] roles)
        {
            _presetsPanel.ShowPreset(roles);
        }

        /// <summary>
        ///before the game started
        /// </summary>
        /// <param name="playerStates"></param>
        public void SetParticipants(List<SimpleClientData> occupatingClients)
        {
            _currentOccupatingClients = occupatingClients;
            RefreshParticipants();
        }

        public void RefreshParticipants()
        {
            if (_currentGameState != null)
            {
                //if (_participantViews.Count != _currentGameState.PlayerStates.Length)
                //{
                FillParticipantsByPlayers(_currentGameState.PlayerStates);

                List<MafiaPlayerRole> currentPreset = new();
                foreach (var playerState in _currentGameState.PlayerStates)
                {
                    if (playerState.Role != MafiaPlayerRole.GameMaster)
                    {
                        currentPreset.Add(playerState.Role);
                    }
                }
                ShowPreset(currentPreset.ToArray());
                //}
                return;
            }

            bool needUpdatePresets = _currentOccupatingClients.Count != _participantViews.Count;

            FillParticipantsByClients(_currentOccupatingClients);

            if (_currentOccupatingClients.Count < 3)
            {
                SetNotEnoughPlayers();
            }
            else if (needUpdatePresets)
            {
                SetInitialState();
                _mafiaEventMediator.RequestPlayerRolePreset(_currentOccupatingClients.Count);
            }
        }

        private void FillParticipantsByClients(List<SimpleClientData> occupatingClients)
        {
            ClearParticipants();
            foreach (var player in occupatingClients)
            {
                var participantView = Instantiate(_participantPrefab, _participantParent);
                participantView.SetName(player.Name);
                participantView.SetGuid(player.Guid);
                participantView.SetText(player.Name);
                participantView.SetActiveAdminButton(true);
                participantView.SetId(player.Id);
                participantView.AdminButtonClicked += AdminChangeClicked;
                participantView.KickButtonClicked += OnKickButtonClicked;
                _participantViews.Add(participantView);
            }
        }

        private void FillParticipantsByPlayers(PlayerState[] playerStates)
        {
            ClearParticipants();
            foreach (var player in playerStates)
            {
                if (player.Role == MafiaPlayerRole.GameMaster)
                {
                    continue;
                }
                var participantView = Instantiate(_participantPrefab, _participantParent);
                participantView.SetName(player.PlayerName);
                participantView.SetGuid(player.PlayerId);
                participantView.SetText($"№{player.Number}. {player.PlayerName}");
                participantView.SetActiveAdminButton(false);
                participantView.AdminButtonClicked += AdminChangeClicked;
                participantView.KickButtonClicked += OnKickButtonClicked;
                _participantViews.Add(participantView);
            }
        }

        private void ClearParticipants()
        {           
            foreach (var participant in _participantViews)
            {
                participant.AdminButtonClicked -= AdminChangeClicked;
                participant.KickButtonClicked -= OnKickButtonClicked;
                Destroy(participant.gameObject);
            }
            _participantViews.Clear();
        }

        private void OnKickButtonClicked(ulong cliendId, Guid guid)
        {
            _mafiaEventMediator.RequestKickUser(guid);
        }

        private void OnSizeClicked(int size)
        {
            SizeClicked?.Invoke(size);
        }

        private void Start()
        {
            OnViewActivate(_startViewPanel);
        }

        private void OnStartButtonClick()
        {
            _mafiaEventMediator.RequestStartGame();
            _startButton.gameObject.SetActive(false);
            //_mafiaGameControllerService.SetAdmin(NetworkManager.Singleton.LocalClientId);
            //_mafiaGameControllerService.StartGame();
            ////_statusText.text = "Старт Игры";
            //_statusText.text = "Обсуждение";
        }

        private void OnNextStageButtonClick()
        {
            _mafiaEventMediator.RequestNextGameStage();
            _nextStageButton.gameObject.SetActive(false);
            //_mafiaGameControllerService.NextGameStage();
            //if (_statusText.text == "Обсуждение")
            //{
            //    _statusText.text = "Голосование";
            //}
            //else
            //{
            //    _statusText.text = "Обсуждение";
            //}
            //_statusText.text = "Ходит мафия";
        }

        private void OnEndButtonClick()
        {
            _statusText.text = "Ждём завершения игры";
            _nextStageButton.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(false);
            _endButton.gameObject.SetActive(false);
            
            _mafiaEventMediator.RequestCompleteGame();
            //SetGameReadyView();
            //_statusText.text = "Игра закончена";
            //_mafiaGameControllerService.CompleteGame();
        }

        private void OnViewActivate(ViewPanel viewPanel)
        {
            foreach (ViewPanelAndActivateButton viewPanelAndActivateButton in _viewPanels)
            {
                if (viewPanel == viewPanelAndActivateButton.ViewPanel)
                {
                    viewPanelAndActivateButton.ViewPanel.ShowPanel();
                }
                else
                {
                    viewPanelAndActivateButton.ViewPanel.HidePanel();
                }
            }
        }

        #region InnerClass

        [Serializable]
        public class ViewPanelAndActivateButton
        {
            public ViewPanel ViewPanel;
            public Button ActivateButton;
        }

        #endregion
    }
}