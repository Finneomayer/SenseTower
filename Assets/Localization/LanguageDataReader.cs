using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Localization
{
    /// <summary>
    /// Parses from csv to Dictionary structure
    /// </summary>
    public static class LanguageDataReader
    {
        private static Dictionary<string, LanguageItem> Dictionary;

        /// <summary>
        /// Returns pairs key-value of selected language
        /// </summary>
        /// <param name="languageCode">format is "ru".."en"</param>
        /// <returns></returns>
        public static Dictionary<string, LanguageItem> ReadLanguageData(string languageCode)
        {
            Dictionary = new Dictionary<string, LanguageItem>();
            //array [x,y] where x - rows, y - columns
            string text = OpenCSV(languageCode);            

            Read(text);

            return Dictionary;
        }

        private static string OpenCSV(string languageCode)
        {
            string textFile;
            try
            {
                textFile = Resources.Load($"LocalizationData/localization_{languageCode}").ToString();
            }
            catch (System.Exception)
            {
                textFile = Resources.Load($"LocalizationData/localization_en").ToString();
            }
            
            return textFile;
        }        

        /// <summary>
		/// Read localization spreadsheets.
		/// </summary>
		public static void Read(string textAsset)
        {
            var text = textAsset.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");
            var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");

            foreach (Match match in matches)
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[_comma_]").Replace("\n", "[_newline_]"));
            }

            // Making uGUI line breaks to work in asian texts.
            //text = text.Replace("?", "? ").Replace("?", "? ").Replace("?", "? ").Replace("?", "? ").Replace("?", " ?").Replace("?", "? ").Trim();

            var lines = text.Split('\n').Where(i => i != "").ToList();

            for (var i = 1; i < lines.Count; i++)
            {
                var columns = lines[i].Split(',').Select(j => j.Trim()).Select(j => j.Replace("[_quote_]", "\"").Replace("[_comma_]", ",").Replace("[_newline_]", "\n")).ToList();
                var key = columns[0];
                if (key == "") continue;
                                
                Dictionary.Add(key, new LanguageItem() { TranslatedText = columns[2] });                
            }

            //foreach (var item in Dictionary)
            //{
            //    Debug.LogWarning($"{item.Key} --  {item.Value.TranslatedText}");
            //}
        }
    }
}