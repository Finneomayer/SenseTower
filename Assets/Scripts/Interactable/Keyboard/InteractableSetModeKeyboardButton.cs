using Assets.Mechanics.Keyboard.Scripts;
using UnityEngine;

namespace Sense.Interectable.Keyboard
{
    public class InteractableSetModeKeyboardButton : InteractableKeyboardButton
    {
        [SerializeField]
        private KeyboardMode _keyboardMode;

        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestSetKeyboardMode(_keyboardMode);
        }
    }
}