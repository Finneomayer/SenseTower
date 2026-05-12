using UnityEngine;

namespace Assets.Scripts.Shared
{
    public class NetworkTransformTransmitter : BaseNetworkTransformTransmitter
    {
        protected override Vector3 GetTargetPosition()
        {
            return TargetTransform.position;
        }

        protected override void SetTargetPosition(Vector3 position)
        {
            TargetTransform.position = position;
        }

        protected override Quaternion GetTargetRotation()
        {
            return TargetTransform.rotation;
        }

        protected override void SetTargetRotation(Quaternion rotation)
        {
            TargetTransform.rotation = rotation;
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