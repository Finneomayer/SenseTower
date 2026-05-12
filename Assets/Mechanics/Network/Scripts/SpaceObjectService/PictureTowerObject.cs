using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class PictureTowerObject : TowerObject
    {
        public Guid PictureId { get; set; }
        public Picture Picture { get; set; }
    }
}

