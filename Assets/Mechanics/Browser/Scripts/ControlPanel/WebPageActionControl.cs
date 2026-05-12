using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Browser
{
    public class WebPageActionControl : MonoBehaviour
    {
        [field: SerializeField]
        public Button Button { get; private set; }
        [field: SerializeField]
        public WebPageAction WebPageAction { get; private set; }

        public event Action<WebPageAction> ActionRequested;

        private void OnEnable()
        {
            Button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnButtonClick);
        }

        public void SetActiveControl(bool active)
        {
            if (Button != null && Button.isActiveAndEnabled) Button.gameObject.SetActive(active);
        }

        private void OnButtonClick()
        {
            ActionRequested?.Invoke(WebPageAction);
        }

        public void ClickRemote()
        {
            Button.onClick.Invoke();
            Debug.LogWarning("oNcLICK " + gameObject.name);
        }
    }
}
