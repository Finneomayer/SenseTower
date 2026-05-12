using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class WindowsPhysicsRaycaster : PhysicsRaycaster
    {
        [SerializeField]
        private float MaxDistance;

        protected override void Awake()
        {
            base.Awake();
            if (Application.platform == RuntimePlatform.Android)
            {
                enabled = false;
                return;
            }
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return;
            }
            base.Raycast(eventData, resultAppendList);

            for (int i = resultAppendList.Count - 1; i > 0; i--)
            {
                resultAppendList.RemoveAt(i);
            }

            resultAppendList.RemoveAll(result => result.distance > MaxDistance);
        }
    }
}