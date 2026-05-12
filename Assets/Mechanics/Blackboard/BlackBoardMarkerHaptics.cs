using UnityEngine;

namespace Assets.Blackboard
{
    public class BlackBoardMarkerHaptics : MonoBehaviour
    {
        [SerializeField]
        private BlackBoardMarker Marker;
        [SerializeField]
        private float VibratinoIntensity;

        private void Update()
        {
            if (Marker.IsDrawing && Marker.PlayerController != null 
                && Marker.PlayerController.GrabInteractor != null)
            {
                Marker.PlayerController.GrabInteractor.SendHapticImpulse(VibratinoIntensity, 0.1f);
            }
        }
    }
}
