using UnityEngine;

namespace Assets.Blackboard
{
    public class SelectButton<T> : ButtonUI
    {
        [field: SerializeField]
        public T SelectValue { get; private set; }
    }
}
