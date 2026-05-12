using UnityEngine;
using TMPro;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public sealed class TextKeyboardButton : KeyboardButton
    {
        [SerializeField]
        private TMP_Text ButtonText;

        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestTextEnter(ButtonText.text);
        }
    }
}
