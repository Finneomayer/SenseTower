using System;
using Assets.Mechanics.Shop.Scripts;
using Assets.Scripts.TowerObjects;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectToBuy : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private decimal _price;
    public ShopItemPlace _shopItemPlace { private set; get; }

    public event Action<ObjectToBuy> OnGrabExit;
    public event Action<ObjectToBuy> OnGrabEnter;

    public string Name => _name;
    public decimal Price => _price;
    
    private XRGrabInteractable _grabInteractable;

    public void Init(ShopItemPlace shopItemPlace, GameObject parentGrabGameObject)
    {
        if (shopItemPlace == null || shopItemPlace.ShopItemDto == null)
        {
            Debug.LogError("shopItemPlace is not initialized");
            return;
        }

        _grabInteractable = parentGrabGameObject.GetComponentInChildren<XRGrabInteractable>();
        if (_grabInteractable != null)
        {
            _grabInteractable.selectExited.AddListener(SelectExit);
            _grabInteractable.selectEntered.AddListener(SelectEnter);
        }

        _name = shopItemPlace.ShopItemDto.Item != null ? shopItemPlace.ShopItemDto.Item.GetLocalizedName() : "no item";
        _price = shopItemPlace.ShopItemDto.Price ?? 0;
        _shopItemPlace = shopItemPlace;
    }

    private void SelectExit(SelectExitEventArgs arg0)
    {
        OnGrabExit?.Invoke(this);
    }

    private void SelectEnter(SelectEnterEventArgs arg0)
    {
        OnGrabEnter?.Invoke(this);
    }
}