using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Space;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.SpaceStaticObjectEditing.Interaction;
using Mechanics.SpaceStaticObjectEditing.Model;
using UnityEngine;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing
{
    public class SpaceFactory : MonoBehaviour, ISpaceFactory
    {
        #region Inspector

        [SerializeField] private SpaceScallableGrabbingModelService _commonScalablePrefab;
        [SerializeField] private SpaceGrabbingModelService _commonStaticPrefab;
        [SerializeField] private List<SpaceStaticObjectModel> _objectVisualModelsList;

        #endregion

        private List<SpaceStaticObjectModel> _sceneObject = new();
        private string _roomId;

        private DiContainer _diContaner;
        private IStaticObjectsService _staticObjectsService;
        private ISpaceManager _spaceManager;
        
        [Inject]
        private void Construct(DiContainer diContainer, IStaticObjectsService staticObjectsService,
            ISpaceManager spaceManager, IClientData clientData)
        {
            _staticObjectsService = staticObjectsService;
            _diContaner = diContainer;
            _spaceManager = spaceManager;
#if !UNITY_SERVER

            if (!string.IsNullOrEmpty(clientData.AccessToken))
            {
                if (spaceManager.CurrentTransitionTarget?.SpaceOwner == null)
                {
                    if (DataExtensions.AvailableUsers(clientData.UserName)
                        || DataExtensions.AvailableAdmin(spaceManager.CurrentTransitionTarget,clientData.UserId.ToString()))
                        GetExistSceneStaticObject(_spaceManager.CurrentTransitionTarget.Id.ToString());

                    return;
                }

                string ownerId = spaceManager.CurrentTransitionTarget.SpaceOwner.UserId.ToString();
                if (clientData.UserId.ToString().Equals(ownerId) 
                    || DataExtensions.AvailableUsers(clientData.UserName)
                    || DataExtensions.AvailableAdmin(spaceManager.CurrentTransitionTarget,clientData.UserId.ToString()))
                    GetExistSceneStaticObject(_spaceManager.CurrentTransitionTarget.Id.ToString());
            }
#endif
#if UNITY_SERVER
            _roomId = DataExtensions.GetSpaceID();
#endif
        }

        public StaticObject[] GetExistModelPrefabs()
        {
            return _sceneObject.Where(elem => elem.SpawnInStartInventory).Select(element => element.StaticObject)
                .ToArray();
        }

        public SpaceStaticObjectModel CreateObjectVisualize(string id)
        {
            SpaceStaticObjectModel tempGameObject = null;

            foreach (SpaceStaticObjectModel objectVisual in _sceneObject)
            {
                if (objectVisual.StaticObject == null)
                    continue;

                if (objectVisual.StaticObject.TowerObjectId.ToString().Equals(id))
                {
                    GameObject gameObjectInstance = _diContaner.InstantiatePrefab(objectVisual.gameObject);
                    if (gameObjectInstance.TryGetComponent(out SpaceStaticObjectModel spaceStaticObjectModel))
                    {
                        spaceStaticObjectModel.StaticObject = objectVisual.StaticObject;
                        tempGameObject = spaceStaticObjectModel;
                    }

                    break;
                }
            }

            return tempGameObject;
        }

        /// <summary>
        /// Using when open space editing mode
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        /// <param name="grabbingHand"></param>
        /// <returns></returns>
        public GameObject CreateSpaceEditingObject(string uid, Vector3 position, Vector3 offset, GrabbingHand grabbingHand = default)
        {
            var ObjectVisualize = CreateObjectVisualize(uid);
            var movementObject = CreateMovementObject(ObjectVisualize, ObjectVisualize.MovementType, false);

            SetObjectPosition(ObjectVisualize.MovementType == MovementType.Scallable ? 
                    movementObject : ObjectVisualize.gameObject, 
                position, ObjectVisualize.offsetPosition, grabbingHand);

            return movementObject;
        }

        /// <summary>
        /// Using when pick up an object from shelf
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameObject CreateObjectWithParametrs(string id)
        {
            var ObjectVisualize = CreateObjectVisualize(id);
            var movementObject = CreateMovementObject(ObjectVisualize, ObjectVisualize.MovementType, true);

            return movementObject;
        }

        private GameObject CreateMovementObject(SpaceStaticObjectModel objectVisual, MovementType movementType, bool TransferSavedSettings)
        {
            GameObject commonMovingPrefab = movementType == MovementType.Scallable
                ? _diContaner.InstantiatePrefab(_commonScalablePrefab)
                : _diContaner.InstantiatePrefab(_commonStaticPrefab);
            if (!TransferSavedSettings)
            {
                if (commonMovingPrefab.TryGetComponent(out ISpaceGrabingService spaceGrabingService))
                {
                    //object on shelf
                    spaceGrabingService.Init(objectVisual.PrefabObjectType, objectVisual);
                    //if (movementType == MovementType.Scallable)
                    //{
                    //    if (objectVisual.TryGetComponent(out PictureImageSpaceEditingMode pictureGrab))
                    //        pictureGrab.DisableCenterGrab();
                    //}
                }
            }
            else
            {
                if (movementType == MovementType.Scallable && objectVisual.StaticObject != null)
                {
                    if (commonMovingPrefab.TryGetComponent(out SpaceScallableGrabbingModelService scallableGrabbingModelService))
                    {
                        Vector3 LeftTopCornerPosition = objectVisual.StaticObject.Vectors.LeftTop.VectorComponentsToVector3();
                        Vector3 RightTopCornerPosition = objectVisual.StaticObject.Vectors.RightDown.VectorComponentsToVector3();

                        scallableGrabbingModelService.SetCornerPosition(LeftTopCornerPosition, RightTopCornerPosition);
                        scallableGrabbingModelService.Init(objectVisual.StaticObject.PrefabObjectType, objectVisual);
                    }
                }
                else if (movementType == MovementType.Grabbing && objectVisual.StaticObject != null)
                {
                    if (commonMovingPrefab.TryGetComponent(out SpaceGrabbingModelService grabbingModelService))
                    {
                        Vector3 ObjectPosition = objectVisual.StaticObject.Vectors.Position.VectorComponentsToVector3();
                        Vector3 ObjectRotation = objectVisual.StaticObject.Vectors.Rotation.VectorComponentsToVector3();

                        grabbingModelService.Init(objectVisual.StaticObject.PrefabObjectType, objectVisual);
                        grabbingModelService.SetPosition(ObjectPosition, ObjectRotation);
                    }
                }
            }

            return commonMovingPrefab;
        }

        private void SetObjectPosition(GameObject target, Vector3 position, Vector3 offset,
            GrabbingHand grabbingHand = default)
        {
            if (target == null)
                return;

            if (grabbingHand != null)
            {
                target.transform.position = grabbingHand.CurrentObjectAnchorPosition + offset;
                //target.transform.SetPositionAndRotation(grabbingHand.CurrentObjectAnchorPosition + offset,
                //    grabbingHand.CurrentObjectAnchorRotation);
            }
            else
            {
                target.transform.position = position + offset;
            }
        }

        private async void GetExistSceneStaticObject(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            List<StaticObject> _sceneObjects = await _staticObjectsService.GetAllSceneStaticObjects(id);

            if (_sceneObjects.Count != 0)
                CollectStaticObject(_sceneObjects);
        }

        private void CollectStaticObject(List<StaticObject> sceneObjects)
        {
            foreach (StaticObject staticObject in sceneObjects)
            {
                var tempObj = _objectVisualModelsList.FirstOrDefault(element =>
                    element.PrefabObjectType == staticObject.PrefabObjectType);

                if (tempObj != null && tempObj.StaticObject == null)
                {
                    SpaceStaticObjectModel tempStaticObjectModel = tempObj.Clone();

                    if (!string.IsNullOrEmpty(staticObject.TempRelatedObjectId))
                    {
                        var relatedStaticObject = sceneObjects.FirstOrDefault(element =>
                            element.TowerObjectId.ToString() == staticObject.TempRelatedObjectId);

                        if (relatedStaticObject != null)
                        {
                            var tempRelatedObj = _objectVisualModelsList.FirstOrDefault(element =>
                                element.PrefabObjectType == relatedStaticObject.PrefabObjectType);
                            if (tempRelatedObj != null)
                            {
                                SpaceStaticObjectModel tempRelatedStaticObjectModel = tempRelatedObj.Clone();
                                tempRelatedStaticObjectModel.StaticObject = relatedStaticObject;
                                tempStaticObjectModel._dependantObjects = new List<SpaceStaticObjectModel>()
                                    {tempRelatedStaticObjectModel};
                            }
                        }
                    }

                    tempStaticObjectModel.StaticObject = staticObject;
                    _sceneObject.Add(tempStaticObjectModel);
                }
            }
        }
    }
}