using System;
using TMPro;
using UnityEngine;

namespace Assets.Mechanics.PadKeyboard
{
    public class KeyboardTextButton : MonoBehaviour
    {
        [SerializeField] private string[] _values;
        /// <summary>
        /// when you want to yse button as command button, not text button
        /// </summary>
        [SerializeField] private string _commandOverride;
        [Space]
        [SerializeField] private RaycastButton _raycastButton;
        [SerializeField] private FingerPhysicButton _physicButton;
        [SerializeField] private TMP_Text _buttonText;

        public event Action<string> OnClickText;
        private PadButtonVariant _currentVariant;

        private string CurrentButtonValue => _values[(int)_currentVariant];

        public void SetButtonVariant(PadButtonVariant type)
        {
            _currentVariant = type;
            _buttonText.text = CurrentButtonValue.ToString();
        }

        public void Hide()
        {
            _raycastButton.Hide();
            _physicButton.Hide();
        }

        public void Show()
        {
            _raycastButton.Show();
            _physicButton.Show();
        }

        private void Start()
        {
            if (_values.Length < 6) Debug.LogError("Not all button values are set");
            SetButtonVariant(PadButtonVariant.EngUpper);

            //_raycastButton.OnClick += ButtonInvoke;
            _physicButton.OnPress += ButtonInvoke;
        }

        private void ButtonInvoke()
        {
            OnClickText?.Invoke(string.IsNullOrEmpty(_commandOverride) ? CurrentButtonValue : _commandOverride);
        }
    }
}
