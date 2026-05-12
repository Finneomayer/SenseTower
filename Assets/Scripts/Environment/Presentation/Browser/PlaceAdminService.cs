using System;
using UnityEngine;

namespace Assets.Scripts.Environmental.Presentation.Browser
{
    public class PlaceAdminService : MonoBehaviour
    {
        public Action BrowserActivate;
        public Action BrowserDeactivate;

        public event Action<ulong> SetAdmin;
        public event Action<ulong> ClearAdmin;
        
        public virtual void SetInteractableObject(GameObject interactionGameObject) { }

        protected void OnSetAdmin(ulong clientId)=> SetAdmin?.Invoke(clientId);
        protected void OnClearAdmin(ulong clientId) => ClearAdmin?.Invoke(clientId);
        public virtual void Show() { }
        public virtual void Hide() { }
        public virtual void Deactivate() { }
        public virtual void Activate(){}
    }
}