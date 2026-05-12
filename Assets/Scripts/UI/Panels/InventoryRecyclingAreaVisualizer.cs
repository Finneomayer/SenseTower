using System;
using UnityEngine;

namespace UI
{
    public class InventoryRecyclingAreaVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Transform MaterialColorChangersContent;
        [SerializeField]
        private InventoryRecyclingArea RecyclingArea;
        [SerializeField, Range(0f, 1f)]
        private float MinAlpha = 0;
        [SerializeField, Range(0f, 1f)]
        private float MaxAlpha = 0.1f;

        private MaterialColorChanger[] _materialChangers;
        private bool _areInventoryObjectsInHands;

        private void Awake()
        {
            _materialChangers = MaterialColorChangersContent.GetComponentsInChildren<MaterialColorChanger>();
        }

        private void Start()
        {
            foreach (var item in _materialChangers)
            {
                item.SetAlphaInstantly(MinAlpha);
            }
        }

        private void Update()
        {
            bool areInventoryObjectsInHands = RecyclingArea.AreObjectsInRecyclingArea();
            if (areInventoryObjectsInHands != _areInventoryObjectsInHands)
            {
                _areInventoryObjectsInHands = areInventoryObjectsInHands;

                foreach (var item in _materialChangers)
                {
                    item.SetAlphaSmoothly(areInventoryObjectsInHands ? MaxAlpha : MinAlpha);
                }
            }
        }
    }
}
