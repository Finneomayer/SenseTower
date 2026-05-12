namespace Assets.Mechanics.Browser
{
    public interface IWebPageActionsHandler
    {
        WebPageAction GetAvailableActions(string url);

        void ProcessAction(WebPageAction webPageAction);
    }
}