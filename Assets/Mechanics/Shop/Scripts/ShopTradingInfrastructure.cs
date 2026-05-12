using UnityEngine;

namespace Assets.Mechanics.Shop.Scripts
{
    public class ShopTradingInfrastructure : MonoBehaviour
    {
        private SellZone[] _sellZones;
        private BuyZone[] _buyZones;

        private void Awake()
        {
            _sellZones = GetComponentsInChildren<SellZone>();
            _buyZones = GetComponentsInChildren<BuyZone>();
        }

        private void OnEnable()
        {
            foreach (var sellZone in _sellZones)
            {
                sellZone.SetObjectToSellRequested += OnSetObjectToSellRequested;
            }
            foreach (var buyZone in _buyZones)
            {
                buyZone.SetObjectRequested += OnSetObjectToBuyRequested;
            }
        }

        private void OnDisable()
        {
            foreach (var sellZone in _sellZones)
            {
                sellZone.SetObjectToSellRequested -= OnSetObjectToSellRequested;
            }
            foreach (var buyZone in _buyZones)
            {
                buyZone.SetObjectRequested -= OnSetObjectToBuyRequested;
            }
        }

        private void OnSetObjectToSellRequested(SellZone sellZoneToSetObject, ObjectToSell objectToSell)
        {
            if (objectToSell == null)
            {
                sellZoneToSetObject.SetObjectToSell(null);
                return;
            }

            bool anySellZoneHasObjectToSell = false;

            foreach (var sellZone in _sellZones)
            {
                if (sellZone.SettedObjectToSell != null)
                {
                    anySellZoneHasObjectToSell = true;
                    break;
                }
            }

            if (anySellZoneHasObjectToSell)
            {
                return;
            }

            sellZoneToSetObject.SetObjectToSell(objectToSell);
        }

        private void OnSetObjectToBuyRequested(BuyZone buyZoneToSetObject, ObjectToBuy objectToBuy)
        {
            if (objectToBuy == null)
            {
                buyZoneToSetObject.SetObject(null);
                return;
            }

            bool anyZoneHasObject = false;

            foreach (var buyZone in _buyZones)
            {
                if (buyZone.SettedObject != null)
                {
                    anyZoneHasObject = true;
                    break;
                }
            }

            if (anyZoneHasObject)
            {
                return;
            }

            buyZoneToSetObject.SetObject(objectToBuy);
        }
    }
}