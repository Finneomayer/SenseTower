using UnityEngine;

namespace Assets.Scripts.Shared
{
    public class NetworkLocalTransformTransmitter : BaseNetworkTransformTransmitter
    {
        protected override Vector3 GetTargetPosition()
        {
            return TargetTransform.localPosition;
        }

        protected override void SetTargetPosition(Vector3 position)
        {
            TargetTransform.localPosition = position;
        }

        protected override Quaternion GetTargetRotation()
        {
            return TargetTransform.localRotation;
        }

        protected override void SetTargetRotation(Quaternion rotation)
        {
            TargetTransform.localRotation = rotation;
        }

        protected override Vector3 GetTargetScale()
        {
            return TargetTransform.localScale;
        }

        protected override void SetTargetScale(Vector3 scale)
        {
            TargetTransform.localScale = scale;
        }
    }
}
