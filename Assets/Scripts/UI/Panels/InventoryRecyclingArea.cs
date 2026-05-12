using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UI
{
    public class InventoryRecyclingArea : MonoBehaviour
    {
        [SerializeField]
        private Transform GrabbablesContent;

        private InventoryPlaceGrabbable[] _grabbables = new InventoryPlaceGrabbable[0];
        private List<GameObject> _recyclingObjects = new();

        private ITowerObjectRecyclingHandler<InventoryObject> _inventoryObjectRecyclingHandler;

        private void Awake()
        {
            _grabbables = GrabbablesContent.GetComponentsInChildren<InventoryPlaceGrabbable>();
        }

        private void OnEnable()
        {
            foreach (var grabbable in _grabbables)
            {
                grabbable.GrabStopped += OnGrabStopped;
            }
        }

        private void OnDisable()
        {
            foreach (var grabbable in _grabbables)
            {
                grabbable.GrabStopped -= OnGrabStopped;
            }
        }

        public void Init(ITowerObjectRecyclingHandler<InventoryObject> inventoryObjectRecyclingHandler)
        {
            _inventoryObjectRecyclingHandler = inventoryObjectRecyclingHandler;
        }

        public bool AreObjectsInRecyclingArea()
        {
            foreach (var grabbable in _grabbables)
            {
                foreach (var hand in grabbable.GrabbingHands)
                {
                    foreach (var item in hand.InventoryObjects)
                    {
                        if (_inventoryObjectRecyclingHandler.CanBeRecycled(item))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void OnGrabStopped(GrabbingHand grabbingHand)
        {
            _recyclingObjects.RemoveAll(item => item == null);

            if (grabbingHand.InventoryObjects.Count == 0)
            {
                return;
            }

            List<InventoryObject> newRecyclingObjects = new();
            foreach (var obj in grabbingHand.InventoryObjects)
            {
                if (_recyclingObjects.FirstOrDefault(item => item.gameObject == obj) != null)
                {
                    continue;
                }
                _recyclingObjects.Add(obj.gameObject);
                newRecyclingObjects.Add(obj);
            }

            foreach (var obj in newRecyclingObjects)
            {
                _inventoryObjectRecyclingHandler.ProcessRecycling(obj);
            }
        }

    }
}
