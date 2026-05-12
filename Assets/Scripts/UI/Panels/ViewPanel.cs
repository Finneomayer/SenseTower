using Assets.Mechanics.Keyboard.Scripts;
using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ViewPanel : MonoBehaviour
    {
        public event Action PanelShown;
        public event Action PanelHidden;
        public event Action<bool> PanelBlockedStatusChanged;
        public bool PanelBlocked => _panelBlocked;
        protected bool _panelBlocked = false;

        #region Inspector
        public CanvasGroup CanvasGroup;
        #endregion

        public KeyboardScript Keyboard { get; private set; }

        protected virtual void Awake()
        {
            if (CanvasGroup == null)
                CanvasGroup = GetComponent<CanvasGroup>();
            
            HidePanel(); }

        public virtual void Init(KeyboardScript keyboard)
        {
            Keyboard = keyboard;
        }

        public virtual void ShowPanel()
        {
            if (ReferenceEquals(CanvasGroup, null)) return;

            CanvasGroup.alpha = 1;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;

            PanelShown?.Invoke();
        }

        public virtual void HidePanel()
        {
            if (ReferenceEquals(CanvasGroup, null)) return;

            CanvasGroup.alpha = 0;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;

            if (Keyboard != null)
            {
                Keyboard.CloseKeyboard();
            }
            PanelHidden?.Invoke();
        }

        public void SetPanelStatus(bool isBlocked)
        {
            _panelBlocked = isBlocked;
            PanelBlockedStatusChanged?.Invoke(isBlocked);
        }

        public virtual bool IsVisible()
        {
            if (ReferenceEquals(CanvasGroup, null))
            {
                return false;
            }

            return CanvasGroup.alpha > 0;
        }

        protected void RaisePanelShownEvent()
        {
            PanelShown?.Invoke();
        }

        protected void RaisePanelHiddenEvent()
        {
            PanelHidden?.Invoke();
        }
    }
}
