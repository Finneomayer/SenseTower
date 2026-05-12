using UnityEngine;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public class KeyboardLayout : MonoBehaviour
    {
        [field: SerializeField]
        public KeyboardMode KeyboardMode { get; private set; }

        public void Init(KeyboardEventMediator keyboardEventMediator)
        {
            KeyboardButton[] buttons = GetComponentsInChildren<KeyboardButton>();
            foreach (var item in buttons)
            {
                item.Init(keyboardEventMediator);
            }
        }

        public void SetLayoutActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
