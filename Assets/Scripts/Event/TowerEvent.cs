using System;
using Assets.Scripts.Space;

namespace Assets.Scripts.Event
{
    /// <summary>
    /// Событие в башне
    /// </summary>
    public sealed class TowerEvent
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        //public DateTimeOffset Date { get; set; }
        // public DateTimeOffset From { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string ImageUrl { get; set; }

        public LocalSpace Space { get; set; }

        public TowerEventState State { get; set; }

        public Ticket[] SoldTickets { get; set; }

        public int TotalTickets { get; set; }

        public int Sold { get; set; }

        public bool IsFreeEvent()
        {
            return TotalTickets <= 0;
        }
    }
}
