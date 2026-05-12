using UnityEngine;

namespace Assets.Mechanics.Doors
{
    public class DoorFrameView : MonoBehaviour
    {
        [SerializeField] private Color _greenDoorColor;
        [SerializeField] private Color _goldDoorColor;
        [SerializeField] private MeshRenderer _doorModel;
        [SerializeField] private MeshRenderer _doorModel2;

        public void SetDoorGreen()
        {
            _doorModel.material.color = _greenDoorColor;
            _doorModel2.material.color = _greenDoorColor;
        }

        public void SetDoorGold()
        {
            _doorModel.material.color = _goldDoorColor;
            _doorModel2.material.color = _goldDoorColor;
        }
    }
}
