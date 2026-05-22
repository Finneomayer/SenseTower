namespace SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate
{
    public class ImageInfoDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string PreviewUrl { get; set; } = null!;
    }
}
