using UnityEngine;

public class LeverSoundFX : MonoBehaviour
{
    public AudioClip leverDown;
    public AudioClip leverUp;

    [SerializeField]
    private AudioSource audioSource;

    public void OnSoundHandle(bool IsUp)
    {
#if UNITY_SERVER
        return;
#endif
        if (audioSource == null) return;

        audioSource.clip = IsUp ? leverUp : leverDown;
        audioSource.Play();

    }
}
