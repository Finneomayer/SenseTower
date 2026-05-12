using TMPro;
using UnityEngine;

namespace Assets.Localization
{
    /// <summary>
    /// Add this component to game object with TMP_Text and set LocalizationKey to autotranslate text 
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [field:SerializeField]
        public string LocalizationKey { get; private set; }

        [field: SerializeField]
        public bool AutoTranslate { get; private set; } = true;

        private TMP_Text _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
            LocalizationManager.LanguageChanged += OnLanguageChanged;
            OnLanguageChanged();
        }

        private void OnDestroy()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        public void Localize()
        {
            if (LocalizationManager.IsLanguageSetted)
            {
                string newTextValue = LocalizationManager.Localize(LocalizationKey);
                if (!newTextValue.Equals(LocalizationManager.NotLocalizedString))
                {
                    _textComponent.text = newTextValue;
                }
            }
        }

        public void Localize(params object[] args)
        {
            if (LocalizationManager.IsLanguageSetted)
            {
                _textComponent.text = LocalizationManager.Localize(LocalizationKey, args);
            }
        }

        public void SetKey(string localizationKey, bool translateImmediate = true)
        {
            LocalizationKey = localizationKey;
            if (translateImmediate)
            {
                Localize();
            }
        }

        private void OnLanguageChanged()
        {
            if (AutoTranslate)
            {
                Localize();
            }
        }
    }
}



