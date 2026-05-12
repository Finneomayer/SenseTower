using Assets.Localization;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class MafiaWinnerCup : MonoBehaviour
    {
        [SerializeField] private Transform _cupVisual;
        [SerializeField] private Animation _cupAnimation;
        [SerializeField] private AudioSource _cupAudio;
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private LookAtPlayer _winCanvas;
        [SerializeField] private LocalizationVariant _mafiaWinText;
        [SerializeField] private LocalizationVariant _citizenWinText;
        

        public void SetPlayer(Transform player)
        {
            _winCanvas.SetPlayer(player);
        }

        public void SetGameState(GameState gameState)
        {
            if (gameState == null) DisableWinnerCup();
            else
            {
                if (gameState.GameStatus == MafiaGameStatus.CitizensWin) ShowWinnerAction(_citizenWinText.Localize());
                else if (gameState.GameStatus == MafiaGameStatus.MafiaWin) ShowWinnerAction(_mafiaWinText.Localize());
            }
        }
        
        private void ShowWinnerAction(string winner)
        {
            _cupVisual.gameObject.SetActive(true);
            _winText.text = winner;
            _cupAnimation.Play();
            _cupAudio.Play();
        }

        private void DisableWinnerCup()
        {
            _cupVisual.gameObject.SetActive(false);
        }
    }
}
