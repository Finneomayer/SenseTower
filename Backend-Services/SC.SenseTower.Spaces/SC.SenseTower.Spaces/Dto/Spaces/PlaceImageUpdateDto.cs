namespace SC.SenseTower.Spaces.Dto.Spaces
{
    public class PlaceImageUpdateDto
    {
        /// <summary>
        /// Индекс расположения в помещении.
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }
    }
}
