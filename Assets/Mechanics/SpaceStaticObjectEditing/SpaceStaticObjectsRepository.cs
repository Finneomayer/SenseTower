using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Interaction;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing
{
    public class SpaceStaticObjectsRepository : NetworkBehaviour, ISpaceRepository
    {
        private Dictionary<string, StaticObject> _objectsInServer = new();

        public void Save(Enumenators.PrefabObjectType prefabType, ISpaceGrabingService obj)
        {
            if (obj.StaticObject == null)
                return;

            SceneStaticObjectData sceneStaticObjectData = new();
            sceneStaticObjectData.TowerPrefabType = prefabType;
            sceneStaticObjectData.RemoteObjectType = obj.StaticObject.RemoteObjectType;
            sceneStaticObjectData.TowerObjectId = obj.StaticObject.TowerObjectId.ToString();
            sceneStaticObjectData.ObjectKey = string.IsNullOrEmpty(obj.StaticObject.ObjectKey)
                ? "Sample"
                : obj.StaticObject.ObjectKey;
            sceneStaticObjectData.TempRelatedObjectId = string.IsNullOrEmpty(obj.StaticObject.TempRelatedObjectId)
                ? string.Empty
                : obj.StaticObject.TempRelatedObjectId;
            sceneStaticObjectData.RepositoryUrl = string.IsNullOrEmpty(obj.StaticObject.RepositoryUrl)
                ? "Sample"
                : obj.StaticObject.RepositoryUrl;
            sceneStaticObjectData.PlaceNumber = obj.StaticObject.PlaceNumber ?? 0;
            sceneStaticObjectData.Position = obj.StaticObject.Vectors.Position.VectorComponentsToVector3();
            sceneStaticObjectData.Rotation = obj.StaticObject.Vectors.Rotation.VectorComponentsToVector3();
            sceneStaticObjectData.Scale = obj.StaticObject.Vectors.Scale.VectorComponentsToVector3();
            sceneStaticObjectData.LeftTop = obj.StaticObject.Vectors.LeftTop.VectorComponentsToVector3();
            sceneStaticObjectData.RightDown = obj.StaticObject.Vectors.RightDown.VectorComponentsToVector3();
            sceneStaticObjectData.IsActive = obj.StaticObject.IsActive;
            
            SaveSpaceStaticObjectDataServerRpc(sceneStaticObjectData);
        }

        public List<StaticObject> GetSpaceObjectsList()
        {
            return _objectsInServer.Values.ToList();
        }

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void SaveSpaceStaticObjectDataServerRpc(SceneStaticObjectData staticObjectData)
        {
            StaticObject tempScalableObject = new();

            if (_objectsInServer.ContainsKey(staticObjectData.TowerObjectId))
            {
                tempScalableObject = _objectsInServer[staticObjectData.TowerObjectId];
            }
            else
            {
                tempScalableObject.RemoteObjectType = staticObjectData.RemoteObjectType;
                tempScalableObject.PrefabObjectType = staticObjectData.TowerPrefabType;
                tempScalableObject.ObjectKey = staticObjectData.ObjectKey;
            }

            tempScalableObject.IsActive = staticObjectData.IsActive;
            tempScalableObject.PlaceNumber = staticObjectData.PlaceNumber;
            tempScalableObject.TempRelatedObjectId = string.IsNullOrEmpty(staticObjectData.TempRelatedObjectId)
                ? null
                : staticObjectData.TempRelatedObjectId;
            
            tempScalableObject.TowerObjectId = Guid.Parse(staticObjectData.TowerObjectId);
            tempScalableObject.Vectors = staticObjectData.Vectors;
            _objectsInServer[staticObjectData.TowerObjectId] = tempScalableObject;
        }

        #endregion

        #region InnerClass

        public struct SceneStaticObjectData : INetworkSerializable
        {
            public string TowerObjectId;
            public Enumenators.RemoteContentType RemoteObjectType;
            public Enumenators.PrefabObjectType TowerPrefabType;
            public string ObjectKey;
            public string RepositoryUrl;
            public string TempRelatedObjectId;
            public int PlaceNumber;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public Vector3 LeftTop;
            public Vector3 RightDown;
            public Vectors Vectors;
            public bool IsActive;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                Vectors = new Vectors();
                serializer.SerializeValue(ref RemoteObjectType);
                serializer.SerializeValue(ref TowerPrefabType);
                serializer.SerializeValue(ref TowerObjectId);
                serializer.SerializeValue(ref ObjectKey);
                serializer.SerializeValue(ref RepositoryUrl);
                serializer.SerializeValue(ref TempRelatedObjectId);
                serializer.SerializeValue(ref PlaceNumber);

                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Rotation);
                serializer.SerializeValue(ref Scale);
                serializer.SerializeValue(ref LeftTop);
                serializer.SerializeValue(ref RightDown);
                serializer.SerializeValue(ref IsActive);

                Vectors = new();

                Vectors.Position = Position.Vector3ToVectorComponents();
                Vectors.Rotation = Rotation.Vector3ToVectorComponents();
                Vectors.Scale = Scale.Vector3ToVectorComponents();
                Vectors.LeftTop = LeftTop.Vector3ToVectorComponents();
                Vectors.RightDown = RightDown.Vector3ToVectorComponents();
            }
        }

        #endregion
    }
}