using System.Collections;
using Assets.Scripts.Environmental.Presentation.Browser;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Environmental
{
    public class XrInteractionAdminService : PlaceAdminService
    {
        [SerializeField] private XRSimpleInteractable _xrInteractable;

        private bool _isInteract;
        private bool _isBlocked = false;
        private Coroutine _blockCoroutine;

        private void OnEnable()
        {
            _isInteract = false;
            if (_xrInteractable != null)
                SetInteractableObject(_xrInteractable.gameObject);
        }

        public override void SetInteractableObject(GameObject interactionGameObject) 
        {
            if (interactionGameObject.TryGetComponent(out XRSimpleInteractable xrInteractable))
            {
                _xrInteractable = xrInteractable;
                _xrInteractable.selectExited.AddListener(delegate { Toggle(); });
                _xrInteractable.firstHoverEntered.AddListener((e) => UnblockDelayed());
                _xrInteractable.lastHoverExited.AddListener((e) => Block());
                
                Activate();
            }
        }

        private void Block()
        {
            if (_blockCoroutine != null) StopCoroutine(_blockCoroutine);
            _isBlocked = true;
        }

        private void UnblockDelayed()
        {
            if (_blockCoroutine != null) StopCoroutine(_blockCoroutine);
            _blockCoroutine = StartCoroutine(BlockCoroutine());
        }

        private IEnumerator BlockCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
            _isBlocked = false;
        }

        private void OnDisable()
        {
            if (_xrInteractable != null)
            {
                _xrInteractable.selectExited.RemoveAllListeners();
                _xrInteractable.firstHoverEntered.RemoveAllListeners();
                _xrInteractable.lastHoverExited.RemoveAllListeners();
            }
        }

        public override void Show()
        {
            _isInteract = false;
            if(_xrInteractable != null)
                _xrInteractable.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            if (_xrInteractable != null)
                _xrInteractable.gameObject.SetActive(false);
        }

        public override void Activate()
        {
            _isInteract = false;
            if(_xrInteractable != null)
                _xrInteractable.gameObject.SetActive(true);
            BrowserDeactivate?.Invoke();
        }

        public override void Deactivate()
        {
            _isInteract = false;
            if (_xrInteractable != null)
                _xrInteractable.gameObject.SetActive(false);
            BrowserActivate?.Invoke();
        }

        [ContextMenu("Toggle")]
        private void Toggle()
        {
            if (!_xrInteractable.isHovered || _isBlocked) return;

            _isInteract = !_isInteract;
            Debug.Log($"Send interact with local client id = {NetworkManager.Singleton.LocalClientId}");
            if (_isInteract)
            {
                OnSetAdmin(NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                OnClearAdmin(NetworkManager.Singleton.LocalClientId);
            }
        }
    }
}