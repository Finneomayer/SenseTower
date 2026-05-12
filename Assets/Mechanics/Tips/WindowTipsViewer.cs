using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Mechanics.Tips
{
    public class WindowTipsViewer : TipsViewer
    {
        #region Inspector

        [SerializeField] private Texture2D _cursorTexture;
        [SerializeField] private InputActionProperty _tipsToogleActionProperty;

        #endregion

        private void Start()
        {
            Init();
        }

        public override void ShowPanel()
        {
#if UNITY_STANDALONE
            base.ShowPanel();
            Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
#endif
        }

        public override void HidePanel()
        {
            base.HidePanel();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        protected override void Init()
        {
#if !UNITY_STANDALONE
            gameObject.SetActive(false);
#endif
            _tipsToogleActionProperty.action.performed += OnShowTipsWindowActionToogle;
        }

        protected override void ToogleTips()
        {
#if UNITY_STANDALONE
            base.ToogleTips();
#endif
        }

        private void OnShowTipsWindowActionToogle(InputAction.CallbackContext obj)
        {
            ToogleTips();
        }
    }
}