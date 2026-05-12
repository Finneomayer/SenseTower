using UnityEngine;
using System;
using Assets.Mechanics.PadKeyboard;
using Vuplex.WebView;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public class KeyboardWebAdapter : MonoBehaviour
    {
        [SerializeField]
        private KeyboardScript KeyboardScript; //for old UI browser keyboard
        [SerializeField]
        private PhysicKeyboard _physicKeyboard; //for new physic keyboard (optional)

        public event Action<object, EventArgs<string>> KeyPressed;

        private void OnEnable()
        {
            if (KeyboardScript != null)
            {
                KeyboardScript.KeyPressed += OnKeyPressed;
                KeyboardScript.OpenKeyboard(null);
            }
        }            

        private void OnDisable()
        {
            if (KeyboardScript != null)
            {
                KeyboardScript.KeyPressed -= OnKeyPressed;
                KeyboardScript.CloseKeyboard();
            }
            if (_physicKeyboard != null) _physicKeyboard.KeyPressed -= OnKeyPressed;
        }

        public void SetSecondaryKeyboard(PhysicKeyboard keyboard)
        {
            if (_physicKeyboard != null)
                _physicKeyboard.KeyPressed -= OnKeyPressed;
            
            _physicKeyboard = keyboard;
            _physicKeyboard.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(string key)
        {
            KeyPressed?.Invoke(null, new EventArgs<string>(key));
        }
    }
}
