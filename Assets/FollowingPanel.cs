using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingPanel : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float maxDistance = 2;
    [SerializeField]
    private float maxAngle = 45;


    void Start()
    {
        StartCoroutine(CorrectPositionAndRotation()); 
    }

    private IEnumerator CorrectPositionAndRotation()
    {
        while (true)
        {
            Vector3 targetForward = target.forward;
            Vector3 targetPosition = target.position;

            if (Vector3.Angle(targetForward, transform.forward) > maxAngle
                || (targetPosition - transform.position).magnitude > maxDistance)
            {
                while (Vector3.Angle(targetForward, transform.forward) > 1f
                    || (targetPosition - transform.position).magnitude > 0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
                    transform.forward = Vector3.Lerp(transform.forward, targetForward, Time.deltaTime * 2f);
                    yield return null;
                }
            }


            yield return null;
        }
    }
}
