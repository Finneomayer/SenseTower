using Data;
using UnityEngine;

namespace Assets.Mechanics.DevDebug
{
    public class OnlyDevWorkingObject : MonoBehaviour
    {
        [SerializeField] private DiscoveryServiceStaticData _contour;

        private void Awake()
        {
#if !UNITY_EDITOR
            if (_contour != null)
            {
                gameObject.SetActive(_contour.DebugMode);
            }
#endif
        }
    }
}
