using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Trading;
using Assets.Mechanics.Triggers;
using Infrastructure.Factory;
using Mechanics.UserWallet;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Assets.Localization;
using Assets.Scripts.TowerObjects;
using UnityEngine.XR.Interaction.Toolkit;
using Assets.Mechanics.Network.Scripts;

public class SellZone : NetworkBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private SellZoneTrigger _trigger;
    [SerializeField] private CoinTriggerObserver _coinTrigger;
    [SerializeField] private NetworkFactory _networkFactory;
    [SerializeField] private Transform _coinSpawnPoint;
    [SerializeField] private NetworkVariable<ulong> _occupiedID = new(0);
    [SerializeField, Range(0, 10f)] private float _rotSpeed;

    [SerializeField] private LocalizationVariant PutObjectLocalizationVariant;
    [SerializeField] private LocalizationVariant ProductLocalizationVariant;

    private ITradeService _tradeService;
    private ObjectToSell _waitingObjectToSell;
    private ObjectToSell _objectToSell;
    private Coroutine _checkObjCoroutine;

    private bool _startTransaction = false;

    public ObjectToSell SettedObjectToSell => _objectToSell;

    public event Action<SellZone, ObjectToSell> SetObjectToSellRequested;

    [Inject]
    private void Constructor(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    private void Awake()
    {
        if (_networkFactory == null)
        {
            _networkFactory = FindObjectOfType<NetworkFactory>();
        }
    }

    private void Start()
    {
        _infoText.text = PutObjectLocalizationVariant.Localize();
    }

    private void FixedUpdate()
    {
        if (_objectToSell != null)
        {
            _objectToSell.transform.Rotate(Vector3.up, _rotSpeed, Space.World);
        }
    }

    public override void OnNetworkSpawn()
    {
        _occupiedID.OnValueChanged += OnChangeOccupiedID;
        OnChangeOccupiedID(0, _occupiedID.Value);

        if (IsServer)
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
        else
        {
            if (_networkFactory != null)
            {
                _networkFactory.ItemDespawned += OnNetworkFactoryItemDespawned;
            }

            _trigger.ObjectToSellUpdated += OnObjectToSellUpdatedInTrigger;
            _coinTrigger.TriggerEnterInvoke += OnTriggerEnterInvoke;
            _coinTrigger.TriggerExitInvoke += OnTriggerExitInvoke;
            _coinTrigger.TriggerStayInvoke += OnTriggerStayInvoke;
        }
    }

    public override void OnNetworkDespawn()
    {
        _occupiedID.OnValueChanged -= OnChangeOccupiedID;

        if (IsServer)
        {
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
        else
        {
            if (_networkFactory != null)
            {
                _networkFactory.ItemDespawned -= OnNetworkFactoryItemDespawned;
            }

            _trigger.ObjectToSellUpdated -= OnObjectToSellUpdatedInTrigger;
            _coinTrigger.TriggerEnterInvoke -= OnTriggerEnterInvoke;
            _coinTrigger.TriggerExitInvoke -= OnTriggerExitInvoke;
            _coinTrigger.TriggerStayInvoke -= OnTriggerStayInvoke;
        }
    }

    public void SetObjectToSell(ObjectToSell objectToSell)
    {
        UpdateObjectToSell(objectToSell);
    }

    private void OnNetworkFactoryItemDespawned(TowerObjectDto towerObject)
    {
        if (_objectToSell == null || _objectToSell.ObjectToSellInfo == null
                                  || _objectToSell.ObjectToSellInfo.TowerObject == null)
        {
            return;
        }

        if (towerObject.Id == _objectToSell.ObjectToSellInfo.TowerObject.Id)
        {
            DeinitZone();
        }
    }

    private void OnTriggerEnterInvoke(CoinInfrastructure coinInfrastructure)
    {
        if (coinInfrastructure.OwnerClientId != NetworkManager.LocalClientId)
            return;
        if (_objectToSell == null)
            return;

        coinInfrastructure.SetCoinValueToReceive((int) _objectToSell.Price);
    }

    private void OnTriggerStayInvoke(CoinInfrastructure coinInfrastructure)
    {
        if (coinInfrastructure.OwnerClientId != NetworkManager.LocalClientId)
            return;

        if (_objectToSell == null)
        {
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
            coinInfrastructure.DeleteCoin();
        }
        else
        {
            coinInfrastructure.SetCoinValueToReceive((int) _objectToSell.Price);
        }
    }

    private async void OnTriggerExitInvoke(CoinInfrastructure coinInfrastructure)
    {
        if (_objectToSell == null || _objectToSell.ObjectToSellInfo == null 
            || _objectToSell.ObjectToSellInfo.TowerObject == null)
            return;
        
        if (_startTransaction)
            return;

        if (coinInfrastructure.IsRemoving())
            return;

        if (coinInfrastructure.OwnerClientId != NetworkManager.LocalClientId)
            return;

        _startTransaction = true;

        bool result = await _tradeService.SellItem(_objectToSell.ObjectToSellInfo.TowerObject.Id);
        if (result)
        {
            _networkFactory.TryDespawnItem(_objectToSell.ObjectToSellInfo.TowerObject);
            coinInfrastructure.TransactionInfrastructure.CurrentUserSuccesOperationActionInvoke();
            coinInfrastructure.DeleteCoin();
            UpdateObjectToSell(null);
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
        }
        else
        {
            coinInfrastructure.DeleteCoin();
            CreateCoinInZone();
        }

        _startTransaction = false;
    }

    private void OnObjectToSellUpdatedInTrigger(ObjectToSell objectToSell)
    {
        SetObjectToSellRequested?.Invoke(this, objectToSell);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeWorkStateServerRpc(ulong requestedClientId, ulong newOccupiedId)
    {
        SetOccupiedIDServer(requestedClientId, newOccupiedId);
    }

    private void SetOccupiedIDServer(ulong requestedClientId, ulong newOccupiedId)
    {
        if (_occupiedID.Value == 0 || _occupiedID.Value == requestedClientId)
        {
            _occupiedID.Value = newOccupiedId;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId == _occupiedID.Value)
        {
            SetOccupiedIDServer(clientId, 0);
        }
    }

    private void OnChangeOccupiedID(ulong previousValue, ulong newValue)
    {
        _trigger.ToogleWork(newValue != 0,newValue);

        if (newValue != 0 && newValue == NetworkManager.LocalClientId)
            _checkObjCoroutine = StartCoroutine(CheckObjInZone());
        else if (newValue == 0 && _checkObjCoroutine != null)
            StopCoroutine(_checkObjCoroutine);

        if (newValue != NetworkManager.LocalClientId)
        {
            _waitingObjectToSell = null;
            _objectToSell = null;
            _infoText.text = PutObjectLocalizationVariant.Localize();
            return;
        }

        if (_waitingObjectToSell != null && _objectToSell == null)
        {
            CreateCoinInZone();
        }

        _objectToSell = _waitingObjectToSell;
        _waitingObjectToSell = null;

        if (_objectToSell != null)
        {
            _objectToSell.UsePhysicsProperties(false);
            _objectToSell.OnGrabEnter += OnObjectToSellGrabEnter;
        }

        if (_objectToSell != null)
        {
            _objectToSell.transform.position = transform.position;
            _objectToSell.transform.rotation = Quaternion.identity;

            _infoText.text = $"{ProductLocalizationVariant.Localize()} " +
                $"{decimal.ToInt32(_objectToSell.Price)} TWR";
        }
        else _infoText.text = PutObjectLocalizationVariant.Localize();
    }

    private void UpdateObjectToSell(ObjectToSell obj)
    {
        if (_objectToSell != null)
        {
            _objectToSell.UsePhysicsProperties(true);
            _objectToSell.OnGrabEnter -= OnObjectToSellGrabEnter;
        }

        if (obj != null)
        {
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, NetworkManager.LocalClientId);
        }
        else
        {
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
        }
        _waitingObjectToSell = obj;
    }

    private void OnObjectToSellGrabEnter(ObjectToSell obj)
    {
        UpdateObjectToSell(null);
    }

    private void DeinitZone()
    {
        UpdateObjectToSell(null);
        ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
    }

    private IEnumerator CheckObjInZone()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_occupiedID.Value == NetworkManager.LocalClientId)
            {
                if (_objectToSell == null)
                {
                    _infoText.text = PutObjectLocalizationVariant.Localize();
                    ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
                }
            }
        }
    }

    private void CreateCoinInZone()
    {
        _networkFactory.ItemSpawned += OnNetworkItemSpawned;
        _networkFactory.CreateCurrentUserCoin(null, _coinSpawnPoint.position, _coinSpawnPoint.rotation, false);
    }

    private void OnNetworkItemSpawned(GameObject go, TowerObjectDto towerObject)
    {
        if (towerObject.TowerObjectClassName != NetworkFactory.TwrCoinObjectClassName)
        {
            return;
        }
        _networkFactory.ItemSpawned -= OnNetworkItemSpawned;

        Rigidbody[] rigidBodies = go.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.useGravity = false;
            rb.angularDrag = 300;
        }
        XRGrabInteractable grabIneractable = go.GetComponentInChildren<XRGrabInteractable>();
        if (grabIneractable != null)
        {
            grabIneractable.throwOnDetach = false;
        }
    }
}