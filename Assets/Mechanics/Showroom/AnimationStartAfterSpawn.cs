using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.Showroom
{
    public class AnimationStartAfterSpawn : MonoBehaviour
    {
        [SerializeField] private SceneChangerView _sceneChanger;
        [SerializeField] private float _delay;
        [SerializeField] private Animation _animation;
        [SerializeField] private AudioSource _sound;

        private Coroutine _delayCoroutine;

        private void Start()
        {
            if (_sceneChanger != null)
            {
                _sceneChanger.PlayerInited += _sceneChanger_PlayerInited;
            }
        }

        private void OnDisable()
        {
            _sceneChanger.PlayerInited -= _sceneChanger_PlayerInited;
        }

        private void _sceneChanger_PlayerInited()
        {
            if (_delayCoroutine == null) _delayCoroutine = StartCoroutine(DelayCoroutine());
        }

        private IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(_delay);
            _animation.Play();
            _sound.Play();
        }
    }
}
