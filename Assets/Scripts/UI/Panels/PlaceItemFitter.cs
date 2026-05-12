using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace UI
{
    public static class PlaceItemFitter
    {
        public static async UniTask PlaceModel(GameObject itemModel, BoxCollider placeCollider)
        {
            SetTriggerColliders(itemModel, true);
            await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate);

            if (itemModel == null || placeCollider == null)
            {
                return;
            }

            Quaternion itemModelRotationShift = itemModel.transform.rotation
                * Quaternion.Inverse(placeCollider.transform.rotation);

            ClearModel(itemModel);

            MeshRenderer[] meshRenderersToEnable = itemModel.GetComponentsInChildren<MeshRenderer>()
                .Where((mr) => mr.enabled).ToArray();
            foreach (var meshRenderer in meshRenderersToEnable)
            {
                meshRenderer.enabled = false;
            }

            Canvas[] canvasRenderersToEnable = itemModel.GetComponentsInChildren<Canvas>()
                .Where((c) => c.enabled).ToArray();
            foreach (var canvas in canvasRenderersToEnable)
            {
                canvas.enabled = false;
            }

            Vector3 initialColliderPosition = placeCollider.transform.position;
            Quaternion initialColliderRotation = placeCollider.transform.rotation;

            Transform itemParent = itemModel.transform.parent;
            Transform placeColliderParent = placeCollider.transform.parent;

            Vector3 initialParentPosition = Vector3.zero;
            Quaternion initialParentRotation = Quaternion.identity;

            if (placeColliderParent != null)
            {
                initialParentPosition = placeColliderParent.transform.position;
                initialParentRotation = placeColliderParent.transform.rotation;
            }

            itemModel.transform.SetParent(null);
            placeCollider.transform.SetParent(null);

            itemModel.transform.rotation = Quaternion.identity;
            placeCollider.transform.rotation = Quaternion.identity;

            await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate);

            await UniTask.WaitUntil(() => itemModel == null || placeCollider == null
                || Quaternion.Angle(itemModel.transform.rotation, Quaternion.identity) < 1
                && Quaternion.Angle(placeCollider.transform.rotation, Quaternion.identity) < 1);

            if (itemModel == null || placeCollider == null)
            {
                return;
            }

            ResizeModelToFitInventoryPlace(itemModel, placeCollider);

            placeCollider.transform.SetPositionAndRotation(initialColliderPosition, initialColliderRotation);
            itemModel.transform.SetPositionAndRotation(placeCollider.transform.position, placeCollider.transform.rotation);

            await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate);

            await UniTask.WaitUntil(() => itemModel == null || placeCollider == null
                || Vector3.SqrMagnitude(placeCollider.transform.position - initialColliderPosition) < 0.01f
                && Vector3.SqrMagnitude(itemModel.transform.position - placeCollider.transform.position) < 0.01f
                && Quaternion.Angle(itemModel.transform.rotation, initialColliderRotation) < 1
                && Quaternion.Angle(placeCollider.transform.rotation, initialColliderRotation) < 1);

            if (itemModel == null || placeCollider == null)
            {
                return;
            }

            itemModel.transform.SetParent(itemParent);
            placeCollider.transform.SetParent(placeColliderParent);

            float distFromModelBottomToCenter = itemModel.transform.position.y - GetModelDimensions(itemModel).min.y;
            PlaceModel(itemModel, placeCollider, distFromModelBottomToCenter);

            SetEnabledColliders(itemModel, false);

            Vector3 positionShift = Vector3.zero;
            Quaternion rotationShift = Quaternion.identity;

            if (placeColliderParent != null)
            {
                positionShift = placeColliderParent.transform.position - initialParentPosition;
                rotationShift = placeColliderParent.transform.rotation * Quaternion.Inverse(initialParentRotation);
            }

            placeCollider.transform.SetPositionAndRotation(
                placeCollider.transform.position + positionShift, rotationShift * placeCollider.transform.rotation);
            itemModel.transform.SetPositionAndRotation(
                itemModel.transform.position + positionShift, rotationShift * itemModel.transform.rotation);

            itemModel.transform.rotation = itemModelRotationShift * itemModel.transform.rotation;

            foreach (var meshRenderer in meshRenderersToEnable)
            {
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                }
            }

            foreach (var canvas in canvasRenderersToEnable)
            {
                if (canvas != null)
                {
                    canvas.enabled = true;
                }
            }

            SetTriggerColliders(itemModel, false);
        }

        public static void ClearModel(GameObject itemModel)
        {
            DestroyUnwantedComponents(itemModel);
        }

        private static void DestroyUnwantedComponents(GameObject itemModel)
        {
            var rigidbodies = itemModel.GetComponentsInChildren<Rigidbody>();
            foreach (var item in rigidbodies)
            {
                item.useGravity = false;
                item.isKinematic = true;
                //UnityEngine.Object.Destroy(item);
            }
        }

        private static void SetEnabledColliders(GameObject itemModel, bool enabled)
        {
            var colliders = itemModel.GetComponentsInChildren<Collider>();
            foreach (var item in colliders)
            {
                item.enabled = enabled;
            }
        }

        private static void SetTriggerColliders(GameObject itemModel, bool isTrigger)
        {
            var colliders = itemModel.GetComponentsInChildren<Collider>();
            foreach (var item in colliders)
            {
                item.isTrigger = isTrigger;
            }
        }

        private static void ResizeModelToFitInventoryPlace(GameObject go, BoxCollider placeCollider)
        {
            Vector3 placeScale = Vector3.Scale(placeCollider.size, placeCollider.transform.lossyScale);
            Bounds modelBounds = GetModelDimensions(go);

            float scaleFactor;
            if (modelBounds.size.x > placeScale.x)
            {
                scaleFactor = placeScale.x / modelBounds.size.x;
                go.transform.localScale = go.transform.lossyScale * go.transform.localScale.x / go.transform.lossyScale.x
                                          * scaleFactor;
                modelBounds.size *= scaleFactor;
            }
            if (modelBounds.size.y > placeScale.y)
            {
                scaleFactor = placeScale.y / modelBounds.size.y;
                go.transform.localScale = go.transform.lossyScale * go.transform.localScale.y / go.transform.lossyScale.y
                                          * scaleFactor;
                modelBounds.size *= scaleFactor;
            }
            if (modelBounds.size.z > placeScale.z)
            {
                scaleFactor = placeScale.z / modelBounds.size.z;
                go.transform.localScale = go.transform.lossyScale * go.transform.localScale.z / go.transform.lossyScale.z
                                          * scaleFactor;
                modelBounds.size *= scaleFactor;
            }
        }

        private static void PlaceModel(GameObject go, BoxCollider placeCollider, float distFromModelBottomToCenter)
        {
            if (go == null || distFromModelBottomToCenter == 0)
            {
                return;
            }

            float bottomPointY = placeCollider.transform.position.y + placeCollider.center.y - placeCollider.bounds.extents.y;

            Vector3 newPosition = placeCollider.transform.position + placeCollider.center;
            newPosition.y = bottomPointY + distFromModelBottomToCenter;

            if (Mathf.Abs(newPosition.y - go.transform.position.y) > 0.01f)
            {
                go.transform.position = newPosition;
            }
        }

        private static Bounds GetModelDimensions(GameObject go)
        {
            Collider[] colliders = go.GetComponentsInChildren<Collider>();

            Vector3 minPoint = float.MaxValue * Vector3.one;
            Vector3 maxPoint = float.MinValue * Vector3.one;
            foreach (var collider in colliders)
            {
                Bounds colliderBounds = collider.bounds;
                minPoint = Vector3.Min(minPoint, colliderBounds.min);
                maxPoint = Vector3.Max(maxPoint, colliderBounds.max);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(minPoint, maxPoint);
            return bounds;
        }
    }
}