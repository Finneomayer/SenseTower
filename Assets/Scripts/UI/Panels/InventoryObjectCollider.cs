using UI;
using UnityEngine;

public class InventoryObjectCollider : MonoBehaviour
{
    public InventoryObject Object { get; private set; }

    public void Init(InventoryObject obj)
    {
        Object = obj;
    }
}