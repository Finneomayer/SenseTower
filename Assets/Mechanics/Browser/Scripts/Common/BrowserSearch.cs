using System.Text.RegularExpressions;

namespace Assets.Mechanics.Browser
{
    public abstract class BrowserSearch
    {
        public abstract bool IsUrlValid(string input);
        public abstract string GetUrlForSearch(string input);
    }

    public sealed class YoutubeBrowserSearch : BrowserSearch
    {
        public override bool IsUrlValid(string input)
        {
            return YoutubeLinkFunctions.IsYoutubeUrl(input);
        }

        public override string GetUrlForSearch(string searchInput)
        {
            return $"https://www.youtube.com/results?search_query={searchInput}";
        }
    }

    public sealed class CommonBrowserSearch : BrowserSearch
    {
        public override bool IsUrlValid(string input)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(input);
        }

        public override string GetUrlForSearch(string searchInput)
        {
            return $"https://www.google.com/search?q={searchInput}";
        }
    }
}
