using Assets.Mechanics.Keyboard.Scripts;
using UnityEngine;

namespace Sense.Interectable.Keyboard
{
    public abstract class InteractableKeyboardButton : KeyboardButton
    {
        [SerializeField]
        private string _tagMask = "PlayerFingers";
        [SerializeField]
        private float _resetTime = 0.5f;

        //возможно в будущем надо вынести
        private Behaviour _trackComponent;
        private CanvasGroup _canvasGroup;

        private bool IsPermissionCollider => _trackComponent.enabled && (_canvasGroup == null ||  _canvasGroup.alpha > 0);

        //ToDo костыль. для переключения клавиатур
        private float _frame;
        private float _time;

        private void Awake()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>(true);
            _trackComponent = GetComponentInParent<Canvas>(true);
            if (_trackComponent == null)
                _trackComponent = this;
        }

        private void OnEnable()
        {
            _frame = Time.frameCount;
        }

        public void OnTriggerEnter(Collider other)
        {
         
            if (other.tag == "Untagged" || !other.CompareTag(_tagMask) || !enabled  || !IsPermissionCollider
                || _frame+1 == Time.frameCount ||   Time.realtimeSinceStartup - _time < _resetTime) return;


            _time = Time.realtimeSinceStartup;
            ProcessButtonDown();
            OnColliderEnterHandler();
        }

        protected virtual void OnColliderEnterHandler()
        {
          
        }
    }
}