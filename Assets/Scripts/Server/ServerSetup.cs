using UnityEngine;

namespace Assets.Scripts.Server
{
    public class ServerSetup : MonoBehaviour
    {
        private const int ServerTargetFrameRate = 50;

        void Start()
        {
#if UNITY_SERVER
        Application.targetFrameRate = ServerTargetFrameRate;
#endif
        }
    }
}
