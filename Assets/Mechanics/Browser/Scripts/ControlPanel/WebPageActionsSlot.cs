using System;
using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public class WebPageActionsSlot : MonoBehaviour
    {
        [SerializeField]
        private WebPageActionControl[] SlotActions;
        [SerializeField]
        private GameObject UnavailableObject;

        public Action<WebPageAction> ActionRequested;

        private void OnEnable()
        {
            foreach (var item in SlotActions)
            {
                item.ActionRequested += OnWebPageActionRequested;
            }
        }

        private void OnDisable()
        {
            foreach (var item in SlotActions)
            {
                item.ActionRequested -= OnWebPageActionRequested;
            }
        }

        private void OnWebPageActionRequested(WebPageAction action)
        {
            ActionRequested?.Invoke(action);
        }

        public void SetActiveActions(WebPageAction actions)
        {
            bool isSlotHasAction = false;
            foreach (var item in SlotActions)
            {
                if (actions.HasFlag(item.WebPageAction))
                {
                    item.SetActiveControl(true);
                    isSlotHasAction = true;
                }
                else
                {
                    item.SetActiveControl(false);
                }
            }

            if (UnavailableObject != null)
            {
                UnavailableObject.SetActive(!isSlotHasAction);
            }
        }
    }
}