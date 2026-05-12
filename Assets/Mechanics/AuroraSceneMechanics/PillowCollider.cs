using Assets.Mechanics.NetworkInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowCollider : MonoBehaviour
{
    private Rigidbody _rb;
    private List<PillowCollider> _neighbors = new();

    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out PillowCollider otherPillow))
        {
            return;
        }

        if (otherPillow._rb == null)
        {
            return;
        }

        _neighbors.Add(otherPillow);

        if (_rb.useGravity && !otherPillow._rb.useGravity)
        {
            otherPillow.EnablePhysics();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out PillowCollider otherPillow))
        {
            return;
        }
        _neighbors.Remove(otherPillow);
    }

    private void EnablePhysics()
    {
        if (gameObject.TryGetComponent(out NetworkXrGrab networkXrGrab))
        {
            networkXrGrab.SetLocalUserOwnership();
        }
        _rb.useGravity = true;
        _rb.isKinematic = false;
        foreach (var item in _neighbors)
        {
            if (!item._rb.useGravity)
            {
                item.EnablePhysics();
            }
        }
    }
}
