using UnityEngine;

namespace Assets.Blackboard
{
    public class BlackBoardEraserHaptics : MonoBehaviour
    {
        [SerializeField]
        private BlackBoardEraser Eraser;
        [SerializeField]
        private float VibratinoIntensity;

        private void Update()
        {
            if (Eraser.IsDrawing)
            {
                Eraser.PlayerControllerInteractor.SendHapticImpulse(VibratinoIntensity, 0.1f);
            }
        }
    }
}
