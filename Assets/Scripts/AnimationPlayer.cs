using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] private Animation animationChair;

    public void Play()
    {
        animationChair.Play();
    }
}
