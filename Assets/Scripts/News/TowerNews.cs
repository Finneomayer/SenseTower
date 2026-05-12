using System;

namespace Assets.Scripts.News
{
    public class TowerNews
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}