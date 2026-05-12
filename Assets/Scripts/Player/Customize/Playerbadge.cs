using Assets.Scripts.Client;
using Assets.Scripts.Shared;
using Oculus.Avatar2;
using Sense.Interectable.Watchs;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player.Customize;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using UnityEngine.XR.Management;

[RequireComponent(typeof(SampleAvatarEntity))]
public class Playerbadge : MonoBehaviour
{
    [System.Serializable]
    public class CashePrefab
    {
        public int SpaceCount;
        public GameObject Prefab;
    }

    [SerializeField] private NetworkPlayer _player;
    [SerializeField]
    private List<CashePrefab> _prefabs = new List<CashePrefab>();
    [SerializeField]
    private bool IsUpdate = true;
    [SerializeField]
    private float _forwardBadgeOffset = 0.003f;
    [SerializeField]
    public GameObject _badgePref;
    [SerializeField]
    private Vector3 _checkOffset;
    [SerializeField]
    private ClientIdView _clientIdView;

    private SampleAvatarEntity _avatarEnt;
    private Transform badge;
    private Coroutine _onLoadedAvatar;

    //ToDo кеширование. чтобы потом не было аллоцирования
    private Transform[] _skinnedBones;
    private Vector3[] _meshVerts;
    private Matrix4x4[] _meshBindposes;
    private BoneWeight[] _meshBoneWeights;
    private Transform _meshTransform = null;
    private Transform _childBadge;
    private int countSpace = 0;

    #region Spawn/Despawn
    private void Start()
    {
        if ((Application.platform != RuntimePlatform.Android
             && NetworkManager.Singleton == null
             && !XRGeneralSettings.Instance.Manager.isInitializationComplete)) return;//For Enter scene on Win client without VR headset return;
        if (_player.IsWinUser.Value) return;

        _avatarEnt = GetComponent<SampleAvatarEntity>();
        WatchSessionData.WatchIdChanged += CreateBadge;
        UpdateCoroutine();
    }
    private void OnDestroy()
    {
        if (_onLoadedAvatar != null)
            StopCoroutine(_onLoadedAvatar);
    }
    #endregion

    #region Update
    private void Update()
    {
        //Костыль((
        if (_meshTransform != null)
        {
            var active = _avatarEnt.CurrentState == OvrAvatarEntity.AvatarState.UserAvatar && _avatarEnt.LoadState == OvrAvatarEntity.LoadingState.Loading;
            _meshTransform.gameObject.SetActive(!active);
        }
        if (_childBadge != null && badge != null)
        {
            _childBadge.gameObject.SetActive(!_avatarEnt.Hidden);
        }
    }
    #endregion

    #region Update Info
    private void UpdateCoroutine()
    {
        if (_onLoadedAvatar != null)
        {
            StopCoroutine(_onLoadedAvatar);
            _onLoadedAvatar = null;
        }
        _onLoadedAvatar = StartCoroutine(OnLoadedAvatar());
    }

    private void CreateBadge()
    {
        if (badge != null)
        {
            Destroy(badge.gameObject);
        }

        if (_player.IsWinUser.Value) return;

        badge = Instantiate(GetPrefab(_clientIdView.CountSpaces)).transform;
        badge.gameObject.SetActive(false);
        _childBadge = badge.GetChild(0);      
    }

    private GameObject GetPrefab(int spaceCount = 0)
    {
        if (spaceCount > _prefabs.Count) spaceCount = _prefabs.Count - 1;
        var index = _prefabs.FindIndex(x => x.SpaceCount == spaceCount);
        var prefab = index != -1 ? _prefabs[index].Prefab : _prefabs[_prefabs.Count - 1].Prefab;
        //var prefab = _prefabs[1].Prefab;
        if (prefab == null)
        {
            Debug.LogError("don't found badge prefab. continue empty");
            prefab = new GameObject();
        };
        return prefab;
    }
    #endregion

    #region main methods
    private Pose GetPoseForBadgeToMesh(Transform meshTransform, Transform chestTransform)
    {
        var lenght = Vector3.up * Vector3.Distance(meshTransform.position, chestTransform.position);
        Pose pose = new Pose();
        List<InputDevice> controllerDevices = new();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, controllerDevices);
        var hasHead = controllerDevices.Count > 0;
        if (!hasHead)
        {
            pose.position = meshTransform.position +
                transform.rotation *
                (meshTransform.localRotation * meshTransform.localPosition) +
                transform.rotation * meshTransform.localRotation * lenght;
            pose.rotation = Quaternion.Euler(0, 180, 0) * transform.rotation * meshTransform.localRotation;
        }
        else
        {
            pose.position = meshTransform.position +
               meshTransform.up +
                transform.rotation * meshTransform.localRotation * lenght;

            pose.rotation = transform.rotation * Quaternion.Euler(0, 180, 0) * meshTransform.localRotation;
        }

        return pose;
    }

    private IEnumerator OnLoadedAvatar()
    {
        if (_player.IsWinUser.Value) yield break;

        OvrAvatarRenderable[] avatars;

        while (_meshTransform == null)
        {
            avatars = GetComponentsInChildren<OvrAvatarRenderable>(true);
            foreach (var av in avatars)
            {
                if (av.transform.parent.name.Contains("LOD0")) //try to use LOD3
                {
                    _meshTransform = av.transform;
                    _meshTransform.gameObject.SetActive(false);
                    break;
                }
            }

            yield return null;
        }

        yield return new WaitUntil(() => _avatarEnt.HasJoints);

        _meshTransform.gameObject.SetActive(true);
        var chestTransform = _avatarEnt.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Chest);

        List<InputDevice> controllerDevices = new();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, controllerDevices);

        var skin = _meshTransform.GetComponent<SkinnedMeshRenderer>();
        var meshFilter = _meshTransform.GetComponent<MeshFilter>();
        _meshVerts = skin.sharedMesh.vertices;
        _skinnedBones = skin.bones;
        _meshBoneWeights = skin.sharedMesh.boneWeights;
        _meshBindposes = skin.sharedMesh.bindposes;

        int triangleIndexForBadge = -1;
        Vector3Int trianglesKey = Vector3Int.zero;
        Pose pose = GetPoseForBadgeToMesh(_meshTransform, chestTransform);

        var mesh = meshFilter.mesh;
        var offset = pose.position + _meshTransform.rotation * _checkOffset; //rotated avatar to real

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        var dist = 9999f;

        //ToDo там 45-60к вертексов. грудь лежит в пределах от 9700 до 26к.
        // мини оптимизация, чтобы не пробегать по всему массиву. типа в 3 раза меньше.
        for (int i = 9999; i <= 27000 || (i >= 35000 && i <= 36000); i += 3)
        {
            if (i > triangles.Length - 3) break;
            Vector3 p0 = vertices[triangles[i + 0]];
            Vector3 p1 = vertices[triangles[i + 1]];
            Vector3 p2 = vertices[triangles[i + 2]];
            p0 = _meshTransform.TransformPoint(p0);
            p1 = _meshTransform.TransformPoint(p1);
            p2 = _meshTransform.TransformPoint(p2);
            Vector3 center = (p0 + p1 + p2) / 3;
            var calculate = Vector3.Distance(center, offset);

            if (calculate < dist)
            {
                dist = calculate;
                triangleIndexForBadge = i;
                trianglesKey = new Vector3Int(triangles[i + 0], triangles[i + 1], triangles[i + 2]);
            }
            if (i == 27000 && triangles.Length >= 36000)
                i = 35001;
        }

        var until = new WaitUntil(() => IsUpdate);

        yield return new WaitUntil(() => (_clientIdView.CountSpaces != 0) || chestTransform == null);
        if (chestTransform == null)
        {
            UpdateCoroutine();
        }
        
        CreateBadge();
        while (_onLoadedAvatar != null)
        {
            if (badge != null)
                badge.gameObject.SetActive(IsUpdate);

            yield return until;

            if (badge == null || _meshTransform == null || chestTransform == null)
            {
                UpdateCoroutine();
                break;
            }

            Vector3 p0 = CalculateVertPos(trianglesKey.x);
            Vector3 p1 = CalculateVertPos(trianglesKey.y);
            Vector3 p2 = CalculateVertPos(trianglesKey.z);

            Vector3 normal = -Vector3.Cross(p1 - p0, p2 - p1).normalized;
            Vector3 center = (p0 + p1 + p2) / 3;
            if (!IsUpdate)
            {
                yield return null;
            }
            badge.position = center;
            badge.SetParent(chestTransform);
            badge.LookAt(badge.transform.position + normal, transform.up);
            badge.position += badge.transform.forward * _forwardBadgeOffset;

            var checkOffset = _checkOffset;
            //Graphics.DrawMesh(mesh, _meshTransform.position, Quaternion.Euler(0, 0, 180) * _meshTransform.rotation, mat, 0);

            yield return null;
        }
    }

    //рассчет вертекса. РЕАЛЬНОГО положения, а не Т-форм
    private Vector3 CalculateVertPos(int key)
    {
        BoneWeight weight;
        Matrix4x4 bm0;
        Matrix4x4 bm1;
        Matrix4x4 bm2;
        Matrix4x4 bm3;
        Matrix4x4 vm = new Matrix4x4();

        weight = _meshBoneWeights[key];

        bm0 = _skinnedBones[weight.boneIndex0].localToWorldMatrix * _meshBindposes[weight.boneIndex0];
        bm1 = _skinnedBones[weight.boneIndex1].localToWorldMatrix * _meshBindposes[weight.boneIndex1];
        bm2 = _skinnedBones[weight.boneIndex2].localToWorldMatrix * _meshBindposes[weight.boneIndex2];
        bm3 = _skinnedBones[weight.boneIndex3].localToWorldMatrix * _meshBindposes[weight.boneIndex3];

        vm.m00 = bm0.m00 * weight.weight0 + bm1.m00 * weight.weight1 + bm2.m00 * weight.weight2 + bm3.m00 * weight.weight3;
        vm.m01 = bm0.m01 * weight.weight0 + bm1.m01 * weight.weight1 + bm2.m01 * weight.weight2 + bm3.m01 * weight.weight3;
        vm.m02 = bm0.m02 * weight.weight0 + bm1.m02 * weight.weight1 + bm2.m02 * weight.weight2 + bm3.m02 * weight.weight3;
        vm.m03 = bm0.m03 * weight.weight0 + bm1.m03 * weight.weight1 + bm2.m03 * weight.weight2 + bm3.m03 * weight.weight3;

        vm.m10 = bm0.m10 * weight.weight0 + bm1.m10 * weight.weight1 + bm2.m10 * weight.weight2 + bm3.m10 * weight.weight3;
        vm.m11 = bm0.m11 * weight.weight0 + bm1.m11 * weight.weight1 + bm2.m11 * weight.weight2 + bm3.m11 * weight.weight3;
        vm.m12 = bm0.m12 * weight.weight0 + bm1.m12 * weight.weight1 + bm2.m12 * weight.weight2 + bm3.m12 * weight.weight3;
        vm.m13 = bm0.m13 * weight.weight0 + bm1.m13 * weight.weight1 + bm2.m13 * weight.weight2 + bm3.m13 * weight.weight3;

        vm.m20 = bm0.m20 * weight.weight0 + bm1.m20 * weight.weight1 + bm2.m20 * weight.weight2 + bm3.m20 * weight.weight3;
        vm.m21 = bm0.m21 * weight.weight0 + bm1.m21 * weight.weight1 + bm2.m21 * weight.weight2 + bm3.m21 * weight.weight3;
        vm.m22 = bm0.m22 * weight.weight0 + bm1.m22 * weight.weight1 + bm2.m22 * weight.weight2 + bm3.m22 * weight.weight3;
        vm.m23 = bm0.m23 * weight.weight0 + bm1.m23 * weight.weight1 + bm2.m23 * weight.weight2 + bm3.m23 * weight.weight3;

        var result = vm.MultiplyPoint3x4(_meshVerts[key]);

        return result;
    }
    #endregion
}
