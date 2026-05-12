using Sense.RemouteScene;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sense.RemouteScene
{
    public class ClientPathFolower : MonoBehaviour
    {
        [SerializeField]
        private PhysicsButton _buttonMidle;
        [SerializeField]
        private PhysicsButton _buttonLong;
        [SerializeField]
        private GameObject child;
        [SerializeField]
        private AudioSource _audio;

        [SerializeField]
        private GameObject[] _xrObjectsToEnable;

        private PathFollower _serverPathFollower;
        private XRInteractionManager _xrInteractionManager;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_serverPathFollower == null)
                _serverPathFollower = FindObjectOfType<PathFollower>();

            if (_serverPathFollower == null)
            {
                child.SetActive(false);
                return;
            }
            child.SetActive(true);
            transform.parent = _serverPathFollower.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _buttonMidle.OnPressed.AddListener(() => _serverPathFollower.ResetDistanceServerRpc(PathFollower.SpeedMode.Fast));
            _buttonLong.OnPressed.AddListener(() => _serverPathFollower.ResetDistanceServerRpc(PathFollower.SpeedMode.Slow));

            _serverPathFollower.OnFinish += FinishDrive;
            _serverPathFollower.OnStart += StartDrive;

            if (_serverPathFollower.IsDrive.Value) StartDrive();
        }

        private void Update()
        {
            if (_serverPathFollower == null)
                Init();

            if (_xrInteractionManager != null)
            {
                return;
            }

            _xrInteractionManager = FindObjectOfType<XRInteractionManager>();
            if (_xrInteractionManager != null)
            {
                foreach (var item in _xrObjectsToEnable)
                {
                    item.SetActive(true);
                }
            }
        }

        private void StartDrive()
        {
            _audio.Play();
        }

        private void FinishDrive()
        {
            _audio.Stop();
        }

        private void OnDestroy()
        {
            _buttonMidle.OnPressed.RemoveListener(() => _serverPathFollower.ResetDistanceServerRpc(PathFollower.SpeedMode.Fast));
            _buttonLong.OnPressed.RemoveListener(() => _serverPathFollower.ResetDistanceServerRpc(PathFollower.SpeedMode.Slow));
        }
    }
}