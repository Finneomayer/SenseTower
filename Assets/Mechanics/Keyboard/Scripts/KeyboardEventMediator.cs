using System;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public class KeyboardEventMediator
    {
        public event Action<string> OnTextEnterRequested;
        public event Action<KeyboardMode> OnSetKeyboardModeRequested;
        public event Action OnDeleteLastSymbolRequested;

        public void RequestTextEnter(string text)
        {
            OnTextEnterRequested?.Invoke(text);
        }

        public void RequestDeleteLastSymbol()
        {
            OnDeleteLastSymbolRequested?.Invoke();
        }

        public void RequestSetKeyboardMode(KeyboardMode newKeyboardMode)
        {
            OnSetKeyboardModeRequested?.Invoke(newKeyboardMode);
        }
    }
}
