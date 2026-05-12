using System;
using Mechanics.UserWallet;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Triggers
{
    public class CoinTriggerObserver : MonoBehaviour
    {
        public event Action<CoinInfrastructure> TriggerEnterInvoke;
        public event Action<CoinInfrastructure> TriggerStayInvoke;
        public event Action<CoinInfrastructure> TriggerExitInvoke;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CoinInfrastructure _coinInfrastructure))
            {
                if (_coinInfrastructure.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
                TriggerEnterInvoke?.Invoke(_coinInfrastructure);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out CoinInfrastructure _coinInfrastructure))
            {
                if (_coinInfrastructure.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
                TriggerStayInvoke?.Invoke(_coinInfrastructure);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CoinInfrastructure _coinInfrastructure))
            {
                if (_coinInfrastructure.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
                
                TriggerExitInvoke?.Invoke(_coinInfrastructure);
            }
        }
    }
}