using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Mechanics.Browser
{
    public static class YoutubeLinkFunctions
    {
        private const string YoutubeLinkRegex = "(?:https?:\\/\\/)?(?:www\\.)?youtu\\.?be(?:\\.com)?\\/?.*(?:watch|embed)?(?:.*v=|v\\/|\\/)([\\w\\-_]+)\\&?";

        public static bool IsYoutubeUrl(string input)
        {
            if (GetVideoId(input) != string.Empty)
            {
                return true;
            }
            string cleanInput = input;

            if (cleanInput.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = input.Replace("http://", "", System.StringComparison.OrdinalIgnoreCase);
            }
            else if (cleanInput.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = input.Replace("https://", "", System.StringComparison.OrdinalIgnoreCase);
            }

            if (cleanInput.StartsWith("www.", System.StringComparison.OrdinalIgnoreCase))
            {
                cleanInput = cleanInput.Replace("www.", "", System.StringComparison.OrdinalIgnoreCase);
            }

            int slashIndex = cleanInput.IndexOf('/');
            if (slashIndex >= 0)
            {
                cleanInput = cleanInput.Substring(0, slashIndex);
            }

            if (string.Equals(cleanInput, "youtube.com", System.StringComparison.OrdinalIgnoreCase) 
                || string.Equals(cleanInput, "youtu.be", System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static string GetVideoId(string input)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(input))
            {
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }

            if (input.Contains("rutube"))
            {
                return "rutube";
            }

            return string.Empty;
        }
    }
}