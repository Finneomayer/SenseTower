using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Localization
{
    public static class LocalizationConst
    {
        public static readonly Dictionary<SystemLanguage, string> LanguageCodeMap = new()
        {
            [SystemLanguage.English] = "en",
            [SystemLanguage.Russian] = "ru",
        };

        public static readonly Dictionary<SystemLanguage, CultureInfo> LanguageCultureInfoMap = new()
        {
            [SystemLanguage.English] = CultureInfo.GetCultureInfo("en-US"),
            [SystemLanguage.Russian] = CultureInfo.GetCultureInfo("ru-RU"),
        };
        //don't forged to check langauge in SetDefaultLanguage of LocalizationManager.cs
    }
}



