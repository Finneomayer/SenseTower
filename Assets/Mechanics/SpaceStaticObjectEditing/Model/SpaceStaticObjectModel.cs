using System;
using System.Collections.Generic;
using agora_gaming_rtc;
using Assets.Scripts.Interactable;
using Data;
using Mechanics.LoadSceneObjects.Models;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing.Model
{
    public class SpaceStaticObjectModel : MonoBehaviour
    {
        public StaticObject StaticObject;
        public Enumenators.PrefabObjectType PrefabObjectType;
        public MovementType MovementType;
        public StaticObjectHandGrabbable TriggerGrabInteractable;
        public TriggerHandGrabbable CenterGrabInteractable;
        public StaticObjectHandGrabbable RightTriggerGrabInteractable;
        public GameObject LefthandAnchor;
        public GameObject RightHandAnchor;
        public List<SpaceStaticObjectModel> _dependantObjects;
        public Vector3 offsetPosition;
        public bool SpawnInStartInventory = true;
        public Vector3 MaxScaleValue;
        public Vector3 MinScaleValue;
        public bool HitTheGround = true;

        private void Awake()
        {
            HitTheGround = true;
        }

        public SpaceStaticObjectModel Clone()
        {
            return (SpaceStaticObjectModel)this.MemberwiseClone();
        }
    }
    
    #region InnerData

    public enum MovementType
    {
        Scallable = 0,
        Grabbing = 1
    }

    #endregion
}