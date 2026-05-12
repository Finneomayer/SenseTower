using UnityEngine;
using UI;
using System.Collections;
using TMPro;
using Assets.Localization;

namespace Assets.Blackboard
{
    public class SnapshotPanel : ViewPanel
    {
        [SerializeField]
        private TMP_Text TextAfterSave;
        [SerializeField]
        private float ShowDelay = 2f;
        [SerializeField]
        private LocalizationVariant TextAfterSaveLocalizationVariant;

        private Coroutine _showCoroutine;

        public override void HidePanel()
        {
            base.HidePanel();
            if (_showCoroutine != null)
            {
                StopCoroutine(_showCoroutine);
                _showCoroutine = null;
            }
        }

        public void ShowPanel(string snapshotPath)
        {
            TextAfterSave.text = TextAfterSaveLocalizationVariant.Localize(snapshotPath);

            if (_showCoroutine != null)
            {
                StopCoroutine(_showCoroutine);
            }
            _showCoroutine = StartCoroutine(ShowPanelRoutine(ShowDelay));         
        }

        private IEnumerator ShowPanelRoutine(float delay)
        {
            ShowPanel();
            yield return new WaitForSeconds(delay);
            HidePanel();

            _showCoroutine = null;
        } 

    }
}
