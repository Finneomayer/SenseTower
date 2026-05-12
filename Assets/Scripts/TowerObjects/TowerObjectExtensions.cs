using Assets.Localization;
using System;
using System.Linq;

namespace Assets.Scripts.TowerObjects
{
    public static class TowerObjectExtensions
    {
        public static string GetLocalizedName(this TowerObjectDto towerObject)
        {
            if (string.IsNullOrEmpty(towerObject.Description) || !LocalizationManager.IsLanguageSetted)
            {
                return towerObject.TowerObjectClassName;
            }

            string[] langDescriptions = towerObject.Description.Trim().Split("//");
            
            string currentLangDescription = langDescriptions.FirstOrDefault(
                x => x.StartsWith(LocalizationManager.CurrentLanguageCode, StringComparison.OrdinalIgnoreCase));
            
            if (string.IsNullOrEmpty(currentLangDescription))
            {
                currentLangDescription = langDescriptions[0];
            }

            string[] currentLangStrings = currentLangDescription.Split(":");
            if (currentLangStrings.Length != 2)
            {
                return currentLangStrings.Length < 2 ? towerObject.Description : towerObject.TowerObjectClassName;
            }

            return currentLangStrings[1];
        }
    }
}