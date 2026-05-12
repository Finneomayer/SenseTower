using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuplex.WebView;

public class NetworkPresentationLaserPointMover : NetworkBehaviour
{
    [SerializeField] private RectTransform Point;
    [SerializeField] private CanvasPointerInputDetector WebCanvasPointerInputDetector;
    [SerializeField] private CanvasWebViewPrefab CanvasWebViewPrefab;
    [SerializeField] private GameObject[] RaycastObjects;

    private float _lastRefreshedTime = float.MinValue;
    private PresentationLaserActivator _presentationLaserActivator;
    private AdminPlace _adminPlace;

    public void Init(AdminPlace adminPlace)
    {
        _adminPlace = adminPlace;
    }

    public void StartTransmission(PresentationLaserActivator presentationLaserActivator)
    {
        StopAllCoroutines();

        _presentationLaserActivator = presentationLaserActivator;
        StartCoroutine(HidingAfterDelayPointRoutine());
        StartCoroutine(RefreshingPositionRoutine());
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
    private void TransferPositionServerRpc(ulong clientId, Vector3 newPosition)
    {
        if (_adminPlace != null && _adminPlace.IsUserAdmin(clientId))
        {
            TransferPositionClientRpc(newPosition);
        }
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    private void TransferPositionClientRpc(Vector3 newPosition, ClientRpcParams clientRpcParams = default)
    {
        if (_adminPlace != null && !_adminPlace.IsUserAdmin(NetworkManager.LocalClientId))
        {
            RefreshPointPosition(newPosition);
        }
    }

    private void RefreshPointPosition(Vector3 newPosition)
    {
        Point.position = newPosition;
        _lastRefreshedTime = Time.unscaledTime;
        SetPointActive(true);
    }

    private IEnumerator RefreshingPositionRoutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (NetworkManager != null && _adminPlace.IsUserAdmin(NetworkManager.LocalClientId))
            {
                if (_presentationLaserActivator.ActiveInteractor.rayOriginTransform != null &&
                    _presentationLaserActivator.ActiveInteractor.TryGetCurrentUIRaycastResult(
                        out RaycastResult raycastResult))
                {
                    if (raycastResult.gameObject != null && RaycastObjects.Contains(raycastResult.gameObject))
                    {
                        RefreshPointPosition(raycastResult.worldPosition);
                        TransferPositionServerRpc(NetworkManager.LocalClientId, raycastResult.worldPosition);
                    }
                }
                else
                {
                    SetPointActive(false);
                }
            }
        }
    }

    private IEnumerator HidingAfterDelayPointRoutine()
    {
        const float MaxLastRefreshedTimeToDisablePoint = 1;

        SetPointActive(false);

        while (true)
        {
            yield return null;
            if ((Time.unscaledTime - _lastRefreshedTime) > MaxLastRefreshedTimeToDisablePoint)
            {
                SetPointActive(false);
            }
        }
    }

    private void SetPointActive(bool active)
    {
        if (Point.gameObject.activeSelf != active)
        {
            Point.gameObject.SetActive(active);
        }
    }
}