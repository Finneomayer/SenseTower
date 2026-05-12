using Oculus.Avatar2;
using System.Collections;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Assets.Scripts.Shared;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using UnityEngine.XR.Management;

[RequireComponent(typeof(SampleAvatarEntity))]
public class PlayerAvatarAttachments : MonoBehaviour
{
    private SampleAvatarEntity _avatarEntity;
    [SerializeField] private NetworkPlayer _player;
    [SerializeField]
    private string tagFinger = "PlayerFingers";

    [SerializeField] private XRPokeInteractor _leftPokeInteractor;
    [SerializeField] private XRPokeInteractor _rightPokeInteractor;
    [SerializeField] private GrabbingHand _leftGrabbingHand;
    [SerializeField] private GrabbingHand _rightGrabbingHand;
    public GameObject FingerColliderLeft;
    public GameObject FingerColliderRight;

    [SerializeField] private XRDirectInteractorWithHandTracking _leftHandGrab;
    [SerializeField] private XRDirectInteractorWithHandTracking _rightHandGrab;

    private CapsuleCollider _colliderLeft;
    private CapsuleCollider _colliderRight;

    private Quaternion _rotate = Quaternion.Euler(0, 0, 90);
    private Vector3 _scale = Vector3.one * 0.015f;
    private Vector3 _offsetFingerCollider = new Vector3(-0.01f, 0, 0);
    private Vector3 _offsetFingerTransform = new Vector3(-0.012f, 0, 0);
    private Coroutine _attachmentCoroutine;
    private PlayerLogic _playerLogic;


    //all
    //private int _layerBoneMask = (1 << 27 | 1 << 31 | 1 << 35 | 1 << 39 | 1 << 43 | 1 << 47 | 1 << 51 | 1 << 55 | 1 << 59 | 1 << 63);
    //only index
    private int _layerBoneMask = ( 1 << 31  | 1 << 51 );

    protected void Start()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            _avatarEntity = GetComponent<SampleAvatarEntity>();
            _avatarEntity.OnSkeletonLoadedEvent.AddListener(OnAvatarLoaded);
            _avatarEntity.OnUserAvatarLoadedEvent.AddListener(OnAvatarLoaded);
            _avatarEntity.OnDefaultAvatarLoadedEvent.AddListener(OnAvatarLoaded);
        }

        if (!_player.IsOwner) return;

        _leftHandGrab.StartGrabingHand += DisableLeftFingerCollider;
        _leftHandGrab.StopGrabingHand += EnableLeftFingerCollider;
        _rightHandGrab.StartGrabingHand += DisableRightFingerCollider;
        _rightHandGrab.StopGrabingHand += EnableRightFingerCollider;
    }

    private void OnDestroy()
    {
        if (!_player.IsOwner) return;
        _leftHandGrab.StartGrabingHand -= DisableLeftFingerCollider;
        _leftHandGrab.StopGrabingHand -= EnableLeftFingerCollider;
        _rightHandGrab.StartGrabingHand -= DisableRightFingerCollider;
        _rightHandGrab.StopGrabingHand -= EnableRightFingerCollider;
    }

    private void EnableLeftFingerCollider()
    {
        if (_colliderLeft != null) _colliderLeft.enabled = true;
    }

    private void EnableRightFingerCollider()
    {
        if (_colliderRight != null) _colliderRight.enabled = true;
    }

    private void DisableLeftFingerCollider()
    {
        if (_colliderLeft != null) _colliderLeft.enabled = false;
    }

    private void DisableRightFingerCollider()
    {
        if (_colliderRight != null) _colliderRight.enabled = false;
    }

    private void OnAvatarLoaded(OvrAvatarEntity arg0)
    {
        if (!isActiveAndEnabled || (_player.IsWinUser.Value && !XRGeneralSettings.Instance.Manager.isInitializationComplete))
        {
            return;
        }
        if (_attachmentCoroutine != null) StopCoroutine(_attachmentCoroutine);
        //if (!_avatarEntity.Hidden) 
            _attachmentCoroutine = StartCoroutine(AttachFingers());
    }

    private IEnumerator AttachFingers()
    {
        if (_player.IsWinUser.Value && !XRGeneralSettings.Instance.Manager.isInitializationComplete) yield break;
        //_avatarEntity = GetComponent<SampleAvatarEntity>();
        yield return new WaitUntil(() => _avatarEntity.HasJoints);

        var criticalJoints = _avatarEntity.GetCriticalJoints();

        foreach (var jointType in criticalJoints)
        {

            if ((1 << (int)jointType & _layerBoneMask) == 0) continue;

            if ((int)jointType > 44)
            {
                if (FingerColliderRight != null) break;
            }
            else
            {
                if (FingerColliderLeft != null) break;
            }

            Transform jointTransform = _avatarEntity.GetSkeletonTransform(jointType);

            if (!jointTransform)
            {
                OvrAvatarLog.LogError(
                    $"SampleAvatarAttachments: No joint transform found for {jointType} on {_avatarEntity.name} ");
                continue;
            }

            var attachmentObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            attachmentObj.name = "FingerCollider";
            //  _colliders.Add(attachmentObj.GetComponent<CapsuleCollider>());
            var componentMesh = attachmentObj.GetComponent<MeshRenderer>();

            Destroy(componentMesh);

            var rb = attachmentObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.mass = 15;

            if ((int)jointType > 44)
            {
                FingerColliderRight = attachmentObj;
                _colliderRight = attachmentObj.GetComponent<CapsuleCollider>();
            }
            else
            {
                FingerColliderLeft = attachmentObj;
                _colliderLeft = attachmentObj.GetComponent<CapsuleCollider>();
            }

            attachmentObj.transform.SetParent(jointTransform, false);
            attachmentObj.tag = tagFinger;
            attachmentObj.transform.localScale = _scale;
            attachmentObj.transform.localPosition =
                (int)jointType > 44 ? _offsetFingerCollider : -_offsetFingerCollider;
            attachmentObj.transform.localRotation = _rotate;

            var fingerTransform = new GameObject();
            fingerTransform.transform.SetParent(jointTransform, false);
            fingerTransform.transform.localPosition =
                (int)jointType > 44 ? _offsetFingerTransform : -_offsetFingerTransform;

            var fingerInteractor = attachmentObj.AddComponent<FingerInteractor>();
            if ((int)jointType > 44)
            {
                if (_rightPokeInteractor != null) _rightPokeInteractor.attachTransform = fingerTransform.transform;
                fingerInteractor.Init(_rightGrabbingHand);
            }
            else
            {
                if (_leftPokeInteractor != null) _leftPokeInteractor.attachTransform = fingerTransform.transform;
                fingerInteractor.Init(_leftGrabbingHand);
            }
            //Debug.LogWarning($"Attached!! {gameObject.name} - {transform.parent.name}");
        }
    }
}
