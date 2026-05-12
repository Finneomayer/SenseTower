using System;

namespace Mechanics.VideoService.Models
{
    public class Video
    {
        /// <summary>
        /// Идентификатор видео.
        /// </summary>
        public Guid Id { get; set; }
    
        /// <summary>
        /// Название видео.
        /// </summary>
        public string Name { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public int Duration { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    
        /// <summary>
        /// Описание видео.
        /// </summary>
        public string? Description { get; set; }
    
        /// <summary>
        /// Ссылка на видео.
        /// </summary>
        public string VideoUrl { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
    }

}