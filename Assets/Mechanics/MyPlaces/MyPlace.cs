using System;
using System.Collections.Generic;
using Assets.Scripts.Space;

namespace Assets.Scripts.Hall
{
    [Serializable]
    public sealed class MyPlace
    {
        //public Guid Id { get; set; } = default;

        //public int Number { get; set; } = default;

        //public string PlaceName { get; set; } = null!;

        //public Guid? OwnerId { get; set; }

        //public string OwnerName { get; set; }

        //public MyImage DoorImage { get; set; }

        //public Dictionary<int, MyImage> Images { get; set; }

        public LocalSpace LocalSpace { get; set; }

        //public SpaceAccessType PublicAccessType { get; set; }
    }
}
