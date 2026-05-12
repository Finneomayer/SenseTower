using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class PictureSpaceObject : SpaceObject<PictureTowerObject>
    {
        public int? PlaceNumber { get; set; }
    }
}
