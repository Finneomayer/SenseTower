using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class FollowingObject : MonoBehaviour
    {
        private Transform _objToFollow;

        public void Init(Transform objToFollow)
        {
            if (objToFollow == null)
            {
                return;
            }
            _objToFollow = objToFollow;
        }

        public void LateUpdate()
        {
            if (_objToFollow == null)
            {
                return;
            }
            transform.SetPositionAndRotation(_objToFollow.position, _objToFollow.rotation);
        }
    }
}
