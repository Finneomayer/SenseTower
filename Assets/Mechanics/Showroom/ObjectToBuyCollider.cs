using UnityEngine;

public class ObjectToBuyCollider : MonoBehaviour
{
    public ObjectToBuy Object { get; private set; }

    public void Init(ObjectToBuy obj)
    {
        Object = obj;
    }
}
