using Assets.Scripts.Zones;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimplePlacesZoneController : MonoBehaviour
{
    [SerializeField]
    private ZonesModel ZonesModel;
    [SerializeField]
    private Transform PlacesContent;
    
    private void Awake()
    {
        Place[] places = PlacesContent.GetComponentsInChildren<Place>();
        foreach (var item in places)
        {
            item.Init(ZonesModel, null);
        }
    }
}
