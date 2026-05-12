using Assets.Scripts.Space;
using Data;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class TipsButtonAdditionalCondition : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private ButtonUI _buttonUI;
        private ISpaceModeData _spaceModeData;

        #endregion

        [Inject]
        private void Construct(ISpaceModeData spaceModeData)
        {
            _spaceModeData = spaceModeData;
        }

        private void Update()
        {
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
            {
                ChangeBackgroundAlpha(true);
            }
            else
            {
                ChangeBackgroundAlpha(false);
            }
        }

        private void ChangeBackgroundAlpha(bool isVisible)
        {
            var tempColor = _buttonUI.Background.color;
            tempColor.a = isVisible ? 1f : 0f;
            _buttonUI.Background.color = tempColor;
        }
    }
}