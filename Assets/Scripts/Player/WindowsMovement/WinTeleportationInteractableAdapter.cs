using Sense.Interectable.Teleportation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player.WindowsMovement
{
    public class WinTeleportationInteractableAdapter : MonoBehaviour, IPointerClickHandler
    {
        private WinClientTeleport _winClientTeleport;
        private CustomTeleportationInteractable _teleportationInteractable;

        public void Init(WinClientTeleport winClientTeleport, CustomTeleportationInteractable teleportationInteractable)
        {
            _winClientTeleport = winClientTeleport;
            _teleportationInteractable = teleportationInteractable;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_winClientTeleport == null)
            {

            }
            if (_winClientTeleport != null && _teleportationInteractable != null)
            {
                _winClientTeleport.TeleportPlayer(_teleportationInteractable.TeleportAnchorTransform);
            }
        }
    }
}
