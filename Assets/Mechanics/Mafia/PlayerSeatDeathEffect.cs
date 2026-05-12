using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatDeathEffect : MonoBehaviour
    {
        [SerializeField] private GameObject _effectContainer;
        [SerializeField] private AudioSource _sound;
        [SerializeField] private Animation _ghostMoving;
        [SerializeField] private Animation _coneColor;
        [SerializeField] private Coroutine _deathCoroutine;

       public void Init(PlayerState playerSeatData, bool wasDead)
        {
            if (wasDead) return;

            if (!playerSeatData.IsAlive)
            {
                ShowDeathEffect();
            }
        }

        [ContextMenu("OnEditorShowEffect")]
        private void ShowDeathEffect()
        {
            if (_deathCoroutine != null) StopCoroutine(_deathCoroutine);
            _deathCoroutine = StartCoroutine(DeathCoroutine());
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _effectContainer.SetActive(true);

            _sound.Play();
            _ghostMoving.Play();
            _coneColor.Play();

            yield return new WaitForSeconds(_ghostMoving.clip.length);
            _effectContainer.SetActive(false);

            _deathCoroutine = null;
        }
    }
}
