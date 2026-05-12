using UnityEngine;
using PathCreation;
using Unity.Netcode;
using System;

namespace Sense.RemouteScene
{
    public class PathFollower : NetworkBehaviour
    {
        [SerializeField]
        private PathCreator _pathCreator;
        [SerializeField]
        private EndOfPathInstruction _endOfPathInstruction;
        [SerializeField]
        private float _speedLong = 5;
        [SerializeField]
        private float _speedFast = 5;
        float distanceTravelled = float.MaxValue;


        public Action OnStart;
        public Action OnFinish;

        public NetworkVariable<bool> IsDrive;
        private float _currentSpeed = 0;

        public void Init(PathCreator pathCreator)
        {
            Debug.Log("Sally INIT");
            _pathCreator = pathCreator;
            _pathCreator.path.OnDriveEndPath += EndPath;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer) return;

            if (IsDrive.Value)
                StartDriveEventClientRpc();
        }

        public enum SpeedMode
        {
            Fast,
            Slow
        }

        private void EndPath()
        {
            EndFollowServer();
            EndDriveEventClientRpc();

        }

        private void Update()
        {
            if (IsServer && IsDrive.Value)
                UpdatePath();
        }

        #region Server
        private void UpdatePath()
        {
            if (_pathCreator != null)
            {
                distanceTravelled += _currentSpeed * Time.deltaTime;
                transform.position = _pathCreator.path.GetPointAtDistance(distanceTravelled, _endOfPathInstruction);
                var rot = _pathCreator.path.GetRotationAtDistance(distanceTravelled, _endOfPathInstruction);
                rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0);
                transform.rotation = rot;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ResetDistanceServerRpc(SpeedMode mode)
        {
         
            if (IsDrive.Value == false && _pathCreator != null && (distanceTravelled / _pathCreator.path.GetLenght() >= 1 || distanceTravelled == 0))
            {
                _currentSpeed = mode == SpeedMode.Fast ? _speedFast : _speedLong;
                StartFollowServerRpc();
                StartDriveEventClientRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartFollowServerRpc()
        {
            IsDrive.Value = true;
            distanceTravelled = 0;
        }


        private void EndFollowServer() => IsDrive.Value = false;
        #endregion

        #region Client

        [ClientRpc]
        public void EndDriveEventClientRpc() => OnFinish?.Invoke();

        [ClientRpc]
        public void StartDriveEventClientRpc() => OnStart?.Invoke();
        #endregion
    }
}