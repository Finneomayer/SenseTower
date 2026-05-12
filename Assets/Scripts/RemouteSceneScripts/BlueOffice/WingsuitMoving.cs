using System.Collections;
using UnityEngine;

public class WingsuitMoving : MonoBehaviour
{

    [SerializeField]
    private Transform _start;


    [SerializeField]
    private Transform _end;


    [SerializeField]
    private float _speed;

    private Vector3 _direction;
    private void Start()
    {
        _direction = (_end.position - _start.position).normalized;
        transform.position = _start.position;
        transform.LookAt(_end);
        StartCoroutine(WingsuitMove());
    }

    private IEnumerator WingsuitMove()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, _end.position) < 1f)
            {
                transform.position = _start.position;
            }
            Debug.DrawRay(transform.position, _direction);
            transform.position += _speed * _direction;
            yield return null;
        }
    }
}
