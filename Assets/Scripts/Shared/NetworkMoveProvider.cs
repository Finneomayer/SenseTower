using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Shared
{
    public class NetworkMoveProvider : ActionBasedContinuousMoveProvider
    {
        [SerializeField]
        public bool EnableInputActions;

        protected override Vector2 ReadInput()
        {
            return EnableInputActions ? base.ReadInput() : Vector2.zero;
        }
    }
}