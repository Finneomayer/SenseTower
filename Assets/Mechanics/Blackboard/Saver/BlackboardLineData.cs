using UnityEngine;
using System;

namespace Assets.Blackboard
{
    [Serializable]
    public class BlackboardLineData
    {
        public Color Color;
        public Vector3[] Points = new Vector3[0];
    }
}
