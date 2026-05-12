using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Shared
{
    public abstract class BaseNetworkTransformTransmitter : NetworkBehaviour
    {
        private const float MinDeltaForDifference = 0.0001f;

        [SerializeField] protected Transform TargetTransform;
        [SerializeField] private float PositionThreshold = 0.01f;
        [SerializeField, Range(0.001F, 360)] private float RotationThreshold = 1f;
        [SerializeField] private float ScaleThreshold = 0.01f;
        [SerializeField] private float HeartbeatPeriodInSec = 1;
        [SerializeField] private bool Interpolate = true;
        [SerializeField] private float InterpolationSpeed = 10;

        private float _positionThresholdSqr;
        private float _scaleThresholdSqr;
        private Coroutine _interpolateTransformRoutine;
        private Vector3 _lastReceivedPosition;
        private Quaternion _lastReceivedRotation;
        private Vector3 _lastReceivedScale;

        private Coroutine _movingCoroutine;
        private bool _isSpawned = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (TargetTransform == null)
            {
                TargetTransform = transform;
            }

            if (!IsClient)
            {
                return;
            }

            _positionThresholdSqr = PositionThreshold * PositionThreshold;
            _scaleThresholdSqr = ScaleThreshold * ScaleThreshold;
            if (IsOwner)
                StartMoving();

            _isSpawned = true;
        }

        public override void OnNetworkDespawn()
        {
            StopAllCoroutines();
            base.OnNetworkDespawn();
        }

        private void FixedUpdate()
        {
            if (_isSpawned)
            {
                if (IsOwner && _movingCoroutine == null) StartMoving();
                else if (!IsOwner && _movingCoroutine != null) StopMoving();
            }
        }

        public void StartMoving()
        {
            StopMoving();
            _movingCoroutine = StartCoroutine(TransferRoutine());
        }

        public void StopMoving()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }

            if (_interpolateTransformRoutine != null)
            {
                StopCoroutine(_interpolateTransformRoutine);
                _interpolateTransformRoutine = null;
            }
        }

        protected abstract Vector3 GetTargetPosition();
        protected abstract void SetTargetPosition(Vector3 position);
        protected abstract Quaternion GetTargetRotation();
        protected abstract void SetTargetRotation(Quaternion rotation);
        protected abstract Vector3 GetTargetScale();
        protected abstract void SetTargetScale(Vector3 scale);

        private IEnumerator TransferRoutine()
        {
            WaitForSeconds checkPeriodWaitForSeconds = new(1f / NetworkManager.NetworkConfig.TickRate);

            float lastSendedTime = float.MinValue;
            Vector3 lastSendedPosition = GetTargetPosition();
            Quaternion lastSendedRotation = GetTargetRotation();
            Vector3 lastSendedScale = GetTargetScale();

            while (true)
            {
                if (IsNetworkRefreshNeeded(lastSendedTime, lastSendedPosition, lastSendedRotation, lastSendedScale))
                {
                    lastSendedTime = Time.unscaledTime;
                    lastSendedPosition = GetTargetPosition();
                    lastSendedRotation = GetTargetRotation();
                    lastSendedScale = GetTargetScale();

                    TransferTransformDataServerRpc(lastSendedPosition, lastSendedRotation, lastSendedScale);
                }

                yield return checkPeriodWaitForSeconds;
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
        private void TransferTransformDataServerRpc(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            SetTargetPosition(position);
            SetTargetRotation(rotation);
            SetTargetScale(scale);
            TransferTransformDataClientRpc(position, rotation, scale);
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void TransferTransformDataClientRpc(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!IsOwner)
            {
                SetReceivedTransformData(position, rotation, scale);
            }
        }

        private bool IsNetworkRefreshNeeded(float lastSendedTime, Vector3 lastSendedPosition,
            Quaternion lastSendedRotation, Vector3 lastSendedScale)
        {
            if (Time.unscaledTime - lastSendedTime > HeartbeatPeriodInSec)
            {
                return true;
            }

            if (Vector3.SqrMagnitude(lastSendedPosition - GetTargetPosition()) > _positionThresholdSqr)
            {
                return true;
            }

            if (Quaternion.Angle(lastSendedRotation, GetTargetRotation()) > RotationThreshold)
            {
                return true;
            }

            if (Vector3.SqrMagnitude(lastSendedScale - GetTargetScale()) > _scaleThresholdSqr)
            {
                return true;
            }

            return false;
        }

        private bool IsApproximately(Vector3 position1, Vector3 position2, float minDelta)
        {
            return (Mathf.Abs(position1.x - position2.x) < minDelta
                    && Mathf.Abs(position1.y - position2.y) < minDelta
                    && Mathf.Abs(position1.z - position2.z) < minDelta);
        }

        private bool IsApproximately(Quaternion rotation1, Quaternion rotation2, float minDelta)
        {
            return Quaternion.Angle(rotation1, rotation2) < minDelta;
        }

        private void SetReceivedTransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _lastReceivedPosition = position;
            _lastReceivedRotation = rotation;
            _lastReceivedScale = scale;

            if (IsApproximately(GetTargetPosition(), _lastReceivedPosition, MinDeltaForDifference)
                && IsApproximately(GetTargetRotation(), _lastReceivedRotation, MinDeltaForDifference)
                && IsApproximately(GetTargetScale(), _lastReceivedScale, MinDeltaForDifference))
            {
                return;
            }

            if (Interpolate)
            {
                if (_interpolateTransformRoutine == null)
                {
                    _interpolateTransformRoutine = StartCoroutine(InterpolateTransformRoutine());
                }
            }
            else
            {
                ApplyTransformParameters(_lastReceivedPosition, _lastReceivedRotation, _lastReceivedScale);
            }
        }

        private IEnumerator InterpolateTransformRoutine()
        {
            while (!IsApproximately(GetTargetPosition(), _lastReceivedPosition, MinDeltaForDifference)
                   || !IsApproximately(GetTargetRotation(), _lastReceivedRotation, MinDeltaForDifference)
                   || !IsApproximately(GetTargetScale(), _lastReceivedScale, MinDeltaForDifference))
            {
                ApplyTransformParameters(
                    Vector3.Lerp(GetTargetPosition(), _lastReceivedPosition, Time.deltaTime * InterpolationSpeed),
                    Quaternion.Lerp(GetTargetRotation(), _lastReceivedRotation,
                        Time.deltaTime * InterpolationSpeed),
                    Vector3.Lerp(GetTargetScale(), _lastReceivedScale, Time.deltaTime * InterpolationSpeed));

                yield return null;
            }

            ApplyTransformParameters(_lastReceivedPosition, _lastReceivedRotation, _lastReceivedScale);

            _interpolateTransformRoutine = null;
        }

        private void ApplyTransformParameters(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            SetTargetPosition(position);
            SetTargetRotation(rotation);
            SetTargetScale(scale);
        }
    }
}