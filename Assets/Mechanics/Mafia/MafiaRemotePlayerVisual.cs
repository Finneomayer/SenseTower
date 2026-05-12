using System;
using System.Collections;
using Assets.Scripts.Client;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Assets.Mechanics.Mafia
{
    public class MafiaRemotePlayerVisual : MonoBehaviour
    {
        [SerializeField] private FollowingObject _headModelsContainer;
        [SerializeField] private GameObject _mask;
        [SerializeField] private GameObject _mouthMask;
        [SerializeField] private GameObject _headphones;
        //[SerializeField] private GameObject _dead;
        //[SerializeField] private ParticleSystem _deathParticles;
        private ClientIdView _clientView;
        //private bool _isDead = false;
        //private Coroutine _deathEffect;
        private Transform _head;
        private Scripts.Shared.NetworkPlayer _player;

        public void Awake()
        {
            _clientView = GetComponent<ClientIdView>();
            _player = GetComponent<Scripts.Shared.NetworkPlayer>();
        }

        private void OnEnable()
        {
            _player.RemoteAvatarLoaded += OnPlayerRemoteAvatarLoaded;
        }

        private void OnDisable()
        {
            _player.RemoteAvatarLoaded -= OnPlayerRemoteAvatarLoaded;
        }

        public string GetPlayerId()
        {
            return _clientView.PlayerAccountId;
        }

        public void SetMaskVisible(bool visible)
        {
            //if (_isDead && visible)
            //{
            //    return;
            //}
            _mask.SetActive(visible);
        }

        public void SetHeadphonesVisible(bool visible)
        {
            _headphones.SetActive(visible);
        }

        public void SetMouthMaskVisible(bool visible)
        {
            _mouthMask.SetActive(visible);
        }

        //public void SetDead(bool dead)
        //{
        //    if (!dead)
        //    {
        //        _dead.SetActive(false);
        //        _isDead = false;
        //        return;
        //    }
        //    else
        //    {
        //        if (_isDead)
        //        {
        //            FindHead();
        //            _dead.SetActive(true);
        //            return;
        //        }
        //        else
        //        {
        //            if (_deathEffect != null) StopCoroutine(_deathEffect);
        //            _deathEffect = StartCoroutine(StartParticles());
        //        }
        //    }
        //}

        //private IEnumerator StartParticles()
        //{
        //    _dead.SetActive(true);
        //    yield return new WaitForSeconds(2f);

        //    _deathParticles.Play();

        //    _isDead = true;
        //    _deathEffect = null;
        //}

        //private void FindHead()
        //{
        //    if (_head == null)
        //    {
        //        _head = transform.Find("Head_jnt");
        //    }

        //    if (_head != null)
        //    {
        //        _dead.transform.SetParent(_head);
        //        _dead.transform.localPosition = new Vector3(0.082f, 0.036f, 0.002f);
        //        _dead.transform.localEulerAngles = Vector3.zero;
        //        _dead.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        //    }
        //}

        private void OnPlayerRemoteAvatarLoaded()
        {
            Transform headTransform = _player.GetRemotePlayerHead();
            if (headTransform == null)
            {
                Debug.Log("MafiaRemotePlayerVisual. HeadTransform == null");
                return;
            }
            _headModelsContainer.Init(headTransform);
        }
    }
}
