namespace SC.SenseTower.Spaces.Dto.Places
{
    public class PlaceInfoDto
    {
        public Guid Id { get; set; }

        public string PlaceName { get; set; } = null!;

        public int PlaceNumber { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }

        public string? DoorImageUrl { get; set; }
    }
}
