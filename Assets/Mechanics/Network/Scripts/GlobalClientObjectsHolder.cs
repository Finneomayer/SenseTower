using UnityEngine;

namespace Assets.Mechanics.MetaAvatars.Network.Scripts
{
    public class GlobalClientObjectsHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _prefabsToInstantiate;

        public static GlobalClientObjectsHolder _instance;

        private void Awake()
        {
#if UNITY_SERVER
        gameObject.SetActive(false);
        return;
#endif

            if (_instance == null)
            {
                _instance = this;
                foreach (var item in _prefabsToInstantiate)
                {
                    Instantiate(item, transform);
                }
                transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
