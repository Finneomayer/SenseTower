using Assets.Mechanics.Shop.Scripts;
using Assets.Scripts.Trading;
using System;
using Assets.Scripts.TowerObjects;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectToSellInfo
{
    public TowerObjectDto TowerObject;
    public decimal Price;
}

public class ObjectToSell : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private decimal _price;

    public event Action<ObjectToSell> OnGrabExit;
    public event Action<ObjectToSell> OnGrabEnter;

    public string Name => _name;
    public decimal Price => _price;
    public ObjectToSellInfo ObjectToSellInfo => _objectToSellInfo;

    private XRGrabInteractable _grabInteractable;
    private ObjectToSellInfo _objectToSellInfo;
    private bool _physicsIsWorking = true;

    private bool _initialKinematicState;
    private bool _initialThrowOnDetachState;
    private int _initialLayer;
    private Rigidbody _rb;

    private void Start()
    {
        _initialLayer = gameObject.layer;
        if (TryGetComponent(out _grabInteractable))
        {
            _grabInteractable.selectExited.AddListener(SelectExit);
            _grabInteractable.selectEntered.AddListener(SelectEnter);
            _initialThrowOnDetachState = _grabInteractable.throwOnDetach;
        }
    }

    public void Init(TowerObjectDto towerObject, CommissionInfoDto itemShopCommission)
    {
        _name = towerObject.GetLocalizedName();
        _price = itemShopCommission.Price;
        _objectToSellInfo = new()
        {
            Price = itemShopCommission.Price,
            TowerObject = towerObject,
        };
    }

    public void UsePhysicsProperties(bool isUse)
    {
        _physicsIsWorking = isUse;
        RefreshPhysicsState();
    }

    private void SelectExit(SelectExitEventArgs arg0)
    {
        RefreshPhysicsState();
        OnGrabExit?.Invoke(this);
    }

    private void SelectEnter(SelectEnterEventArgs arg0)
    {
        OnGrabEnter?.Invoke(this);
    }

    private void RefreshPhysicsState()
    {
        if (_rb == null)
        {
            if (TryGetComponent(out _rb))
            {
                _initialKinematicState = _rb.isKinematic;
            }
            else
            {
                return;
            }
        }

        if (_physicsIsWorking)
        {
            _rb.isKinematic = _initialKinematicState;
            gameObject.layer = _initialLayer;
        }
        else
        {
            _rb.isKinematic = true;
            gameObject.layer = 20;
        }

        if (_grabInteractable != null)
        {
            _grabInteractable.throwOnDetach = _initialThrowOnDetachState && !_rb.isKinematic;
        }
    }
}