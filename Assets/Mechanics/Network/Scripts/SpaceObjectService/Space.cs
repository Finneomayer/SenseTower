using System;
using System.Collections;
using System.Collections.Generic;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public class Space
    {
        public Guid Id { get; set; }
        public virtual Dictionary<string, SpaceObject> Objects { get; set; } = new();
    }
}
