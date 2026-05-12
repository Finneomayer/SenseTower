using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedGrabbing : NetworkBehaviour
{
    #region Inspector
    public List<XRBaseControllerInteractor> interactables;
    #endregion

    public override void OnNetworkSpawn()
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            interactables[i].selectEntered.AddListener(OnSelectEntered);
        }
    }

    public override void OnDestroy()
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            interactables[i].selectEntered.RemoveAllListeners();
        }
    }

    public void OnSelectEntered(SelectEnterEventArgs eventArgs) 
    {
        Debug.Log("select entered");
        if (IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = eventArgs.interactableObject.transform.root.GetComponent<NetworkObject>();
            
            if (networkObjectSelected != null && networkObjectSelected.OwnerClientId != NetworkManager.Singleton.LocalClientId) 
            {
                RequestGrabbableOwnershipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference) 
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else 
        {
            Debug.Log($"Unable to change ownership for clientid {newOwnerClientId}");
        }
    }

    public void OnSelectExisted()
    {

    }
}
