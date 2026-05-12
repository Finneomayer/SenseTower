using System;
using System.Collections.Generic;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

namespace Mechanics.CustomScenario
{
    public class MetaConfScenario : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private string _spaceId;
        [SerializeField] private List<string> _speakerId;
        [SerializeField] private Button _button;
        [SerializeField] private HandButton _handButton;
        [SerializeField] private List<GameObject> _firstObjects = new();
        [SerializeField] private List<GameObject> _secondObjects = new();

        #endregion

        private IClientData _clientData;
        private ISpaceManager _spaceManager;

        private bool _isScenarioComplete = false;

        [Inject]
        private void Construct(IClientData clientData, ISpaceManager spaceManager)
        {
            _spaceManager = spaceManager;
            _clientData = clientData;
        }

        private void OnEnable()
        {
            _handButton.selectEntered.AddListener(OnButtonSelectHandler);
            _button.onClick.AddListener(ChangeScenarioStateServerRpc);
        }

        private void OnDisable()
        {
            _button.onClick.AddListener(ChangeScenarioStateServerRpc);
        }

        public override void OnNetworkSpawn()
        {
            if (_spaceId.Equals(_spaceManager.CurrentTransitionTarget.Id.ToString()))
            {
                GetCurrentScenarioStateServerRpc(NetworkManager.Singleton.LocalClientId);
                _handButton.gameObject.SetActive(_speakerId.Contains(_clientData.UserId.ToString()));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnButtonSelectHandler(SelectEnterEventArgs arg0)
        {
            ChangeScenarioStateServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeScenarioStateServerRpc()
        {
            _isScenarioComplete = !_isScenarioComplete;
            SendStateToAllUsers();
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetCurrentScenarioStateServerRpc(ulong clientId)
        {
            SendStateToUser(clientId);
        }

        private void SendStateToAllUsers()
        {
            ChangeScenarioStateClientRpc(_isScenarioComplete);
        }

        private void SendStateToUser(ulong clientId)
        {
            ChangeScenarioStateClientRpc(_isScenarioComplete, clientId);
        }

        [ClientRpc]
        private void ChangeScenarioStateClientRpc(bool isScenarioComplete, ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                _isScenarioComplete = isScenarioComplete;
                ChangeScenarioStateByClient();
            }
        }

        [ClientRpc]
        private void ChangeScenarioStateClientRpc(bool isScenarioComplete)
        {
            _isScenarioComplete = isScenarioComplete;
            ChangeScenarioStateByClient();
        }

        private async void ChangeScenarioStateByClient()
        {
            Sequence sequence = DOTween.Sequence();
            _firstObjects.ForEach(element => element.gameObject.SetActive(true));
            _secondObjects.ForEach(element => element.gameObject.SetActive(true));

            if (_isScenarioComplete)
            {
                foreach (GameObject firstObject in _firstObjects)
                {
                    sequence.Join(firstObject.transform.DOScale(Vector3.zero, 1f));
                }

                foreach (GameObject secondObject in _secondObjects)
                {
                    sequence.Join(secondObject.transform.DOScale(Vector3.one, 1f));
                }
            }
            else
            {
                foreach (GameObject firstObject in _firstObjects)
                {
                    sequence.Join(firstObject.transform.DOScale(Vector3.one, 1f));
                }

                foreach (GameObject secondObject in _secondObjects)
                {
                    sequence.Join(secondObject.transform.DOScale(Vector3.zero, 1f));
                }
            }

            sequence.Play();

            await UniTask.Delay(2000);
            _firstObjects.ForEach(element => element.gameObject.SetActive(!_isScenarioComplete));
            _secondObjects.ForEach(element => element.gameObject.SetActive(_isScenarioComplete));
        }
    }
}