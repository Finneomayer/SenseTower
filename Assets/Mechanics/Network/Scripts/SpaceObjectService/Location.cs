using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    /// <summary>
    /// В какой сущности находится объект
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Тип расположения
        /// </summary>
        public LocationType Type { get; set; }

        /// <summary>
        /// Идентификатор объекта расположения
        /// </summary>
        public Guid? LocationObjectId { get; set; }
    }
    public enum LocationType
    {
        /// <summary>
        /// В мегахранилище Sense - это когда предмет ничей и нигде не лоццирован
        /// </summary>
        SenseTower = 0,
        /// <summary>
        /// Предмет в пространстве
        /// </summary>
        Space = 1,
        /// <summary>
        /// Предмет на складе магазина
        /// </summary>
        Shop = 2,
        /// <summary>
        /// В рюкзаке пользователя
        /// </summary>
        User = 3
    }
}
