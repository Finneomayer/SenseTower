using Mechanics.Network.Scripts.SpaceObjectsService;

namespace Mechanics.VideoService.Models
{
    public class VideoSpaceObject : SpaceObject<VideoTowerObject>
    {
        /// <summary>
        /// Номер позиции видео в пространстве.
        /// </summary>
        public int? PlaceNumber { get; set; }
    }
}