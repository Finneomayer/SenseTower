using UnityEngine;

public class AirplanePropellerRotation : MonoBehaviour
{
    [SerializeField]
    private float angle = 21;

    private void Update() => transform.Rotate(Vector3.up, angle);
}
