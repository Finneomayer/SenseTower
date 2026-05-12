using System;
using System.Linq;
using Assets.Localization;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatTextVisualizer : MonoBehaviour
    {
        private const string DefaultText = "<unknown>";
        private const string SeatNumberPrefixText = "№";
        private const string GameMasterText = "Ведущий";
        private const string LocalUserRoleDontWorryText = "(роль видна только вам)";
        //private const string VotesCountPrefixText = "Набрал голосов";
        private const string SelectedPlayerPrefixText = "Голосую за";
        private const string SelectedSelfPlayerText = "себя";

        //[SerializeField] private GameObject AdminTextPanel;
        [SerializeField] private GameObject PlayerTextPanel;
        [SerializeField] private GameObject SelectOtherPlayerPanel;

        [SerializeField] private TMP_Text SeatNumberText;
        [SerializeField] private TMP_Text SeatNumberTextSelf;
        //[SerializeField] private TMP_Text VotesCountText;
        [SerializeField] private TMP_Text SelectedPlayerText;
        [SerializeField] private TMP_Text SelectedPlayerText2;
        [SerializeField] private TMP_Text _selfSeeingText;

        [SerializeField] private LocalizationVariant _mafia;
        [SerializeField] private LocalizationVariant _notMafia;

        private int _playerNumber;
        /// <summary>
        /// this place is my place (of this client)
        /// </summary>
        private bool _isLocalPlayer;
        
        private PlayerState _thisPlacePlayerState;
        private string _thisPlaceRoleName;
        private MafiaLocalizationResultDto _mafiaLocalizationData;

        public void Init(PlayerState thisPlacePlayerState, MafiaLocalizationResultDto mafiaLocalizationData, PlayerState thisClientPlayerState)
        {
            if (thisPlacePlayerState == null)
            {
                //AdminTextPanel.SetActive(false);
                PlayerTextPanel.SetActive(false);
                SelectOtherPlayerPanel.SetActive(false);
                return;
            }

            _thisPlacePlayerState = thisPlacePlayerState;
            _mafiaLocalizationData = mafiaLocalizationData;
            _mafiaLocalizationData.RoleNames.TryGetValue(_thisPlacePlayerState.Role, out _thisPlaceRoleName);

            _playerNumber = thisPlacePlayerState.Number;
            _isLocalPlayer = thisPlacePlayerState.PlayerId == thisClientPlayerState.PlayerId;

            SetPlayerTexts(thisPlacePlayerState, _thisPlaceRoleName, _isLocalPlayer);

            if (_isLocalPlayer)
            {
                ShowRoleToYourself();
            }
        }

        public void ShowRoleToAdmin()
        {
            if (_thisPlacePlayerState.Role == MafiaPlayerRole.GameMaster)
            {
                return;
            }
            SeatNumberText.text = $"{SeatNumberPrefixText}: {_playerNumber}\nРоль: {_thisPlaceRoleName}";
            SeatNumberTextSelf.text = SeatNumberText.text;
        }

        /// <summary>
        /// For Commissioner & Sergent to see role of selected player, except Don (!)
        /// </summary>
        public void ShowRoleToPoliceIfHasEffect()
        {
            if (_thisPlacePlayerState.Role is MafiaPlayerRole.Commissioner or MafiaPlayerRole.Sergent)
            {
                return;
            }

            if (_thisPlacePlayerState.Effects.TryGetValue(Effect.InvestigatedByCommissioner, out int count))
            {
                if (count > 0)
                {
                    string playerSeatRole = _notMafia.Localize();
                    //game exception is to show Don as Citizen!
                    if (_thisPlacePlayerState.Role == MafiaPlayerRole.Mafia)
                    {
                        playerSeatRole = _mafia.Localize();
                    }

                    SeatNumberText.text = $"{playerSeatRole}";
                    SeatNumberTextSelf.text = SeatNumberText.text;
                }
            }
        }


        /// <summary>
        /// For Don & Mafia to see each other
        /// </summary>
        public void ShowMafiaDonEachOther()
        {
            if (_thisPlacePlayerState.Role is MafiaPlayerRole.Don or MafiaPlayerRole.Mafia)
            {
                string playerSeatRole = _thisPlaceRoleName;

                SeatNumberText.text = $"{SeatNumberPrefixText}: {_playerNumber}\nРоль: {playerSeatRole}";
                SeatNumberTextSelf.text = SeatNumberText.text;
            }
        }

        private void ShowRoleToYourself()
        {
            if (_isLocalPlayer && _thisPlacePlayerState.Role == MafiaPlayerRole.GameMaster)
            {
                return;
            }
            SeatNumberTextSelf.text = $"{SeatNumberPrefixText}: {_playerNumber}\nРоль: {_thisPlaceRoleName}";
        }

        private void SetPlayerTexts(PlayerState thisSeatPlayerData, string thisSeatRoleName, bool isLocalPlayer)
        {
            if (thisSeatPlayerData == null)
            {
                SeatNumberText.text = $"{SeatNumberPrefixText}: {DefaultText}";
                return;
            }

            if (thisSeatPlayerData.Role == MafiaPlayerRole.GameMaster)
            {
                SeatNumberText.text = $"{GameMasterText}";
                _selfSeeingText.text = "";
            }
            else
            {
                SeatNumberText.text = $"{SeatNumberPrefixText}: {thisSeatPlayerData.Number}";

                //for dead player
                if (!thisSeatPlayerData.IsAlive) SeatNumberText.text = $"{SeatNumberPrefixText}: {_playerNumber}\nРоль: {thisSeatRoleName}";

                _selfSeeingText.text = isLocalPlayer ? LocalUserRoleDontWorryText : "";
            }
            SeatNumberTextSelf.text = SeatNumberText.text;

            //VotesCountText.text = $"{VotesCountPrefixText}: {thisSeatPlayerData.VoteCount}";

            if (thisSeatPlayerData.Role != MafiaPlayerRole.GameMaster
                && thisSeatPlayerData.AvailableActions.Contains(MafiaPlayerAction.Vote)
                && thisSeatPlayerData.SelectedNumberOfOtherPlayer != PlayerState.DefaultSelectedNumber)
            {
                if (thisSeatPlayerData.Number == thisSeatPlayerData.SelectedNumberOfOtherPlayer)
                {
                    SelectedPlayerText.text = $"{SelectedPlayerPrefixText}\n{SelectedSelfPlayerText}";
                }
                else
                {
                    SelectedPlayerText.text = $"{SelectedPlayerPrefixText}\n" +
                        $"{thisSeatPlayerData.SelectedNumberOfOtherPlayer}";
                }

                SelectOtherPlayerPanel.SetActive(true);
            }
            else
            {
                SelectOtherPlayerPanel.SetActive(false);
                SelectedPlayerText.text = "";
            }
            SelectedPlayerText2.text = SelectedPlayerText.text;
        }
    }
}
