using UnityEngine;
using TMPro;
using System.Linq;
using System;

namespace Assets.Mechanics.Keyboard.Scripts
{

    public class KeyboardScript : MonoBehaviour
    {
        private const string EnterButtonCommand = "Enter"; 
        private const string BackspaceButtonCommand = "Backspace"; 
        
        [SerializeField]
        private KeyboardLayout[] Layouts;
        [SerializeField]
        private KeyboardLayout CommonButtonsLayout;
        [SerializeField]
        private KeyboardFeedback KeyboardFeedback;

        public KeyboardMode KeyboardMode =>_currentKeyboardMode;
        public TMP_InputField AttachedInputField => _textField;

        private const float _doubleClickDelay = 0.5f;

        private TMP_InputField _textField;
        private Canvas _keyboardCanvas;

        private KeyboardEventMediator _keyboardEventMediator;

        private KeyboardMode _currentKeyboardMode;
        private bool _keepUpperCase = false;
        private float _lastShiftTime = 0;
        private bool _integerFilterOn = false;


        public event Action<string> KeyPressed;

        private void Awake()
        {
            _keyboardEventMediator = new();
        }

        private void OnEnable()
        {
            _keyboardEventMediator.OnTextEnterRequested += OnTextEnterRequested;
            _keyboardEventMediator.OnSetKeyboardModeRequested += OnSetKeyboardModeRequested;
            _keyboardEventMediator.OnDeleteLastSymbolRequested += OnDeleteLastSymbolRequested;
        }

        private void OnDisable()
        {
            _keyboardEventMediator.OnTextEnterRequested -= OnTextEnterRequested;
            _keyboardEventMediator.OnSetKeyboardModeRequested -= OnSetKeyboardModeRequested;
            _keyboardEventMediator.OnDeleteLastSymbolRequested -= OnDeleteLastSymbolRequested;
        }

        private void Start()
        {
            _keyboardCanvas = GetComponent<Canvas>();

            foreach (var item in Layouts)
            {
                item.Init(_keyboardEventMediator);

                if (item.gameObject.activeInHierarchy == true) //define the start type of keyboard
                {
                    _currentKeyboardMode = item.KeyboardMode; 
                }
            }
            CommonButtonsLayout.Init(_keyboardEventMediator);
        }

        private void Update()
        {
            if (!ReferenceEquals(_textField, null))
            {
                _textField.caretPosition = _textField.text.Length;
                _textField.ForceLabelUpdate();
            }
        }

        private void OnDeleteLastSymbolRequested()
        {
            if (_textField != null)
            {
                if (_textField.text.Length > 0)
                {
                    _textField.text = _textField.text.Remove(_textField.text.Length - 1);
                }
                KeyPressed?.Invoke(BackspaceButtonCommand);
            }
            else
            {
                KeyPressed?.Invoke(BackspaceButtonCommand);
            }

            KeyboardFeedback.PlayKeyDownFeedback();
        }

        private void OnSetKeyboardModeRequested(KeyboardMode keyboardMode)
        {
            if (_textField == null && keyboardMode == KeyboardMode.Enter)
            {
                KeyPressed?.Invoke(EnterButtonCommand);
                KeyboardFeedback.PlayKeyDownFeedback();
                return;
            }

            _keepUpperCase = false;

            if (keyboardMode == KeyboardMode.EngUpperCase || keyboardMode == KeyboardMode.RusUpperCase)
            {
                _lastShiftTime = Time.time;
            }

            if (keyboardMode == KeyboardMode.EngLowerCase || keyboardMode == KeyboardMode.RusLowerCase)
            {
                var delta = Time.time - _lastShiftTime;

                if (delta < _doubleClickDelay)
                {
                    keyboardMode = keyboardMode == KeyboardMode.EngLowerCase ? KeyboardMode.EngUpperCase : KeyboardMode.RusUpperCase;
                    _keepUpperCase = true;
                }
            }

            _currentKeyboardMode = keyboardMode;

            if (keyboardMode == KeyboardMode.None || keyboardMode == KeyboardMode.Enter)
            {
                CloseKeyboard();
                KeyboardFeedback.PlayKeyDownFeedback();
                return;
            }

            KeyboardLayout layout = Layouts.First(l => l.KeyboardMode == keyboardMode);
            if (layout == null)
            {
                Debug.LogWarning($"Keyboard layout {keyboardMode} not found!");
                return;
            }

            CloseAllLayouts();
            layout.SetLayoutActive(true);
            KeyboardFeedback.PlayKeyDownFeedback();
        }

        private void OnTextEnterRequested(string text)
        {
            if (_textField != null)
            {
                _textField.Select();

                if (_integerFilterOn)
                {
                    var textChar = text.ToCharArray();

                    foreach (var charSymbol in textChar)
                    {
                        if (!Char.IsDigit(charSymbol)) return;
                    }
                }

                _textField.text += text;
            }

            KeyPressed?.Invoke(text);
            KeyboardFeedback.PlayKeyDownFeedback();

            if (!_keepUpperCase)
            {
                if (_currentKeyboardMode == KeyboardMode.EngUpperCase) ChangeToLowerCase(KeyboardMode.EngLowerCase);
                else if (_currentKeyboardMode == KeyboardMode.RusUpperCase) ChangeToLowerCase(KeyboardMode.RusLowerCase);
            }
        }

        private void ChangeToLowerCase(KeyboardMode keyboardMode)
        {
            KeyboardLayout layout = Layouts.First(l => l.KeyboardMode == keyboardMode);
            if (layout == null)
            {
                Debug.LogWarning($"Keyboard layout {keyboardMode} not found!");
                return;
            }

            CloseAllLayouts();
            layout.SetLayoutActive(true);
        }

        private void CloseAllLayouts()
        {
            foreach (var item in Layouts)
            {
                item.SetLayoutActive(false);
            }
        }

        public bool IsOpened()
        {
            return _keyboardCanvas.enabled;
        }

        public void OpenKeyboard(TMP_InputField inputField, bool integerFilterOn = false)
        {            
            if (_keyboardCanvas != null)
            {
                _keyboardCanvas.enabled = true;
            }

            if (_textField != null)
            {
                _textField.onValueChanged.RemoveListener(OnTextFieldValueChanged);
            }
            _textField = inputField;
            if (_textField != null)
            {
                _textField.shouldHideSoftKeyboard = true;
                _textField.onValueChanged.AddListener(OnTextFieldValueChanged);
            }

            _integerFilterOn = integerFilterOn;
        }

        private void OnTextFieldValueChanged(string text)
        {
            if (_textField == null)
            {
                return;
            }
            if (_textField.characterLimit != 0 && _textField.text.Length > _textField.characterLimit)
            {
                _textField.text = _textField.text.Substring(0, _textField.characterLimit);
            }
        }

        public void SetMode(KeyboardMode keyboardMode)
        {
            _currentKeyboardMode = keyboardMode;

            KeyboardLayout layout = Layouts.First(l => l.KeyboardMode == keyboardMode);
            if (layout == null)
            {
                Debug.LogWarning($"Keyboard layout {keyboardMode} not found!");
                return;
            }

            CloseAllLayouts();
            layout.SetLayoutActive(true);
        }

        public void CloseKeyboard()
        {
            if (_keyboardCanvas != null)
            {
                _keyboardCanvas.enabled = false;
            }
            if (_textField != null)
            {
                _textField.onValueChanged.RemoveListener(OnTextFieldValueChanged);
            }
        }
    }
}
