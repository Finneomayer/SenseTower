using UnityEngine;

namespace Assets.Mechanics.MetaAvatars.Network.Scripts
{
    public class ClientObject : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_SERVER
        gameObject.SetActive(false);
#endif
        }
    }
}
