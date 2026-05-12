using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Mechanics.Network.Scripts;
using Assets.Mechanics.Triggers;
using Assets.Scripts.Trading;
using Mechanics.UserWallet;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class BuyZone : NetworkBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private BuyZoneTrigger _trigger;
    [SerializeField] private CoinTriggerObserver _coinTrigger;

    [SerializeField] private NetworkVariable<ulong> _occupiedID = new(0);
    [SerializeField, Range(0, 10f)] private float _rotSpeed;

    [SerializeField] private LocalizationVariant PutObjectLocalizationVariant;
    [SerializeField] private LocalizationVariant ProductLocalizationVariant;
    [SerializeField] private LocalizationVariant PriceLocalizationVariant;

    private ObjectToBuy _waitingObject;
    private ObjectToBuy _objectToBuy;
    private ITradeService _tradeService;
    private Coroutine _checkObjCoroutine;
    private bool _startTransaction = false;

    public ObjectToBuy SettedObject => _objectToBuy;

    public Action<BuyZone, ObjectToBuy> SetObjectRequested; //is listening in ShopTradingInfrastructure.cs
    //when put object to the zone

    [Inject]
    private void Constructor(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    private void Start()
    {
        _infoText.text = PutObjectLocalizationVariant.Localize();
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
            _trigger.ObjectToSellUpdated += OnObjectToBuyUpdatedInTrigger;
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
            _trigger.ObjectToSellUpdated -= OnObjectToBuyUpdatedInTrigger;
            _coinTrigger.TriggerEnterInvoke -= OnTriggerEnterInvoke;
            _coinTrigger.TriggerExitInvoke -= OnTriggerExitInvoke;
            _coinTrigger.TriggerStayInvoke -= OnTriggerStayInvoke;
        }
    }

    private void FixedUpdate()
    {
        if (_objectToBuy != null)
        {
            _objectToBuy.transform.Rotate(Vector3.up, _rotSpeed, Space.World);
        }
    }

    private void OnDisable()
    {
        _coinTrigger.TriggerEnterInvoke -= OnTriggerEnterInvoke;
        _coinTrigger.TriggerExitInvoke -= OnTriggerExitInvoke;
        _coinTrigger.TriggerStayInvoke -= OnTriggerStayInvoke;
    }

    public void SetObject(ObjectToBuy objectToBuy)
    {
        UpdateObjectToBuy(objectToBuy);
    }

    private void OnTriggerEnterInvoke(CoinInfrastructure coinInfrastructure)
    {
        if (_objectToBuy == null)
            return;

        coinInfrastructure.TrySetCoinValue((int) _objectToBuy.Price, _objectToBuy.Name);
    }

    private async void OnTriggerStayInvoke(CoinInfrastructure coinInfrastructure)
    {
        if (_objectToBuy == null)
            return;

        if (coinInfrastructure.OwnerClientId != NetworkManager.Singleton.LocalClientId)
            return;

        coinInfrastructure.TrySetCoinValue((int) _objectToBuy.Price, _objectToBuy.Name);

        if (coinInfrastructure.GetWalletValue() < _objectToBuy.Price)
            return;

        if (!coinInfrastructure.IsGrabbing() && coinInfrastructure.IsFirstContactExist())
        {
            if (_objectToBuy._shopItemPlace == null)
                return;

            if (_startTransaction || coinInfrastructure.IsRemoving())
                return;

            _startTransaction = true;
            bool result = await _tradeService.BuyItem(_objectToBuy._shopItemPlace.ShopItemDto);

            if (result)
            {
                coinInfrastructure.TransactionInfrastructure.CurrentUserSuccesOperationActionInvoke();

                coinInfrastructure.DeleteCoin();

                if (_objectToBuy != null && _objectToBuy._shopItemPlace != null)
                    _objectToBuy._shopItemPlace.DeInit();

                UpdateObjectToBuy(null);
                _startTransaction = false;
                ChangeWorkStateServerRpc(NetworkManager.Singleton.LocalClientId, 0);
            }
            else
            {
                _startTransaction = false;
                coinInfrastructure.DeleteCoin();
            }
        }
    }

    private void OnTriggerExitInvoke(CoinInfrastructure coinInfrastructure)
    {
        coinInfrastructure.SetManualMode();
    }

    private void OnObjectToBuyUpdatedInTrigger(ObjectToBuy objectToBuy)
    {
        SetObjectRequested?.Invoke(this, objectToBuy);
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
            _waitingObject = null;
            _objectToBuy = null;
            _infoText.text = PutObjectLocalizationVariant.Localize();
            return;
        }

        _objectToBuy = _waitingObject;
        _waitingObject = null;

        if (_objectToBuy != null)
        {
            _objectToBuy.transform.position = transform.position;

            _infoText.text = $"{ProductLocalizationVariant.Localize()} " +
                $"{decimal.ToInt32(_objectToBuy.Price)} TWR\n {PriceLocalizationVariant.Localize()} {_objectToBuy.Name}";
        }
        else _infoText.text = PutObjectLocalizationVariant.Localize();
    }

    private void UpdateObjectToBuy(ObjectToBuy obj)
    {
        _waitingObject = obj;
        if (obj != null)
        {
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, NetworkManager.LocalClientId);
        }
        else
        {
            ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
        }
    }
    
    
    private IEnumerator CheckObjInZone()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_occupiedID.Value == NetworkManager.Singleton.LocalClientId)
            {
                if (_objectToBuy == null)
                {
                    _infoText.text = PutObjectLocalizationVariant.Localize();
                    ChangeWorkStateServerRpc(NetworkManager.LocalClientId, 0);
                }
            }
        }
    }
}