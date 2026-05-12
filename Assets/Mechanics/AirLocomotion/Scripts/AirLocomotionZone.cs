using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    public class AirLocomotionZone : MonoBehaviour
    {
        [field: SerializeField]
        public BoxCollider Collider { get; private set; }
    }
}