using UnityEngine;

namespace Assets.Localization
{
    [System.Serializable]
    public class LocalizationVariant
    {
        [SerializeField]
        private string LocalizationKey;
        [SerializeField]
        private string DefaultValue;

        public void Init(string localizationKey, string defaultValue)
        {
            LocalizationKey = localizationKey;
            DefaultValue = defaultValue;
        }

        public string Localize()
        {
            if (IsLocalizable())
            {
                return LocalizationManager.Localize(LocalizationKey);
            }
            return DefaultValue;
        }

        public string Localize(params object[] arguments)
        {
            if (IsLocalizable())
            {
                return LocalizationManager.Localize(LocalizationKey, arguments);
            }
            return string.Format(DefaultValue, arguments);
        }

        private bool IsLocalizable()
        {
            return !string.IsNullOrEmpty(LocalizationKey) && LocalizationManager.IsLanguageSetted;
        }
    }
}



