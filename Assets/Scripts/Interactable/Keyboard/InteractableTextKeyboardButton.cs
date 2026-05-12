using TMPro;

namespace Sense.Interectable.Keyboard
{
    public class InteractableTextKeyboardButton : InteractableKeyboardButton
    {
      //  [SerializeField]
        private TMP_Text _buttonText;

        private void Start()
        {
            //TODO там 100500 кнопок. назначать в ручную - оч долго и рутинно. один раз при старте
            _buttonText = GetComponentInChildren<TMP_Text>();
        }

        protected override void ProcessButtonDown()
        {
            _keyboardEventMediator.RequestTextEnter(_buttonText.text);
        }
    }
}
