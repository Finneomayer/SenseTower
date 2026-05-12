using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class Picture
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название изображения в галерее.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание изображения.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Автор изображения.
        /// </summary>
        public string Author { get; set; } = null!;

        /// <summary>
        /// Информация о файле изображения.
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        /// Ширина изображения в метрах для Unity.
        /// </summary>
        public decimal PictureWidthInMeters { get; set; }

        /// <summary>
        /// Ширина паспарту в метрах для Unity.
        /// </summary>
        public decimal MattingWidthInMeters { get; set; }
    }
}

