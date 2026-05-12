using Unity.Netcode;
using UnityEngine;

public class LeverSwitchHandle : NetworkBehaviour
{
    [SerializeField]
    private Lever lever;
    [SerializeField]
    private LeverSoundFX soundFX;
    [SerializeField]
    private GameObject fireFX;

    public NetworkVariable<bool> IsUp;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            IsUp.Value = true;
            lever.OnSwitchLever.AddListener((x) => OnHandleServerSwitch(x));
        }
        else
        {
            OnHandleSwitch(IsUp.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer)
        {
            lever.OnSwitchLever.RemoveListener((x) => OnHandleServerSwitch(x));
        }       
    }

    protected virtual void OnHandleServerSwitch(bool isUp)
    {
        IsUp.Value = isUp;
        OnHandleSwitchClientClientRpc(isUp);
    }

    [ClientRpc]
    protected virtual void OnHandleSwitchClientClientRpc(bool isUp)
    {
        OnHandleSwitch(isUp);
    }

    protected virtual void OnHandleSwitch(bool isUp)
    {
        if (soundFX != null)
            soundFX.OnSoundHandle(isUp);

        if (fireFX != null)
            fireFX.SetActive(isUp);
    }
}
