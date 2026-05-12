using UnityEngine;
using TMPro;
using Assets.Scripts.News;

namespace UI
{
    public sealed class TowerNewsViewItem : TowerMonitorPanelViewItem
    {
        [SerializeField]
        private TMP_Text DateTimeText;
        [SerializeField]
        private TMP_Text DescriptionText;

        public void Init(TowerNews towerNews)
        {
            DateTimeText.text = GetEventDateTimeString(towerNews.Date);
            DescriptionText.text = towerNews.Description;

            InitImage(towerNews.ImageUrl);
        }
    }
}
