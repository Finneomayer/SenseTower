using Assets.Scripts.Space;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Player.VR.Input
{
    //возможно стоит в будущем вынести интерфейс, когда добав€тс€ другие VR устройства
    public class VRPlayerInputFocus : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 600)] // sec
        private float _timeToBack = 300;

        private Coroutine _coroutine;
        private SceneChangerView _sceneChanger;


        private void Awake()
        {
            //возможно стоит это вынести отсюда(зинжект?)
            _sceneChanger = FindObjectOfType<SceneChangerView>();


            OVRManager.HMDMounted += OnFocusAcquired;
            OVRManager.HMDUnmounted += OnFocusLost;
        }

        private void OnDestroy()
        {
            OVRManager.HMDMounted -= OnFocusAcquired;
            OVRManager.HMDUnmounted -= OnFocusLost;
        }

        private void OnFocusLost() => _coroutine = StartCoroutine(TimerFocusLost());

        private void OnFocusAcquired()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private IEnumerator TimerFocusLost()
        {
            yield return new WaitForSeconds(_timeToBack);

            _coroutine = null;
            _sceneChanger.ChangeSpace(SpaceType.EnterScene, sceneName:"Ќачальна€ сцена");
        }
    }
}