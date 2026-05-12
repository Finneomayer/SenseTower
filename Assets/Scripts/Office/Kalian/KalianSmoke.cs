using System.Collections;
using UnityEngine;

public class KalianSmoke : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particle;

    [SerializeField]
    private float _timeSmokePause = 7;

    [SerializeField]
    private string _objectName = "Head_jnt";

    private bool _canPlay = false;
    [SerializeField]
    private AudioSource _smokeAudio;

    private ParticleSystem _currentParticle;
    private Transform _currentTarget;
    
    private void Start()
    {
        StartCoroutine(PlaySmoke());
    }

    private void CreateParticle()
    {
        DestroyParticle();
        _currentParticle = Instantiate(_particle);
    }

    private void DestroyParticle()
    {
        if(_currentParticle != null)
            Destroy(_currentParticle.gameObject);
    }

    private void SetParticlePosition(Transform target)
    {
        _currentParticle.transform.parent = target;
        _currentParticle.gameObject.transform.position = target.position + target.up * 0.15f;
        _currentParticle.gameObject.transform.rotation = Quaternion.LookRotation(target.right, target.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if /*(other.gameObject.layer == LayerMask.NameToLayer("MirrorPlayer") &&*/ (other.gameObject.name == _objectName)
        {
            if (_currentParticle == null)
                CreateParticle();
            _currentTarget = other.transform;
            SetParticlePosition(other.transform);
            _canPlay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ( other.gameObject.name == _objectName)
        {
            _smokeAudio.Stop();
            if(_currentParticle !=null)
                _currentParticle.Stop();
            
            _currentTarget = null;

            _canPlay = false;
        }
    }

    private IEnumerator PlaySmoke()
    {
        while (true)
        {
            if (_canPlay)
            {
                if (_currentParticle == null)
                {
                    CreateParticle();
                    SetParticlePosition(_currentTarget);
                }

                _currentParticle.Play();
                _smokeAudio.Play();
                yield return new WaitForSeconds(_timeSmokePause);
                DestroyParticle();
            }
            yield return null;
        }
    }
}
