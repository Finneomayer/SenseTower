using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Localization
{
    /// <summary>
    /// Class for getting and setting localization
    /// </summary>
    public static class LocalizationManager
    {
        public const string NotLocalizedString = "<not_localized_string>";
        private static Dictionary<string, LanguageItem> _languageDataMap = new();

        public static bool IsLanguageSetted { get; private set; }
        public static string CurrentLanguageCode { get; private set; }
        public static SystemLanguage CurrentSystemLanguage { get; private set; }

        public static event Action LanguageChanged;

        public static CultureInfo GetCurrentCultureInfo()
        {
            if (LocalizationConst.LanguageCultureInfoMap.TryGetValue(CurrentSystemLanguage, out CultureInfo cultureInfo))
            {
                return cultureInfo;
            }
            return CultureInfo.CurrentCulture;
        }

        public static void SetDefaultLanguage()
        { 
            string systemLanguage = Application.systemLanguage.ToString();
            int textLength = systemLanguage.Length;

            systemLanguage = systemLanguage.Remove(2, textLength - 2);
            systemLanguage = systemLanguage.ToLower();

            SetLanguage(systemLanguage);
        }

        public static void SetLanguage(string languageCode)
        {
            if (!languageCode.Equals(CurrentLanguageCode))
            {
                CurrentSystemLanguage = SystemLanguage.Unknown;
                foreach (var item in LocalizationConst.LanguageCodeMap)
                {
                    if (item.Value.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentSystemLanguage = item.Key;
                        break;
                    }
                }
                _languageDataMap = LanguageDataReader.ReadLanguageData(languageCode);
                CurrentLanguageCode = languageCode;
                IsLanguageSetted = true;
                LanguageChanged?.Invoke();
            }
        }

        public static string Localize(string localizationKey)
        {
            if (!IsLanguageSetted)
            {
                return null;
            }

            if (!_languageDataMap.ContainsKey(localizationKey))
            {
                Debug.LogWarning($"{CurrentLanguageCode} Localization key not found: {localizationKey}");
                return NotLocalizedString;
            }
            return _languageDataMap[localizationKey].TranslatedText;
        }

        public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }
    }
}



