using UnityEngine;

public class AirplaneSoundOcclision : MonoBehaviour
{

    [SerializeField]
    private AudioSource _airpane;
    [SerializeField]
    private float _maxVolume = 1;
    [SerializeField]
    private float _minVolume = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            _airpane.volume = _minVolume;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            _airpane.volume = _maxVolume;
        }
    }
}
