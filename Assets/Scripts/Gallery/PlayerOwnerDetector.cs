using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Assets.Scripts.Shared;
using UnityEditor;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

public class PlayerOwnerDetector : MonoBehaviour
{
    public Action<NetworkPlayer> PlayerEnter;
    public Action PlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<NetworkPlayer>();
        if (player != null && player.PlayerIsOwner) PlayerEnter?.Invoke(player);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<NetworkPlayer>();
        if (player != null && player.PlayerIsOwner) PlayerExit?.Invoke();
    }
}
