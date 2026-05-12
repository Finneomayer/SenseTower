using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectTransform : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<ObjectNetworkData> _networkObjectTransform = new NetworkVariable<ObjectNetworkData>(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private float _interpolationTime = 0.1f;

    private Vector3 _vel;
    private float _rotVel;

    private void Update()
    {
        if (IsOwner)
        {
            _networkObjectTransform.Value = new ObjectNetworkData
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };
        }
        else 
        {
            transform.position = Vector3.SmoothDamp(transform.position, _networkObjectTransform.Value.Position, ref _vel, _interpolationTime);
            
            Vector3 currentNetworkRotation = _networkObjectTransform.Value.Rotation;
            transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, currentNetworkRotation, ref _vel, _interpolationTime));

            //transform.rotation = Quaternion.Euler(
            //    Mathf.SmoothDampAngle(transform.rotation.x,currentNetworkRotation.x, ref _rotVel,_interpolationTime),
            //    Mathf.SmoothDampAngle(transform.rotation.y,currentNetworkRotation.y, ref _rotVel,_interpolationTime),
            //    Mathf.SmoothDampAngle(transform.rotation.z,currentNetworkRotation.z, ref _rotVel,_interpolationTime) 
            //    );
        }
    }

    struct ObjectNetworkData : INetworkSerializable
    {
        private float _x, _y, _z;
        private float _xRot, _yRot, _zRot;

        internal Vector3 Position 
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
            }
        }
        internal Vector3 Rotation
        {
            get => new Vector3(_xRot, _yRot, _zRot);
            set
            {
                _xRot = value.x;
                _yRot = value.y;
                _zRot = value.z;
            }
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _y);
            serializer.SerializeValue(ref _z);

            serializer.SerializeValue(ref _xRot);
            serializer.SerializeValue(ref _yRot);
            serializer.SerializeValue(ref _zRot);
        }
    }
}
