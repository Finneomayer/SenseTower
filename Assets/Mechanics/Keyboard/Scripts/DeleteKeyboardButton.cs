namespace Assets.Mechanics.Keyboard.Scripts
{
    public sealed class DeleteKeyboardButton : KeyboardButton
    {
        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestDeleteLastSymbol();
        }
    }
}
