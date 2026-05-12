using System;
using System.Collections.Generic;
using Assets.Scripts.TowerObjectsRevision.Models;
using JetBrains.Annotations;
using Mechanics.LoadSceneObjects.Models;
using Mono.CSharp.Linq;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class SpaceObject
    {
        public TowerObject TowerObject { get; set; }
        public Guid TowerObjectId { get; set; }
        public Vectors Vectors { get; set; }
        public bool IsFixed { get; set; }
        public bool IsActive { get; set; }
        public string HelpContent { get; set; }
        public string TempRelatedObjectId { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }

    public class SpaceObject<T> : SpaceObject where T : TowerObject
    {
        public new T TowerObject
        {
            get => (T) base.TowerObject;
            set => base.TowerObject = value;
        }
    }
}