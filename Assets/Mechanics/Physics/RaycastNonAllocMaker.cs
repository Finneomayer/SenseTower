using UnityEngine;

namespace Assets.Mechanics.Physics
{
    public class RaycastNonAllocMaker
    {
        public RaycastHit[] LastHits { get; private set; }
        public int LastHitCount { get; private set; }

        private Ray _ray;

        public RaycastNonAllocMaker(int maxTouchCount = 20)
        {
            LastHits = new RaycastHit[maxTouchCount];
            LastHitCount = 0;
            _ray = new Ray();
        }

        public int Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
        {
            _ray.origin = origin;
            _ray.direction = direction;
            LastHitCount = UnityEngine.Physics.RaycastNonAlloc(_ray, LastHits, maxDistance, layerMask);
            return LastHitCount;
        }
    }
}
