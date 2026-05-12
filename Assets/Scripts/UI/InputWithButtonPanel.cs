using System;
using Assets.Mechanics.Keyboard.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InputWithButtonPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button SubmitButton;

        private KeyboardScript _keyboard;

        public Action<string> SubmitRequested;

        private void OnEnable()
        {
            InputField.onSelect.AddListener(OnInputFieldSelect);
            //InputField.onDeselect.AddListener(OnInputFieldDeselect);

            SubmitButton.onClick.AddListener(OnSubmitButtonClick);
        }

        private void OnDisable()
        {
            InputField.onSelect.RemoveListener(OnInputFieldSelect);
            //InputField.onDeselect.RemoveListener(OnInputFieldDeselect);

            SubmitButton.onClick.RemoveListener(OnSubmitButtonClick);
        }

        public void Init(KeyboardScript keyboard)
        {
            _keyboard = keyboard;
        }

        public void OpenKeyboard()
        {
            if (_keyboard != null)
            {
                _keyboard.OpenKeyboard(InputField);
            }
        }

        public void CloseKeyboard()
        {
            if (_keyboard != null)
            {
                _keyboard.CloseKeyboard();
            }
        }

        public void Clear()
        {
            InputField.text = string.Empty;
        }

        private void OnSubmitButtonClick()
        {
            SubmitRequested?.Invoke(InputField.text);
        }

        private void OnInputFieldSelect(string arg0)
        {
            if (_keyboard != null)
            {
                _keyboard.OpenKeyboard(InputField);
            }
        }

        //private void OnInputFieldDeselect(string arg0)
        //{
        //    if (_keyboard != null)
        //    {
        //        _keyboard.CloseKeyboard();
        //    }
        //}
    }
}
