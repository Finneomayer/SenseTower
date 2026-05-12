using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    #region Inspector
    [SerializeField] private bool _needRotation = true;
    [SerializeField] private Transform _player;
    [Space]
    [SerializeField] private float maxAngle = 45;
	[SerializeField] private float maxDistance = 2;
	[SerializeField] private float minDistance = 0.1f;
	[SerializeField] private float Damping;
    [SerializeField] private float _maxVerticalAngle = 10;
	public float YRotationOffset;
	public Vector3 Offset;
	#endregion


	private bool _isFollow;
	

	void Update()
	{
		if (_player != null)
        {
            var playerPos = _player.position;
            var playerForward = _player.forward;

            if (_needRotation)
            {
                RotateObject(playerPos);
            }

            if (_isFollow)
            {
                TranslateFrontOfPlayer(playerPos, playerForward);
            }
        }
    }

    public void SetPlayer(Transform player)
    {
        _player = player;

        if (_player != null)
        {
            Camera camera = _player.GetComponentInChildren<Camera>();
            if (camera != null) _player = camera.transform;
        }
    }        
	
	public void DeletePlayer() => SetPlayer(null);

	public void SetPlayerFollow(bool isFollow) => _isFollow = isFollow;

	public void SetFirstPosition()
	{
		Vector3 _destinationPos = _player.TransformPoint(Offset);
		_destinationPos.y = _player.position.y + Offset.y;
		transform.position = _destinationPos;

        if (!_needRotation)
        {
            transform.forward = new Vector3(_player.forward.x, 0, _player.forward.z);
        }
    }

    private void RotateObject(Vector3 playerPos)
    {
        var lookPos = transform.position - playerPos;

        lookPos.y = YRotationOffset;
        var rotation = Quaternion.LookRotation(lookPos);

        if (rotation.eulerAngles.x > _maxVerticalAngle) 
            rotation = Quaternion.Euler(_maxVerticalAngle, rotation.eulerAngles.y, rotation.eulerAngles.z);
        if (rotation.eulerAngles.x < -_maxVerticalAngle) 
            rotation = Quaternion.Euler(-_maxVerticalAngle, rotation.eulerAngles.y, rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotation,
            Time.deltaTime * Damping);      
    }

    private void TranslateFrontOfPlayer(Vector3 playerPos, Vector3 targetForward)
    {
        var normalize = targetForward;
        normalize.y = 0;
        normalize = Vector3.Normalize(normalize);

        Vector3 _destinationPos = normalize * new Vector3(Offset.x, 0, Offset.z).magnitude + playerPos;
        _destinationPos.y = playerPos.y + Offset.y;

        if (Vector3.Angle(targetForward, transform.forward) > maxAngle
            || Vector3.Distance(_player.position, transform.position) > maxDistance
            || Vector3.Distance(_player.position, transform.position) < minDistance)
        {
            transform.position = Vector3.Lerp(transform.position, _destinationPos, Time.deltaTime * Damping);
        }

        Vector3 pos = transform.position;
        pos.y = _destinationPos.y;
        transform.position = pos;
    }
}
