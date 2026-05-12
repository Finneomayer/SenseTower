using Assets.Mechanics.NetworkInteraction;
using Assets.Scripts.Shared;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Mechanics.Chess.Scripts
{
    public enum ChessTeam
    {
        White,
        Black
    }

    [Serializable]
    public class ChessMesh
    {
        public GameObject Parent;
        public GameObject Mesh;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkTransformTransmitter))]
    [RequireComponent(typeof(NetworkXrGrab))]
    public class ChessPiece : NetworkBehaviour
    {
        [SerializeField] private ChessMesh _model;
        [SerializeField] private ChessTeam _color;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private NetworkXrGrab _xrGrab;
        [SerializeField] private PhysicMaterial _zeroFrictionMaterial;
        [SerializeField] private PhysicMaterial _standartMaterial;

        public event Action<Transform, ChessTeam> IsEaten;
        public ChessMesh Model => _model;
        public ChessTeam Team => _color;
        private float _counter;
        private bool _userIsOwner;
        private Material _material;
        private Vector3 _startMeshScale;
        private int _smoothCounter;

        private void Start()
        {
            _model.LocalPosition = _model.Mesh.transform.localPosition;
            _model.LocalRotation = _model.Mesh.transform.localRotation;
            _material = _model.Mesh.GetComponent<MeshRenderer>().material;
            _startMeshScale = _model.Mesh.transform.localScale;

            _xrGrab.StartGrab += StartGrab;
        }

        private void OnDestroy()
        {
            base.OnDestroy();
            _xrGrab.StartGrab -= StartGrab;
        }

        private void FixedUpdate()
        {
            if (!_userIsOwner) return;

            if (_xrGrab.isSelected)
            {
                _meshCollider.material = _zeroFrictionMaterial;

                RaycastHit hit;
                Ray ray = new Ray(transform.position, Vector3.down);
                UnityEngine.Physics.Raycast(ray, out hit, 0.08f);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 0.08f);

                if (hit.collider != null && hit.transform.name == "Chess(Clone)") KeepVerticalOrientation();
                else KeepRotatedOrientation();
            }
            else _meshCollider.material = _standartMaterial;
        }

        public void SetUserIsOwner(bool isOwner)
        {
            _userIsOwner = isOwner;
        }

        public void ResetMaterial()
        {
            _material.SetFloat("_Dissolve", 0);
        }

        public void ResetScaleAndLocalPosition()
        {
            transform.localScale = Vector3.one;
            _model.Mesh.transform.localScale = _startMeshScale;
            _model.Mesh.transform.position = _meshCollider.transform.position;
        }

        public void SetKinematic(bool isKinematic)
        {
            if (isKinematic && _xrGrab.isSelected) return;
            _rigidbody.isKinematic = isKinematic;
        }

        public void SetUseGravity(bool useGravity)
        {
            _rigidbody.useGravity = useGravity;
        }

        public void SetMeshColliderAvailable(bool isAvailable)
        {
            if (!isAvailable && _xrGrab.isSelected) return;
            _meshCollider.enabled = isAvailable;

            if (isAvailable) ResetMaterial();
        }

        private void OnTriggerEnter(Collider other)
        {
            HandGrabbingObject obj;
            ChessPiece piece;

            if (other.TryGetComponent<HandGrabbingObject>(out obj))
            {
                piece = obj.GetComponentInParent<ChessPiece>();

                if (piece != null && piece.Team != _color && CheckRotationAndOwnership())
                {
                    _counter = 0;
                    SetMaterialStateServerRpc(true, NetworkManager.Singleton.LocalClientId);
                    //piece.KeepVerticalOrientation();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            HandGrabbingObject obj;
            ChessPiece piece;

            if (other.TryGetComponent<HandGrabbingObject>(out obj))
            {
               piece = obj.GetComponentInParent<ChessPiece>();

                if (piece != null && piece.Team != _color && CheckRotationAndOwnership())
                {
                    _counter += Time.deltaTime;
                    _material.SetFloat("_Dissolve", _counter / 4);
                    if (_counter > 2)
                    {
                        EatPiece(piece.name);
                    }
                    //piece.KeepVerticalOrientation();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            HandGrabbingObject obj;
            ChessPiece piece;

            if (other.TryGetComponent<HandGrabbingObject>(out obj))
            {
                piece = obj.GetComponentInParent<ChessPiece>();

                if (piece != null && piece.Team != _color && CheckRotationAndOwnership())
                {
                    ResetMaterial();
                    SetMaterialStateServerRpc(false, NetworkManager.Singleton.LocalClientId);
                    //piece.KeepRotatedOrientation();
                }
            }
        }

        public void KeepVerticalOrientation()
        {
            var previousRotation = transform.rotation;
            _xrGrab.trackRotation = false;

            transform.rotation = Quaternion.Slerp(previousRotation, Quaternion.identity, 0.2f);
        }

        public void KeepRotatedOrientation()
        {
            _xrGrab.trackRotation = true;
        }

        private bool CheckRotationAndOwnership()
        {
            return transform.localEulerAngles.x is < 10 or > 350 && 
                   transform.localEulerAngles.z is < 10 or > 350 && _userIsOwner;
        }

        private void EatPiece(string other)
        {
            IsEaten?.Invoke(transform, _color);
        }

        private void StartGrab()
        {
            transform.localScale = Vector3.one;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetMaterialStateServerRpc(bool isHighlighted, ulong clientId)
        {
            SetMaterialStateClientRpc(isHighlighted, clientId);
        }

        [ClientRpc]
        private void SetMaterialStateClientRpc(bool isHighlighted, ulong invokerId)
        {
            if (invokerId == NetworkManager.Singleton.LocalClientId) return;

            if (isHighlighted) _material.SetFloat("_Dissolve", 0.5f);
            else ResetMaterial();
        }
    }
}
