using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Player.WindowsMovement
{
    public class WinClientTeleport : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Transform[] _anchors;
        private CompositionRootNetworkScene _compositionRoot;
        private PlayerLogic _player;
        private EditorMovementSystem _playerMover;

        private void Awake()
        {
            _compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
        }

        public void SetAnchors(Transform[] targets)
        {
            _anchors = targets;
        }

        private void Start()
        {
            GetPlayer();
        }

        public void TeleportPlayer(Transform teleportationPoint)
        {
            if (_playerMover == null)
                _playerMover = _player.GetComponent<EditorMovementSystem>();
            _playerMover.SetPosition(teleportationPoint.position, teleportationPoint.rotation);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogWarning("Click");
            TeleportPlayer();
        }

        private async void GetPlayer()
        {
            _player = await _compositionRoot.GetLocalPlayerAsync();
        }

        private void TeleportPlayer()
        {
            if (_anchors == null || _anchors.Length == 0)
            {
                return;
            }

            int i = Random.Range(0, _anchors.Length);
            TeleportPlayer(_anchors[i]);
        }
    }
}
