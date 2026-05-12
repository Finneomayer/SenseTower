using UnityEngine;
using Oculus.Avatar2;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class GlobalAvatarManager : MonoBehaviour
    {
        [field: SerializeField]
        public OvrAvatarBodyTrackingBehavior OvrAvatarBodyTrackingBehavior { get; private set; }
    }
}
