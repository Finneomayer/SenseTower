using Assets.Mechanics.Keyboard.Scripts;
using System;
using UnityEngine;

namespace Assets.Mechanics.PadKeyboard
{
    public class PhysicKeyboard : MonoBehaviour
    {
        [SerializeField] private KeyboardTextButton[] _buttons;
        [SerializeField] private GameObject _baseVisual; 
        [SerializeField] private KeyboardFeedback KeyboardFeedback;
        public event Action<string> KeyPressed;

        private const string EnterButtonCommand = "Enter";
        private const string BackspaceButtonCommand = "Backspace";
        private const float DoubleClickDelay = 0.5f;

        private PadButtonVariant _currentVariant = PadButtonVariant.EngUpper;
        
        private PadButtonVariant CurrentVariant
        {
            set
            {
                _currentVariant = value;
                UpdateKeyboardVariant();
            }
        }

        private bool _keepUpperCase = false;
        private float _lastUpperSetTime = 0;


        public void OpenKeyboard()
        {
            _baseVisual.SetActive(true);
            foreach (var button in _buttons)
            {
                button.Show();
            }
        }

        public void CloseKeyboard()
        {
            _baseVisual.SetActive(false);
            foreach (var button in _buttons)
            {
                button.Hide();
            }
        }

        private void Start()
        {
            foreach (var button in _buttons)
            {
                button.OnClickText += Button_OnClickText;
            }
        }

        private void Button_OnClickText(string textInput)
        {
            KeyboardFeedback.PlayKeyDownFeedback();

            switch (textInput)
            {
                case "Shift":
                    ShiftCommand();
                    return;
                case "Backspace":
                    BackspaceCommand();
                    return;
                case "Language":
                    LanguageCommand();
                    return;
                case "Symbol":
                    SymbolCommand();
                    return;
                case "Enter":
                    EnterCommand();
                    return;
                default:
                    KeyPressed?.Invoke(textInput);
                    if (!_keepUpperCase)
                    {
                        if (_currentVariant == PadButtonVariant.RusUpper) CurrentVariant = PadButtonVariant.RusLower;
                        if (_currentVariant == PadButtonVariant.EngUpper) CurrentVariant = PadButtonVariant.EngLower;
                    }
                    break;
            }
        }

        private void UpdateKeyboardVariant()
        {
            foreach (var button in _buttons)
            {
                button.SetButtonVariant(_currentVariant);
            }
        }

        private void ShiftCommand()
        {
            float delta = 0;
            _keepUpperCase = false;

            switch (_currentVariant)
            {
                case PadButtonVariant.EngUpper:
                    delta = Time.time - _lastUpperSetTime;
                    if (delta < DoubleClickDelay)
                    {
                        CurrentVariant = PadButtonVariant.EngUpper;
                        _keepUpperCase = true;
                        return;
                    }
                    CurrentVariant = PadButtonVariant.EngLower;
                    return;
                case PadButtonVariant.EngLower:
                    CurrentVariant = PadButtonVariant.EngUpper;
                    _lastUpperSetTime = Time.time;
                    return;
                case PadButtonVariant.RusUpper:
                    delta = Time.time - _lastUpperSetTime;
                    if (delta < DoubleClickDelay)
                    {
                        CurrentVariant = PadButtonVariant.RusUpper;
                        _keepUpperCase = true;
                        return;
                    }
                    CurrentVariant = PadButtonVariant.RusLower;
                    return;
                case PadButtonVariant.RusLower:
                    CurrentVariant = PadButtonVariant.RusUpper;
                    _lastUpperSetTime = Time.time;
                    return;
            }
        }

        private void LanguageCommand()
        {
            switch (_currentVariant)
            {
                case PadButtonVariant.EngUpper:
                    CurrentVariant = PadButtonVariant.RusUpper;
                    return;
                case PadButtonVariant.EngLower:
                    CurrentVariant = PadButtonVariant.RusLower;
                    return;
                case PadButtonVariant.RusUpper:
                    CurrentVariant = PadButtonVariant.EngUpper;
                    return;
                case PadButtonVariant.RusLower:
                    CurrentVariant = PadButtonVariant.EngLower;
                    return;
                case PadButtonVariant.Symbol1:
                    CurrentVariant = PadButtonVariant.RusLower;
                    return;
                case PadButtonVariant.Symbol2:
                    CurrentVariant = PadButtonVariant.RusLower;
                    return;
            }
        }

        private void SymbolCommand()
        {
            switch (_currentVariant)
            {
                case PadButtonVariant.EngUpper:
                    CurrentVariant = PadButtonVariant.Symbol1;
                    return;
                case PadButtonVariant.EngLower:
                    CurrentVariant = PadButtonVariant.Symbol1;
                    return;
                case PadButtonVariant.RusUpper:
                    CurrentVariant = PadButtonVariant.Symbol1;
                    return;
                case PadButtonVariant.RusLower:
                    CurrentVariant = PadButtonVariant.Symbol1;
                    return;
                case PadButtonVariant.Symbol1:
                    CurrentVariant = PadButtonVariant.Symbol2;
                    return;
                case PadButtonVariant.Symbol2:
                    CurrentVariant = PadButtonVariant.Symbol1;
                    return;
            }
        }

        private void EnterCommand()
        {
            KeyPressed?.Invoke(EnterButtonCommand);
        }

        private void BackspaceCommand()
        {
            KeyPressed?.Invoke(BackspaceButtonCommand);
        }
    }
}
