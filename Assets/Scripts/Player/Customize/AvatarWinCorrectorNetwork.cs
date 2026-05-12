using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Avatar2;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.XR.Management;

namespace Assets.Scripts.Player.Customize
{
    public class AvatarWinCorrectorNetwork : MonoBehaviour
    {
        [SerializeField] private Shared.NetworkPlayer _player;
        [SerializeField] private SampleAvatarEntity _avatar;
        [SerializeField, Range(1.8f, 2f)] private float _headCutShift = 1.92f;
        private Transform _meshTransform;
        private Coroutine _cutAvatarCoroutine;

        private void Start()
        {
            _avatar.OnUserAvatarLoadedEvent.AddListener(CutAvatarUser);
            _avatar.OnDefaultAvatarLoadedEvent.AddListener(CutAvatarDefault);
        }

        private void OnDisable()
        {
            _avatar.OnUserAvatarLoadedEvent.RemoveListener(CutAvatarUser);
            _avatar.OnDefaultAvatarLoadedEvent.RemoveListener(CutAvatarDefault);
        }

        private void CutAvatarUser(OvrAvatarEntity arg0)
        {
            Debug.Log("CutAvatarUser check");
            if (_cutAvatarCoroutine != null) StopCoroutine(_cutAvatarCoroutine);

            _cutAvatarCoroutine = StartCoroutine(SetAvatarWinViewCoroutine(true));
        }

        private void CutAvatarDefault(OvrAvatarEntity arg0)
        {
            Debug.Log("CutAvatarDefault check");
            if (_cutAvatarCoroutine != null) StopCoroutine(_cutAvatarCoroutine);

            _cutAvatarCoroutine = StartCoroutine(SetAvatarWinViewCoroutine(false));
            
            //var headTransform = _avatar.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
            //if (headTransform != null)
            //{
            //    SphereCollider headCollider = headTransform.gameObject.AddComponent<SphereCollider>();
            //    headCollider.radius = 0.2f;
            //    headCollider.center = Vector3.right * 0.1f;
            //    headCollider.isTrigger = true;
            //}
        }



        private IEnumerator SetAvatarWinViewCoroutine(bool isUserAvatar)
        {
            OvrAvatarRenderable[] avatars;
            SkinnedMeshRenderer skin = new();
            _meshTransform = null;

            while (_meshTransform == null)
            {
                avatars = GetComponentsInChildren<OvrAvatarRenderable>(true);

                yield return null;


                foreach (var av in avatars)
                {
                    //if (!isUserAvatar && av.transform.parent.name.Contains("AllLOD") 
                    //    || (isUserAvatar && av.transform.parent.name.Contains("LOD0")))
                    if (av != null)
                    {
                        _meshTransform = av.transform;
                        if (_meshTransform.TryGetComponent<SkinnedMeshRenderer>(out skin)) CutAvatar(skin);
                    }

                    yield return null;
                }
            }
        }

        private void CutAvatar(SkinnedMeshRenderer skin)
        {
            Mesh sharedMesh = new Mesh();

            sharedMesh.vertices = skin.sharedMesh.vertices;
            sharedMesh.colors = skin.sharedMesh.colors;
            sharedMesh.triangles = skin.sharedMesh.triangles;
            sharedMesh.bounds = skin.sharedMesh.bounds;
            sharedMesh.bindposes = skin.sharedMesh.bindposes;
            sharedMesh.boneWeights = skin.sharedMesh.boneWeights;
            sharedMesh.indexBufferTarget = skin.sharedMesh.indexBufferTarget;
            sharedMesh.indexFormat = skin.sharedMesh.indexFormat;
            sharedMesh.colors32 = skin.sharedMesh.colors32;
            sharedMesh.bindposes = skin.sharedMesh.bindposes;
            sharedMesh.normals = skin.sharedMesh.normals;
            sharedMesh.tangents = skin.sharedMesh.tangents;
            sharedMesh.uv = skin.sharedMesh.uv;
            sharedMesh.subMeshCount = skin.sharedMesh.subMeshCount;

            var deltaVertices = new Vector3[skin.sharedMesh.vertexCount];
            var deltaNormals = new Vector3[skin.sharedMesh.vertexCount];
            var deltaTangents = new Vector3[skin.sharedMesh.vertexCount];

            List<string> blendShapeNames = new List<string>();

            for (int i = 0; i < skin.sharedMesh.blendShapeCount; i++)
            {
                blendShapeNames.Add(skin.sharedMesh.GetBlendShapeName(i));
            }

                //for (int i = 0; i < skin.sharedMesh.blendShapeCount; i++)
                //{
                //    int frameCount = skin.sharedMesh.GetBlendShapeFrameCount(i);
                //    string shapeName = skin.sharedMesh.GetBlendShapeName(i);

                //    for (int j = 0; j < frameCount; j++)
                //    {
                //        var frameWeight = skin.sharedMesh.GetBlendShapeFrameWeight(i, j);
                //        skin.sharedMesh.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                //        sharedMesh.AddBlendShapeFrame(shapeName, j, deltaVertices, deltaNormals, deltaTangents);
                //    }
                //}



                var triangles = sharedMesh.triangles.ToList();
            var vertices = sharedMesh.vertices.ToList();

            float cutPlaneCoordinate = transform.localPosition.y + _headCutShift + 0.01f;

            int count = triangles.Count / 3;
            for (int i = count - 1; i >= 0; i--)
            {
                Vector3 p0 = vertices[triangles[i * 3 + 0]];
                Vector3 p1 = vertices[triangles[i * 3 + 1]];
                Vector3 p2 = vertices[triangles[i * 3 + 2]];

                Vector3 center = (p0 + p1 + p2) / 3;

                if (center.y < transform.localPosition.y + _headCutShift)
                {
                    triangles.RemoveRange(i * 3, 3);
                }
                else
                {
                    if (vertices[triangles[i * 3 + 0]].y < cutPlaneCoordinate)
                        vertices[triangles[i * 3 + 0]] = new Vector3(
                            vertices[triangles[i * 3 + 0]].x,
                            cutPlaneCoordinate,
                            vertices[triangles[i * 3 + 0]].z);

                    if (vertices[triangles[i * 3 + 1]].y < cutPlaneCoordinate)
                        vertices[triangles[i * 3 + 1]] = new Vector3(
                            vertices[triangles[i * 3 + 1]].x,
                            cutPlaneCoordinate,
                            vertices[triangles[i * 3 + 1]].z);

                    if (vertices[triangles[i * 3 + 2]].y < cutPlaneCoordinate)
                        vertices[triangles[i * 3 + 2]] = new Vector3(
                            vertices[triangles[i * 3 + 2]].x,
                            cutPlaneCoordinate,
                            vertices[triangles[i * 3 + 2]].z);
                }
            }

            if (triangles.Count == 0) return;

            for (int c = 0; c < 100; c++)
            {
                if (_player.IsWinUser.Value
                    || (Application.platform != RuntimePlatform.Android
                        && NetworkManager.Singleton == null
                        && !XRGeneralSettings.Instance.Manager
                            .isInitializationComplete)) //For Enter scene on Win client without VR headset

                {
                    sharedMesh.vertices = vertices.ToArray();
                    sharedMesh.triangles = triangles.ToArray();
                    sharedMesh.RecalculateBounds();

                    //skin.sharedMesh = null;
                    //skin.sharedMesh = sharedMesh;
                    skin.sharedMesh = new Mesh();

                    skin.sharedMesh.vertices = sharedMesh.vertices;
                    skin.sharedMesh.colors = sharedMesh.colors;
                    skin.sharedMesh.triangles = sharedMesh.triangles;
                    skin.sharedMesh.bounds = sharedMesh.bounds;
                    skin.sharedMesh.bindposes = sharedMesh.bindposes;
                    skin.sharedMesh.boneWeights = sharedMesh.boneWeights;
                    skin.sharedMesh.indexBufferTarget = sharedMesh.indexBufferTarget;
                    skin.sharedMesh.indexFormat = sharedMesh.indexFormat;
                    skin.sharedMesh.colors32 = sharedMesh.colors32;
                    skin.sharedMesh.bindposes = sharedMesh.bindposes;
                    skin.sharedMesh.normals = sharedMesh.normals;
                    skin.sharedMesh.tangents = sharedMesh.tangents;
                    skin.sharedMesh.uv = sharedMesh.uv;
                    skin.sharedMesh.subMeshCount = sharedMesh.subMeshCount;

                    //for (int i = 0; i < sharedMesh.blendShapeCount; i++)
                    //{
                    //    int frameCount = sharedMesh.GetBlendShapeFrameCount(i);
                    //    string shapeName = sharedMesh.GetBlendShapeName(i);

                    //    for (int j = 0; j < frameCount; j++)
                    //    {
                    //        var frameWeight = sharedMesh.GetBlendShapeFrameWeight(i, j);
                    //        sharedMesh.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                    //        skin.sharedMesh.AddBlendShapeFrame(shapeName, j, deltaVertices, deltaNormals, deltaTangents);
                    //    }
                    //}


                    for (int i = 0; i < blendShapeNames.Count; i++)
                    {
                        skin.sharedMesh.AddBlendShapeFrame(blendShapeNames[i], 0, deltaVertices, deltaNormals, deltaTangents);
                    }

                    break;
                }
            }
        }
    }
}
