using System;
using System.Collections.Generic;
using Assets.Scripts.Space;
using UnityEditor;

namespace Assets.Scripts.Gallery
{
    public class Gallery
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public GalleryInfoTable GalleryInfoTable { get; set; }
        public LocalSpace Space { get; set; }
        // Это есть только на беке, в юнити это ПОКА не нужно
        //public TowerPicture[] Pictures { get; set; }
        public Dictionary<int, TowerPicture> PicturesLocation { get; set; }
    }
}