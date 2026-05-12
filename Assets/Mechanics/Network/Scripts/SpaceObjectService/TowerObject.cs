using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.TowerObjectsClass.Models;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class TowerObject
    {
        public Guid Id { get; set; }

        public TowerObjectClass ObjectClass { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Owner? Owner { get; set; }

        public Dictionary<string, string>? Data { get; set; }

        public string? HelpId { get; set; }

        public string? HelpContent { get; set; }

        /// <summary>
        /// Где расположен объект
        /// </summary>
        public Location? Location { get; set; }
    }
}