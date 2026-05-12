using UnityEngine;
using UI;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;

namespace Assets.Blackboard
{
    public class DeleteBlackBoardContentPanel : ViewPanel
    {
        [SerializeField]
        private Button ConfirmButton;
        [SerializeField]
        private Button CancelButton;

        public event Action DeleteButtonClicked;
        public event Action CancelButtonClicked;

        private void OnEnable()
        {
            ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
            CancelButton.onClick.AddListener(OnCancelButtonClick);
        }

        private void OnDisable()
        {
            ConfirmButton.onClick.RemoveListener(OnConfirmButtonClick);
            CancelButton.onClick.RemoveListener(OnCancelButtonClick);
        }

        private void OnConfirmButtonClick()
        {
            DeleteButtonClicked?.Invoke();
        }

        private void OnCancelButtonClick()
        {
            CancelButtonClicked?.Invoke();
        }
    }
}
