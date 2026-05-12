using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlaceReconnector : MonoBehaviour
{
    private List<Place> _places = new();

    public ulong? GetOccupiedPlaceNetworkObjectId()
    {
        foreach (var place in _places)
        {
            if (place.IsOccupiedID.Value == NetworkManager.Singleton.LocalClientId)
            {
                return place.NetworkObjectId;
            }
        }
        return null;
    }

    public async UniTask RestoreInPlace(Assets.Scripts.Shared.NetworkPlayer networkPlayer, ulong occupiedPlaceNetworkObjectId)
    {
        const float MaxSitSearchTime = 5;

        float endSitSearchTime = Time.time + MaxSitSearchTime;
        while (NetworkManager.Singleton != null && Time.time < endSitSearchTime)
        {
            foreach (var place in _places)
            {
                if (place.NetworkObjectId == occupiedPlaceNetworkObjectId)
                {
                    if (place.IsOccupiedID.Value != 0 && place.IsOccupiedID.Value != NetworkManager.Singleton.LocalClientId)
                    {
                        return;
                    }

                    place.LeaveAndOccupyPlace();
                    Debug.Log("Restored in place");

                    return;
                }
            }

            await UniTask.Delay(1000);
        }

        Debug.LogWarning("Not restored in place: not found place");
    }

    private void OnEnable()
    {
        Place.PlaceSpawned += OnPlaceSpawned;
        Place.PlaceDespawned += OnPlaceDespawned;
    }

    private void OnDisable()
    {
        Place.PlaceSpawned -= OnPlaceSpawned;
        Place.PlaceDespawned -= OnPlaceDespawned;
    }

    private void OnPlaceSpawned(Place place)
    {
        _places.Add(place);
    }

    private void OnPlaceDespawned(Place place)
    {
        _places.Remove(place);
    }
}
