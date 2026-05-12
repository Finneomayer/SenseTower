using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static Data.Enumenators;

namespace Mechanics.LoadSceneObjects.Models
{
    public class StaticObject
    {
        public Guid TowerObjectId { get; set; }
        public RemoteContentType RemoteObjectType { get; set; } = RemoteContentType.NetworkPrefab;
        public PrefabObjectType PrefabObjectType { get; set; }
        public string ObjectKey { get; set; }
        public string RepositoryUrl { get; set; }
        public int? PlaceNumber { get; set; }

        public Vectors Vectors { get; set; }
        public float LeftTopX { get; set; }
        public float LeftTopY { get; set; }
        public float LeftTopZ { get; set; }
        public float RightDownX { get; set; }
        public float RightDownY { get; set; }
        public float RightDownZ { get; set; }
        public string TempRelatedObjectId { get; set; }
        public string HelpContent  { get; set; }
        public bool IsActive { get; set; }

        public Dictionary<string, string> Data { get; set; }

    }
}