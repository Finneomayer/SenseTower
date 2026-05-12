using System.Collections;
using UnityEngine;

namespace Sense.Interectable.Watchs
{
    public abstract class Watch : MonoBehaviour
    {
        protected WaitForSeconds _waiting = new WaitForSeconds(1);
        protected void Start()
        {
            StartCoroutine(UpdateTime());
        }

        protected IEnumerator UpdateTime()
        {
            while (true)
            {
                UpdateTimeInfo();
                yield return _waiting;
            }
        }

        protected abstract void UpdateTimeInfo();

    }
}