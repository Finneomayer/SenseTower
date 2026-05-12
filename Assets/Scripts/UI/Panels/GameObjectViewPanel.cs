namespace UI
{
    public class GameObjectViewPanel : ViewPanel
    {
        public override void ShowPanel()
        {
            gameObject.SetActive(true);
            RaisePanelShownEvent();
        }

        public override void HidePanel()
        {
            gameObject.SetActive(false);
            RaisePanelHiddenEvent();
        }

        public override bool IsVisible()
        {
            return gameObject.activeSelf;
        }
    }
}
