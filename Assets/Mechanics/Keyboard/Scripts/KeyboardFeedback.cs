using UnityEngine;

namespace Assets.Mechanics.Keyboard.Scripts
{
    public class KeyboardFeedback : MonoBehaviour
    {
        [SerializeField]
        private AudioSource KeyDownSound;

        public void PlayKeyDownFeedback()
        {
            if (KeyDownSound.isPlaying)
            {
                KeyDownSound.Stop();
            }
            KeyDownSound.Play();
        }
    }
}
