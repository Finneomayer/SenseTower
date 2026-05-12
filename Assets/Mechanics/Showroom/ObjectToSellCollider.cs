using UnityEngine;

public class ObjectToSellCollider : MonoBehaviour
{
    public ObjectToSell Object { get; private set; }

    public void Init(ObjectToSell obj)
    {
        Object = obj;
    }
}
