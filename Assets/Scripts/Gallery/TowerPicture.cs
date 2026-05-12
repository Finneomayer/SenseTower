using System;
using UnityEditor;

namespace Assets.Scripts.Gallery
{
    public class TowerPicture
    {
        public Guid Id { get; set; }

        public string Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public MyImage Image { get; set; }
        public decimal PictureWidthInMeters { get; set; }
        public decimal PassepartoutWidthInMeters { get; set; }
    }
}