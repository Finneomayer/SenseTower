using UnityEngine;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public sealed class SetModeKeyboardButton : KeyboardButton
    {
        [SerializeField]
        private KeyboardMode KeyboardMode;

        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestSetKeyboardMode(KeyboardMode);
        }
    }
}
