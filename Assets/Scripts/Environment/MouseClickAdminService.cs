using System;
using Assets.Scripts.Environmental.Presentation.Browser;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

namespace Assets.Scripts.Environmental
{
    [RequireComponent(typeof(Collider))]
    public class MouseClickAdminService : PlaceAdminService, IPointerClickHandler
    {
        #region Inspector
        [SerializeField] private GameObject logo;
        #endregion
        
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

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSetAdmin(NetworkManager.Singleton.LocalClientId);
        }
    }
}