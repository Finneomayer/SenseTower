using System;

namespace Assets.Scripts.Trading
{
    [Serializable]
    public class ShopDto
    {
        public Guid Id;
        public ShopItemDto[] Items;
        public CommissionInfoDto[] CommissionPrices;

        public bool IsEqual(ShopDto shop)
        {
            if (shop == null)
            {
                return false;
            }

            if (Id != shop.Id)
            {
                return false;
            }

            if (Items != shop.Items)
            {

            }

            if (Items == null && shop.Items != null
                || Items != null && shop.Items == null)
            {
                return false;
            }

            if (CommissionPrices == null && shop.CommissionPrices != null
                || CommissionPrices != null && shop.CommissionPrices == null)
            {
                return false;
            }

            if (shop.Items != null)
            {
                if (Items.Length != shop.Items.Length)
                {
                    return false;
                }

                for (int i = 0; i < shop.Items.Length; i++)
                {
                    if (Items[i] == null)
                    {
                        if (shop.Items[i] != null)
                        {
                            return false;
                        }
                    }
                    else if (!Items[i].IsEqual(shop.Items[i]))
                    {
                        return false;
                    }
                }
            }

            if (shop.CommissionPrices != null)
            {
                if (CommissionPrices.Length != shop.CommissionPrices.Length)
                {
                    return false;
                }

                for (int i = 0; i < shop.CommissionPrices.Length; i++)
                {
                    if (CommissionPrices[i] == null)
                    {
                        if (shop.CommissionPrices[i] != null)
                        {
                            return false;
                        }
                    }
                    else if (!CommissionPrices[i].IsEqual(shop.CommissionPrices[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}