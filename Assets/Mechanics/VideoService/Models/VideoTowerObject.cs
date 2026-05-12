using System;
using Mechanics.Network.Scripts.SpaceObjectsService;

namespace Mechanics.VideoService.Models
{
    public class VideoTowerObject : TowerObject
    {
        /// </summary>
        public Guid VideoRecordId { get; set; }

        /// <summary>
        /// Объект видео, содержащий данные о видео.
        /// </summary>
        public Video VideoRecord { get; set; }

        public PlayerSettings PlayerSettings { get; set; }
    }

    public class PlayerSettings
    {
        public int AccessMode = 0;
        public bool AutoPlay = false;
        public bool DefaultMuted = false;
        public bool EnableTextToSpeech = true;
    }
}