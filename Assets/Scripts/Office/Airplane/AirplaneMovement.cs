using PathCreation;
using UnityEngine;

public class AirplaneMovement : MonoBehaviour
{
    [SerializeField]
    private float _currentSpeed = 10;
    [SerializeField]
    private PathCreator _pathCreator;
    [SerializeField]
    private EndOfPathInstruction _endOfPathInstruction;

    float distanceTravelled = 0;

    private void FixedUpdate()
    {
        if (_pathCreator != null)
        {
            distanceTravelled -= _currentSpeed * Time.deltaTime;
            transform.position = _pathCreator.path.GetPointAtDistance(distanceTravelled, _endOfPathInstruction);
            var rot = _pathCreator.path.GetRotationAtDistance(distanceTravelled, _endOfPathInstruction);
            rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0);
            transform.rotation = rot;
        }
    }
}
