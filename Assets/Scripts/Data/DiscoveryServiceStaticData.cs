using API.Models;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "DiscoveryServiceData", menuName = "Static Data/DiscoveryServiceData")]
    public class DiscoveryServiceStaticData : ScriptableObject
    {
        public Assembly Assembly;
        public bool DebugMode;
    }
}
