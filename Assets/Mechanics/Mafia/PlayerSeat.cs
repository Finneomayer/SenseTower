using Assets.Mechanics.Mafia.UI;
using System;
using Meta.WitAi;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{

    public class PlayerSeat : MonoBehaviour
    {
        [SerializeField]
        private PlayerSeatSelectable PlayerSeatSelectable;
        [SerializeField]
        private PlayerSeatSelectable PlayerSelfSeatSelectable;
        [SerializeField]
        private PlayerSeatTextVisualizer TextVisualizer;
        [SerializeField]
        private MafiaVotesCountView VotesCountView;
        [SerializeField] private MafiaEffectOnPlayerView _effectOnPlayerView;
        [SerializeField]
        private GameObject AdminMarkerPanel;
        [SerializeField]
        private PlayerTimerView PlayerKickTimerView;
        [Space, Header("Effects")]
        [SerializeField] private PlayerSeatDeathEffect _deathEffect;
        [SerializeField] private MeshRenderer _roleCube;
        [SerializeField] private Material _roleCubeDefault;
        [SerializeField] private Material _roleCubeBlack;
        [SerializeField] private Material _roleCubeGreen;
        [SerializeField] private TMP_Text[] _roleTexts;

        private PlayerState _playerSeatData;
        private bool _canBeSelected;
        private MafiaEventMediatorClient _eventMediator;
        private MafiaLocalizationResultDto _mafiaLocalizationData;
        private PlayerSeatSelectable _currentSeatSelectable;

        public bool IsSelected { get; private set; }
        public PlayerState PlayerSeatData => _playerSeatData;

        public void Init(GameState gameState, MafiaEventMediatorClient eventMediator, MafiaLocalizationResultDto mafiaLocalizationData, 
            PlayerState playerSeatData, bool canBeSelected, bool highlightSeatTab, PlayerState currentPlayerState, bool wasDead)
        {
            _eventMediator = eventMediator;
            _mafiaLocalizationData = mafiaLocalizationData;
            _playerSeatData = playerSeatData;
            _canBeSelected = canBeSelected;

            _currentSeatSelectable = playerSeatData == currentPlayerState ? PlayerSelfSeatSelectable : PlayerSeatSelectable;
            _currentSeatSelectable.Init(_eventMediator, _playerSeatData.Number, _canBeSelected);

            TextVisualizer.Init(_playerSeatData, _mafiaLocalizationData, currentPlayerState);
            VotesCountView.Init(gameState, _playerSeatData.VoteCount, _currentSeatSelectable.GetSelectedColor());
            _effectOnPlayerView.Init(gameState.GameStage,_playerSeatData.Effects);
            _deathEffect.Init(playerSeatData, wasDead);
            
            #region Role cube color
            //white - black
            _roleCube.material = (playerSeatData.IsAlive) ? _roleCubeDefault : _roleCubeBlack;

            //green if player
            if (highlightSeatTab) _roleCube.material = _roleCubeGreen;

            SetRoleTextsColor((playerSeatData.IsAlive) ? Color.black : Color.white);

            #endregion            

            AdminMarkerPanel.SetActive(playerSeatData.Role == MafiaPlayerRole.GameMaster);
        }

        public void SetKickTimerActive(bool active)
        {
            PlayerKickTimerView.SetActiveTimer(active);
        }

        public void SetKickTimerValue(int timeInSeconds)
        {
            PlayerKickTimerView.SetTimeInSeconds(timeInSeconds);
        }

        public void SelectSeat()
        {
            IsSelected = true;
            _currentSeatSelectable.SelectSeat();
            VotesCountView.SetSelected(IsSelected);
        }

        public void UnselectSeat()
        {
            IsSelected = false;
            _currentSeatSelectable.UnselectSeat();
            VotesCountView.SetSelected(IsSelected);
        }

        public void ShowRole()
        {
            TextVisualizer.ShowRoleToAdmin();
        }

        public void ApplyInvestigatedByCommissionerEffect()
        {
            TextVisualizer.ShowRoleToPoliceIfHasEffect();
        }

        public void ShowMafiaDonEachOther()
        {
            TextVisualizer.ShowMafiaDonEachOther();
        }

        private void SetRoleTextsColor(Color color)
        {
            foreach (var text in _roleTexts)
            {
                text.color = color;
            }
        }
    }
}
