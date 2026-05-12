using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Interactable;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Model;
using UnityEngine;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing.Interaction
{
    public class SpaceScallableGrabbingModelService : MonoBehaviour, ISpaceGrabingService
    {
        #region inspector

        [SerializeField] private Transform _leftTopCorner;
        [SerializeField] private Transform _rightDownCorner;

        [SerializeField] private SpaceStaticObjectModel _followingObj;
        #endregion

        public StaticObject StaticObject { get; private set; }
        public Vector3 LeftCornerPosition => _leftTopCorner.position;
        public Vector3 RightCornerPosition => _rightDownCorner.position;

        private ISpaceRepository _spaceRepository;

        private Enumenators.PrefabObjectType _towerPrefabType;

        private Vector3 ball1XZPosition = Vector3.zero;
        private Vector3 ball2XZPosition = Vector3.zero;

        private float InitialDistXZ = 0;
        private float InitialDistY = 0;
        private Vector3 InitialScale = Vector3.zero;

        private Coroutine _hitTheGroundCoroutine;
        private PictureImageSpaceEditingMode _grabCenter;
        
        [Inject]
        private void Construct(ISpaceRepository spaceRepository)
        {
            _spaceRepository = spaceRepository;
        }

        private void OnDisable()
        {
            if (_followingObj.TriggerGrabInteractable != null)
            {
                _followingObj.TriggerGrabInteractable.GrabStopped -= OnEndInteract;
                _followingObj.TriggerGrabInteractable.GrabStarted -= OnStartInteract;

            }
            if (_followingObj.RightTriggerGrabInteractable != null)
            {
                _followingObj.RightTriggerGrabInteractable.GrabStarted -= OnStartInteract;
                _followingObj.RightTriggerGrabInteractable.GrabStopped -= OnEndInteract;
            }
            if (_followingObj.CenterGrabInteractable != null)
            {
                _followingObj.CenterGrabInteractable.GrabStarted -= OnStartInteract;
                _followingObj.CenterGrabInteractable.GrabStopped -= OnEndInteract;
            }
            if (_grabCenter != null) _grabCenter.OnCenterGrabExit -= OnEndInteract;
        }

        private void Update()
        {
            CalculateFirstPosition();
        }

        public void SetInactiveObject()
        {
            StaticObject.IsActive = false;
            _spaceRepository.Save(_towerPrefabType, this);
        }

        public void Save()
        {
            StaticObject.Vectors.LeftTop = _followingObj.LefthandAnchor.transform.position.Vector3ToVectorComponents();
            StaticObject.Vectors.RightDown = _followingObj.RightHandAnchor.transform.position.Vector3ToVectorComponents();

            StaticObject.Vectors.Position = _followingObj.transform.position.Vector3ToVectorComponents();
            StaticObject.Vectors.Rotation = _followingObj.transform.rotation.eulerAngles.Vector3ToVectorComponents();
            StaticObject.Vectors.Scale = _followingObj.transform.lossyScale.Vector3ToVectorComponents();
            StaticObject.IsActive = true;
            _spaceRepository.Save(_towerPrefabType, this);
        }

        public void Init(Enumenators.PrefabObjectType towerPrefabType, SpaceStaticObjectModel gameObjectVisual)
        {
            StaticObject = gameObjectVisual.StaticObject;
            _towerPrefabType = towerPrefabType;
            _followingObj = gameObjectVisual;

            if (_followingObj.TryGetComponent<PictureImageSpaceEditingMode>(out PictureImageSpaceEditingMode _picture))
            {
                _grabCenter = _picture;
                _grabCenter.OnCenterGrabExit += OnEndInteract;
            }

            if (_leftTopCorner.TryGetComponent(out StaticObjectHandGrabbable left) &&
                _rightDownCorner.TryGetComponent(out StaticObjectHandGrabbable right))
            {
                gameObjectVisual.TriggerGrabInteractable = left;
                gameObjectVisual.RightTriggerGrabInteractable = right;
                gameObjectVisual.TriggerGrabInteractable.GrabStarted += OnStartInteract;
                gameObjectVisual.RightTriggerGrabInteractable.GrabStarted += OnStartInteract;
                gameObjectVisual.TriggerGrabInteractable.GrabStopped += OnEndInteract;
                gameObjectVisual.RightTriggerGrabInteractable.GrabStopped += OnEndInteract;

                if (gameObjectVisual.CenterGrabInteractable != null)
                {
                    gameObjectVisual.CenterGrabInteractable.GrabStarted += OnStartInteract;
                    gameObjectVisual.CenterGrabInteractable.GrabStopped += OnEndInteract;  
                }
            }
            
            gameObjectVisual.transform.SetParent(gameObject.transform);
            gameObjectVisual.transform.localPosition = Vector3.zero;
            StartCoroutine(SaveAfterInit());
        }

        private void OnStartInteract()
        {
            if (_followingObj != null)
            {
                _followingObj.HitTheGround = false;
            }
        }

        public void SetCornerPosition(Vector3 leftTopPosition, Vector3 rightTopPosition)
        {
            _leftTopCorner.transform.position = leftTopPosition;
            _rightDownCorner.transform.position = rightTopPosition;
        }

        private void CalculateFirstPosition()
        {
            _followingObj.transform.rotation = Quaternion.identity;

            StaticObject scalableObject = GetScalableObject();

            ball1XZPosition = new Vector3(_leftTopCorner.position.x, 0, _leftTopCorner.position.z);
            ball2XZPosition = new Vector3(_rightDownCorner.position.x, 0, _rightDownCorner.position.z);

            float CurrentDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition);
            float CurrentDistY = _leftTopCorner.position.y - _rightDownCorner.position.y;

            //Aspect ration exception for browser only to fix 16:9 senseticket
            if (_followingObj.PrefabObjectType == Enumenators.PrefabObjectType.BrowserPlace)
            {
                CurrentDistY = CurrentDistXZ / 16 * 9;
            }

            if (_followingObj.MaxScaleValue.y < CurrentDistY)
            {
                CurrentDistY = _followingObj.MaxScaleValue.y;
            }
            else if(_followingObj.MinScaleValue.y > CurrentDistY)
            {
                CurrentDistY = _followingObj.MinScaleValue.y;
            }

            if (_followingObj.MaxScaleValue.z < CurrentDistXZ)
            {
                CurrentDistXZ = _followingObj.MaxScaleValue.z;
            }
            else if(_followingObj.MinScaleValue.z > CurrentDistXZ)
            {
                CurrentDistXZ = _followingObj.MinScaleValue.z;
            }

            _followingObj.transform.localScale =
                new Vector3(_followingObj.transform.localScale.x, CurrentDistY, CurrentDistXZ);

            _followingObj.transform.position = DataExtensions.CalculateMiddlePosition(scalableObject);
            _followingObj.transform.transform.rotation =
                DataExtensions.CalculateRotation(_followingObj.transform.transform, scalableObject);

            Vector3 leftCornerRotation = _followingObj.transform.localRotation.eulerAngles;
            
            Vector3 rightCornerRotation = _followingObj.transform.localRotation.eulerAngles;
            _leftTopCorner.transform.localRotation = Quaternion.Euler(leftCornerRotation);
            _rightDownCorner.transform.localRotation = Quaternion.Euler(rightCornerRotation);
        }

        private StaticObject GetScalableObject()
        {
            StaticObject.Vectors.Position = _followingObj.transform.position.Vector3ToVectorComponents();
            StaticObject.Vectors.Rotation = _followingObj.transform.rotation.eulerAngles.Vector3ToVectorComponents();
            ;
            StaticObject.Vectors.Scale = _followingObj.transform.lossyScale.Vector3ToVectorComponents();

            StaticObject.Vectors.LeftTop = _leftTopCorner.position.Vector3ToVectorComponents();
            StaticObject.Vectors.RightDown = _rightDownCorner.position.Vector3ToVectorComponents();

            return StaticObject;
        }

        private void OnEndInteract()
        {
            Save();
            if (_hitTheGroundCoroutine != null)
            {
                StopCoroutine(_hitTheGroundCoroutine);
                _hitTheGroundCoroutine = null;
            }

            if (this.isActiveAndEnabled) _hitTheGroundCoroutine = StartCoroutine(HitTheGroundCoroutine());
        }

        private IEnumerator HitTheGroundCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            
            if (_followingObj != null)
            {
                _followingObj.HitTheGround = true;
            }
        }
        private IEnumerator SaveAfterInit()
        {
            yield return new WaitForSeconds(1f);
            Save();
        }
    }
}