using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

namespace Assets.Mechanics.UserWallet
{
    public class EnterSceneCoin : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioSource _sound;
        [SerializeField] private GameObject _coinVisual;
        [SerializeField] private XRGrabInteractable _interactable;
        [SerializeField] private GameObject _vrAdvice;
        [SerializeField] private GameObject _winAdvice;

        public event Action OnRecieveCoin;

        private CancellationTokenSource _cts;

        private void Start()
        {
            _cts = new CancellationTokenSource();
            _interactable.selectExited.AddListener(OnSelectExit);

            bool isNotVr = XRGeneralSettings.Instance != null &&
                        !XRGeneralSettings.Instance.Manager.isInitializationComplete;

            _vrAdvice.SetActive(!isNotVr);
            _winAdvice.SetActive(isNotVr);
        }

        private void OnSelectExit(SelectExitEventArgs arg0)
        {
            RecieveCoinAsync(_cts.Token).Forget();
        }

        private async UniTask RecieveCoinAsync(CancellationToken cancelToken)//https://github.com/Cysharp/UniTask#cancellation-and-exception-handling
        {
            _sound.Play(); 

            await UniTask.WaitUntil(() => !_sound.isPlaying, cancellationToken: cancelToken);

            OnRecieveCoin?.Invoke();
        }

        private void OnDestroy()
        {
            _cts.Cancel();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _coinVisual.SetActive(false);

            RecieveCoinAsync(_cts.Token).Forget();
        }
    }
}
