using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public abstract class KeyboardButton : MonoBehaviour, IPointerDownHandler
    {
        protected KeyboardEventMediator _keyboardEventMediator;

        public void OnPointerDown(PointerEventData eventData)
        {
            ProcessButtonDown();
        }

        public void Init(KeyboardEventMediator keyboardEventMediator)
        {
            _keyboardEventMediator = keyboardEventMediator;
        }

        protected abstract void ProcessButtonDown();
    }
}
