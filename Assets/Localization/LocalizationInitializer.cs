using System.Collections;
using UnityEngine;

namespace Assets.Localization
{
    /// <summary>
    /// Add this script to scene to initialize localization engine
    /// </summary>
    public class LocalizationInitializer : MonoBehaviour
    {
        private const string LanguageCodePlayerPrefsKey = "languageCode";

        private void OnEnable()
        {            
            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        private void Awake()
        {
            if (LocalizationManager.IsLanguageSetted)
            {
                return;
            }

            if (PlayerPrefs.HasKey(LanguageCodePlayerPrefsKey))
            {
                LocalizationManager.SetLanguage(PlayerPrefs.GetString(LanguageCodePlayerPrefsKey));
            }
            else
            {
                LocalizationManager.SetDefaultLanguage();
            }
        }

        private void OnLanguageChanged()
        {
            PlayerPrefs.SetString(LanguageCodePlayerPrefsKey, LocalizationManager.CurrentLanguageCode);
        }
    }
}



