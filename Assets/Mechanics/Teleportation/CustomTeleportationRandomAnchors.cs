using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;

namespace Sense.Interectable.Teleportation
{
    public class CustomTeleportationRandomAnchors : CustomTeleportationInteractable
    {
        [SerializeField]
        protected List<Transform> _anchorsCollection = new List<Transform>();

        public void SetLocalTeleports(Transform[] targets, Collider collider)
        {
            _anchorsCollection.Clear();
            _anchorsCollection = targets.ToList();

            colliders.Clear();
            colliders.Add(collider);
        }

        public override Transform TeleportAnchorTransform 
        {
            get  => _anchorsCollection[Random.Range(0, _anchorsCollection.Count)];
            set { /* _anchorsCollection[0] = value*/ } 
        }

        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (TeleportAnchorTransform == null || TeleportActivateR.action.inProgress || TeleportActivateL.action.inProgress)
                return false;

            teleportRequest.destinationPosition = TeleportAnchorTransform.position;
            teleportRequest.destinationRotation = TeleportAnchorTransform.rotation;
            return true;
        }

        protected override void OnValidate()
        {
            if (_anchorsCollection == null || _anchorsCollection.Count.Equals(0))
                _anchorsCollection = new List<Transform> { transform };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (interactionManager == null)
                interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        protected override void Awake()
        {
            base.Awake();
            if (interactionManager == null)
                interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        protected override void Reset()
        {
            base.Reset();
            _anchorsCollection = new List<Transform> { transform };
        }

#if UNITY_EDITOR
        //protected Vector3 _anchorSize = new Vector3(.33f, .33f, .33f);
        //protected void OnDrawGizmos()
        //{
        //    if (_anchorsCollection == null || _anchorsCollection.Count.Equals(0))
        //        return;

        //    foreach (var anchor in _anchorsCollection)
        //    {
        //        DrawBezierLine(anchor);

        //        Gizmos.color = Color.yellow;
        //        Gizmos.DrawWireCube(anchor.position, _anchorSize);
        //    }

        //}


        //private void DrawBezierLine(Transform anchor)
        //{
        //    Vector3 managerPos = transform.position;
        //    Vector3 anchorPos = anchor.position;
        //    Vector3 offset = Vector3.up * 1;
        //    Gizmos.color = Color.red;

        //    Handles.DrawBezier(
        //        managerPos,
        //        anchorPos,
        //        managerPos - offset,
        //        anchorPos + offset,
        //        Color.red,
        //        EditorGUIUtility.whiteTexture,
        //        1f);
        //}
#endif
    }

}