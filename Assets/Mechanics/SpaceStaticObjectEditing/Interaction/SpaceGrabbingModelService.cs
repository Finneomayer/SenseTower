using System;
using System.Collections;
using Assets.Scripts.Data;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Model;
using UnityEngine;
using Zenject;

namespace Mechanics.SpaceStaticObjectEditing.Interaction
{
    public class SpaceGrabbingModelService : MonoBehaviour, ISpaceGrabingService
    {
        public StaticObject StaticObject { get; private set; }

        private SpaceStaticObjectModel _spaceObjectModel;

        private ISpaceRepository _spaceRepository;
        private StaticObject _staticObject;
        private Coroutine _hitTheGroundCoroutine;

        [Inject]
        private void Construct(ISpaceRepository spaceRepository)
        {
            _spaceRepository = spaceRepository;
        }

        private void OnDisable()
        {
            if (_spaceObjectModel.TriggerGrabInteractable != null)
            {
                _spaceObjectModel.TriggerGrabInteractable.GrabStarted -= OnStartInteract;
                _spaceObjectModel.TriggerGrabInteractable.GrabStopped -= OnEndInteract;
            }
        }

        public void Init(Enumenators.PrefabObjectType towerPrefabType, SpaceStaticObjectModel gameObjectVisual)
        {
            gameObjectVisual.transform.SetParent(gameObject.transform);
            gameObjectVisual.transform.localPosition = Vector3.zero;
            gameObjectVisual.transform.localRotation = Quaternion.Euler(Vector3.zero);
            StaticObject = gameObjectVisual.StaticObject;
            _spaceObjectModel = gameObjectVisual;
            
            if (_spaceObjectModel.TriggerGrabInteractable != null)
            {
                _spaceObjectModel.TriggerGrabInteractable.GrabStarted += OnStartInteract;
                _spaceObjectModel.TriggerGrabInteractable.GrabStopped += OnEndInteract;
            }
            StartCoroutine(SaveAfterInit());
        }

        public void SetPosition(Vector3 position, Vector3 rotation)
        {
            transform.position = position;
            if (_spaceObjectModel != null)
            {
                _spaceObjectModel.gameObject.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        public void SetInactiveObject()
        {
            StaticObject.IsActive = false;
            _spaceRepository.Save(_spaceObjectModel.PrefabObjectType, this);
        }

        public void Save()
        {
            StaticObject.Vectors.LeftTop = null;
            StaticObject.Vectors.RightDown = null;
            StaticObject.Vectors.Position = _spaceObjectModel.transform.position.Vector3ToVectorComponents();
            StaticObject.Vectors.Rotation = _spaceObjectModel.transform.rotation.eulerAngles.Vector3ToVectorComponents();
            StaticObject.Vectors.Scale = _spaceObjectModel.transform.lossyScale.Vector3ToVectorComponents();
            StaticObject.IsActive = true;
            _spaceRepository.Save(_spaceObjectModel.PrefabObjectType, this);
        }
        
        private void OnStartInteract()
        {
            if (_spaceObjectModel != null)
            {
                _spaceObjectModel.HitTheGround = false;
            }
        }
        private void OnEndInteract()
        {
            if (StaticObject == null && _spaceObjectModel == null)
                return;

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
            
            if (_spaceObjectModel != null)
            {
                _spaceObjectModel.HitTheGround = true;
            }
        }

        private IEnumerator SaveAfterInit()
        {
            yield return new WaitForSeconds(1f);
            Save();
        }
    }
}