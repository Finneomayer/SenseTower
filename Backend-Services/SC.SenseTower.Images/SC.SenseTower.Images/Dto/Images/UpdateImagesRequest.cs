namespace SC.SenseTower.Images.Dto.Images
{
    public class UpdateImagesRequest
    {
        public Guid? PlaceId { get; set; }

        public UpdateImagesDto[] UpdateImages { get; set; }
    }
}
