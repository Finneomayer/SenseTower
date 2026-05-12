using Assets.Mechanics.NetworkInteraction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.Chess.Scripts
{
    public class ColliderSwitchWhileCatching : MonoBehaviour
    {
        [SerializeField] private NetworkXrGrab _xrGrab;
        private Collider[] _colliders;
        private Rigidbody[] _bodies;

        private void OnEnable()
        {
            _colliders = GetComponentsInChildren<Collider>();
            _bodies = GetComponentsInChildren<Rigidbody>();

            _xrGrab.StartGrab += OnGrab;
            _xrGrab.CurrentUserDrop += OnDrop; 
        }

        private void OnDisable()
        {
            _xrGrab.StartGrab -= OnGrab;
            _xrGrab.CurrentUserDrop -= OnDrop;
        }

        private void OnGrab()
        {
            
        }

        private void OnDrop()
        {
            
        }

        
    }
}
