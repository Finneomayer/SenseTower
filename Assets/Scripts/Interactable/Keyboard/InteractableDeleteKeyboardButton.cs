
namespace Sense.Interectable.Keyboard
{
    public class InteractableDeleteKeyboardButton : InteractableKeyboardButton
    {
        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestDeleteLastSymbol();
        }
    }
}
