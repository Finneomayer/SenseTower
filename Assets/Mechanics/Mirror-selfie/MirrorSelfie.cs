using Oculus.Avatar2;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.Mirror_selfie
{
    public class MirrorSelfie : NetworkBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private XRGrabInteractable _scalablePart;
        [SerializeField] private MirrorControlPanel _controlPanel;
        [Space]
        [SerializeField] private GameObject _mirrorOwnerVisual;
        [SerializeField] private GameObject _mirrorPhantom;


        public event Action CloseRequested;
        public XRGrabInteractable MirrorGrabable => _scalablePart;

        private NetworkVariable<bool> _isVisible = new();
        private RenderTexture _renderTexture;
        private const float StartFieldOfView = 60;
        private AvatarLODManager _avatarManager;


        private void Start()
        {
            if (!IsClient) return;
            
            _renderTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
            _renderTexture.Create();

            _camera.targetTexture = _renderTexture;
            _camera.Render();
            _rawImage.texture = _renderTexture;

            _avatarManager = FindObjectOfType<AvatarLODManager>();
            if (_avatarManager != null) _avatarManager.AddExtraCamera(_camera);

            if (IsOwner) SetMirrorModelVisible(true);
            else SetMirrorModelVisible(_isVisible.Value);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient)
            {
                return;
            }

            if (IsOwner) SetMirrorModelVisible(true);
            else SetMirrorModelVisible(false);

            _controlPanel.MirrorClickVisible += OnMirrorClickVisible;
            _controlPanel.MirrorCloseClick += OnMirrorClickClose;
            _isVisible.OnValueChanged += VisibleChangedFromServer;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient)
            {
                return;
            }
            _controlPanel.MirrorClickVisible -= OnMirrorClickVisible;
            _controlPanel.MirrorCloseClick -= OnMirrorClickClose;
            _isVisible.OnValueChanged -= VisibleChangedFromServer;

            Destroy(MirrorGrabable.gameObject);

            base.OnNetworkDespawn();
        }

        private void OnMirrorClickClose()
        {
            CloseRequested?.Invoke();
        }

        private void VisibleChangedFromServer(bool previousvalue, bool newvalue)
        {
            _controlPanel.SetVisibleButtonActive(newvalue);
            if (!IsOwner) SetMirrorModelVisible(newvalue);
        }

        private void OnMirrorClickVisible(bool isVisible)
        {
            SetVisibleServerRpc(isVisible);
        }

        private void Update()
        {
            if (!IsClient) return;
            if (_camera.isActiveAndEnabled)
                _camera.fieldOfView = StartFieldOfView + (_scalablePart.transform.localScale.x - 1) * StartFieldOfView / 2;
        }

        private void OnDisable()
        {
            if (!IsClient) return;
            _renderTexture.Release();
        }

        public void SetMirrorModelVisible(bool visible)
        {
            if (!IsClient) return;
            _mirrorOwnerVisual.SetActive(visible);
            _mirrorPhantom.SetActive(!visible);
            _controlPanel.GetComponent<Canvas>().enabled = visible & IsOwner;
            _scalablePart.enabled = visible & IsOwner;
        }

        [ServerRpc]
        private void SetVisibleServerRpc(bool isVisible)
        {
            _isVisible.Value = isVisible;
        }
    }
}
