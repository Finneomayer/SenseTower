using Assets.Mechanics.MetaAvatars.Scripts;
using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player.Customize;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using UnityEngine.XR.Management;

public class PlayerWatch : MonoBehaviour
{
    #region Inspector
    [SerializeField] private NetworkPlayer _player;
    //[SerializeField] private bool IsLocalAvatar = false;
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();

    [SerializeField] private bool IsUpdate = true;
    [SerializeField] private float _forwardPrefOffset = 0.003f; //move watch down into the arm
    [SerializeField] private Vector3 _checkOffset;
    //[SerializeField] private Vector3 _checkOffset1; 
    //[SerializeField] private Vector3 _checkOffset2;
    [SerializeField] private ClientIdView _clientIdView;
    #endregion

    private SampleAvatarEntity _avatarEnt;
    private Transform watch;
    private Coroutine _onLoadedAvatar;
    private Transform _meshTransform = null;
    private Transform _childPref;
    private bool CanUpdateScale = true;

    //ToDo кеширование. чтобы потом не было аллоцирования
    private Transform[] _skinnedBones;
    private Vector3[] _meshVerts;
    private Matrix4x4[] _meshBindposes;
    private BoneWeight[] _meshBoneWeights;

    #region Spawn/Despawn
    private void Start()
    {
        if ((Application.platform != RuntimePlatform.Android
             && NetworkManager.Singleton == null
             && !XRGeneralSettings.Instance.Manager.isInitializationComplete)) return;//For Enter scene on Win client without VR headset return;
        if (_player.IsWinUser.Value) return;

        if (_clientIdView.IsEnterScene)
            WatchSessionData.WatchIdChanged += CreatePref;
        else
            _clientIdView.OnUpdateClientInfo += CreatePref;
        _avatarEnt = GetComponent<SampleAvatarEntity>();
        UpdateCoroutine();
    }

    private void OnDestroy()
    {
        if (_onLoadedAvatar != null)
            StopCoroutine(_onLoadedAvatar);

        if (_clientIdView == null)
            WatchSessionData.WatchIdChanged -= CreatePref;
        else
            _clientIdView.OnUpdateClientInfo -= CreatePref;
    }
    #endregion

    public void Update()
    {
        //Костыль((
        //if (_meshTransform != null)
        //{
        //    var active = _avatarEnt.CurrentState == OvrAvatarEntity.AvatarState.UserAvatar && _avatarEnt.LoadState == OvrAvatarEntity.LoadingState.Loading;
        //    _meshTransform.gameObject.SetActive(!active);
        //}

        //для человека - камеры OBS
        if (_childPref != null && watch != null)
        {
            _childPref.gameObject.SetActive(!_avatarEnt.Hidden);
        }

    }

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

    private void CreatePref()
    {
        if (watch != null)
        {
            Destroy(watch.gameObject);
        }

        if (_player.IsWinUser.Value) return;

        int pref;

        if (!_clientIdView.IsEnterScene)
        {
            if (_clientIdView.IsCustomAvatarWatch)
            {
                if (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0)
                {
                    pref = _clientIdView.WatchIdOculus; //when other avatar is custom and I have VPN
                }
                else
                {
                    pref = _clientIdView.WatchID; //when other avatar is custom but i have NO VPN
                }
            }
            else 
            {
                pref = _clientIdView.WatchID;  //when other avatar is preloaded type (no VPN)
            }
        }
        else
        {
            if (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0)
            {
                pref = WatchSessionData.WatchPlayerIdOculus;
            }
            else
            {
                pref = WatchSessionData.WatchPlayerId;
            }
        }

        watch = Instantiate(GetPrefab(pref), Vector3.one * -999f, Quaternion.identity).transform;
        if (watch == null) return;
        watch.gameObject.SetActive(false);
        _childPref = watch.GetChild(0);
        CanUpdateScale = true;
    }

    private GameObject GetPrefab(int value = 0)
    {
        //if (value > _prefabs.Count) value = _prefabs.Count - 1;
        //var index = _prefabs.FindIndex(x => x.SpaceCount == value);
        var prefab = value < _prefabs.Count ? _prefabs[value] : _prefabs[0];
        //var prefab = _prefabs[1].Prefab;
        if (prefab == null)
        {
            Debug.LogError("don't found badge prefab. continue empty");
            prefab = new GameObject();
        }

        return prefab;
    }
    #endregion

    #region main methods
    private Pose GetPoseForPrefToMesh(Transform meshTransform, Transform chestTransform)
    {
       Pose pose = new Pose();        
       pose.position = chestTransform.position;
       pose.rotation = chestTransform.rotation;        

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
                if (av.transform.parent.name.Contains("LOD3"))
                {
                    _meshTransform = av.transform;
                    //_meshTransform.gameObject.SetActive(false);                 
                }
                //if (IsLocalAvatar) av.enabled = false;
            }

            yield return null;
        }

        yield return new WaitUntil(() => _avatarEnt.HasJoints);

        //_meshTransform.gameObject.SetActive(true);

        var chestTransform = _avatarEnt.GetSkeletonTransform(CAPI.ovrAvatar2JointType.RightArmLower);

        var skin = _meshTransform.GetComponent<SkinnedMeshRenderer>();
        var meshFilter = _meshTransform.GetComponent<MeshFilter>();
        _meshVerts = skin.sharedMesh.vertices;
        _skinnedBones = skin.bones;
        _meshBoneWeights = skin.sharedMesh.boneWeights;
        _meshBindposes = skin.sharedMesh.bindposes;

        int triangleIndexForPref = -1;
        Vector3Int trianglesKey = Vector3Int.zero;

        Pose pose = GetPoseForPrefToMesh(_meshTransform, chestTransform);
      
        var mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        var dist = 9999f;

        //var offset = Quaternion.Inverse( transform.rotation ) * (pose.position + chestTransform.rotation * _checkOffset - (_meshTransform.position - _meshTransform.localPosition));
        //if(IsLocalAvatar)
            //offset = _checkOffset1;
            //Debug.LogWarning($"OFFSET {offset}");
            var offset = new Vector3(-0.49f, 1.07f, 0.00f);

        //if (_text4 != null) _text4.text = $"offset= {offset}";
        //if (_text5 != null) _text5.text = $"_checkOffset1= {_checkOffset1}";

        for (int i = 0; i < 100000 && i < triangles.Length; i += 3)
        //for (int i = 0; i < triangles.Length; i += 3) 
        {
            Vector3 p0 = vertices[triangles[i + 0]];
            Vector3 p1 = vertices[triangles[i + 1]];
            Vector3 p2 = vertices[triangles[i + 2]];

            Vector3 center = (p0 + p1 + p2) / 3;
            var calculate = Vector3.Distance(center, offset);

            if (calculate < dist)
            {
                dist = calculate;
                triangleIndexForPref = i;
                trianglesKey = new Vector3Int(triangles[i + 0], triangles[i + 1], triangles[i + 2]);
                //if (_text3 != null) _text3.text = $"dist= {dist}";
            }
        }

        //if (_text1 != null) _text1.text = $"triangleIndexForPref= {triangleIndexForPref}";

       // Debug.Break();
        var until = new WaitUntil(() => IsUpdate || (watch == null || _meshTransform == null || chestTransform == null));
        if (chestTransform == null)
        {
            UpdateCoroutine();
        }
        CreatePref();
      
        while (_onLoadedAvatar != null)
        {
            if (watch != null)
                watch.gameObject.SetActive(IsUpdate);

            yield return until;

            if (watch == null || _meshTransform == null || chestTransform == null)
            {
                UpdateCoroutine();
                break;
            }

            Vector3 p0 = CalculateVertPos(trianglesKey.x);
            Vector3 p1 = CalculateVertPos(trianglesKey.y);
            Vector3 p2 = CalculateVertPos(trianglesKey.z);

            Vector3 normal = -Vector3.Cross(p1 - p0, p2 - p1).normalized;
            Vector3 center = (p0 + p1 + p2) / 3;
          
            //offset = chestTransform.position + chestTransform.rotation * _checkOffset - (_meshTransform.position - _meshTransform.localPosition);
            

            watch.position = center;
            watch.SetParent(chestTransform);
          
            var s = FindNearestPointOnLine(chestTransform.position, chestTransform.right, center);
            watch.LookAt(center - (center - s), chestTransform.right);

            watch.position += watch.forward * _forwardPrefOffset;
            if (CanUpdateScale)
            {
                // куча магичиских чисел. по идее надо вынести, но лень
                var scale = 2 * Vector3.Distance(center, s);
                //if (_text2 != null) _text2.text = $"scale= {scale}";
                if (scale > 0.071)
                    scale /= 1.2f;
                //if (IsLocalAvatar)
                scale *= 1.1f;

                ////костыль для срочного релиза
                ///аватар саши оч мелкий.
                ///печаль
                if (scale < 0.041)
                    scale *= 1.3f;
                
                _childPref.localScale = (_childPref.localScale / 0.062f * scale);
                CanUpdateScale = false;
            }
            //Graphics.DrawMesh(mesh, meshTransform.position, Quaternion.Euler(0, 0, 180) * meshTransform.rotation, mat, 0);
            //Debug.DrawRay(center, normal, Color.red);
            Debug.DrawRay(chestTransform.position, chestTransform.right, Color.red);
            yield return null;
        }
    }

    private Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        Vector3 lhs = point - origin;

        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
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
