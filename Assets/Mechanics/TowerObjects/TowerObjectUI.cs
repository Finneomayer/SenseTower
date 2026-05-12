using System;
using Assets.Mechanics.NetworkInteraction;
using Assets.Scripts.Client;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class TowerObjectUI : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private TMP_Text _tittle;
        [SerializeField] private LookAtPlayer _player;
        [SerializeField] private Canvas _parentCanvas;
        #endregion

        private NetworkXrGrab _networkXrGrab;
        private Transform _playerTransorm;

        private string _ownerId = string.Empty;

        private IClientData _clientData;
        
        [Inject]
        private void Contruct(IClientData clientData)
        {
            _clientData = clientData;
        }

        private void OnEnable()
        {
            if (_playerTransorm == null)
            {
                _playerTransorm = Camera.main.transform.root;
                _player.SetPlayer(_playerTransorm);
            }

            _parentCanvas.enabled = false;
            if (_networkXrGrab == null)
                return;

            _networkXrGrab.StartGrab += OnStartGrab;
            _networkXrGrab.StopGrab += OnStopGrab;
        }

        private void OnDisable()
        {
            if (_networkXrGrab == null)
                return;
            
            _networkXrGrab.StartGrab -= OnStartGrab;
            _networkXrGrab.StopGrab -= OnStopGrab;
        }

        public void OnActionListener(NetworkXrGrab networkXrGrab)
        {
            networkXrGrab.StartGrab += OnStartGrab;
            networkXrGrab.StopGrab += OnStopGrab;
        }

        public void SetText(string tittle)
        {
            if (string.IsNullOrEmpty(tittle))
                tittle = "Владелец";
            
            _tittle.text = tittle;
        }

        public void SetParent(GameObject parentObj)
        {
            gameObject.transform.SetParent(parentObj.transform);
        }

        public void SetOwner(string ownerId)
        {
            _ownerId = ownerId;
        }

        private void OnStartGrab()
        {
            if (_clientData.UserId.ToString().Equals(_ownerId))
                return;
            
            if (_playerTransorm == null)
            {
                _playerTransorm = Camera.main.transform.root;
                _player.SetPlayer(_playerTransorm);
            }
            
            _parentCanvas.enabled = true;
        }

        private void OnStopGrab()
        {
            _parentCanvas.enabled = false;
        }
    }
}