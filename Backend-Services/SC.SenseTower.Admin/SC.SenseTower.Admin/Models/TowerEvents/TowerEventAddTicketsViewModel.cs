namespace SC.SenseTower.Admin.Models.TowerEvents
{
    public class TowerEventAddTicketsViewModel
    {
        public Guid EventId { get; set; }

        public int Quantity { get; set; } = 10;
    }
}
