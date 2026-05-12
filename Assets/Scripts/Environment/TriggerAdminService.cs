using System;
using Assets.Scripts.Environmental.Presentation.Browser;
using UnityEngine;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

namespace Assets.Scripts.Environmental
{
    [RequireComponent(typeof(Collider))]
    public class TriggerAdminService : PlaceAdminService
    {
        #region Inspector
        [SerializeField] private GameObject logo;
        #endregion
        private void OnTriggerEnter(Collider other)
        {
            var playerObject = other.GetComponent<Camera>();
            ulong clientId = 0;

            if (playerObject != null)
                clientId = playerObject.transform.GetComponentInParent<NetworkPlayer>().OwnerClientId;
            
            if(clientId !=0)
                OnSetAdmin(clientId);
        }


        private void OnTriggerExit(Collider other)
        {
            var playerObject = other.GetComponent<Camera>();
            ulong clientId = 0;

            if (playerObject != null)
                clientId = playerObject.transform.GetComponentInParent<NetworkPlayer>().OwnerClientId;
            
            if(clientId !=0)
                OnClearAdmin(clientId);
        }

        public override void Deactivate()
        {
            logo.SetActive(false);
            gameObject.SetActive(false);
        }

        public override void Activate()
        {
            logo.SetActive(true);
            gameObject.SetActive(true);
        }

        public override void Show()
        {
            logo.SetActive(true);
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            logo.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}