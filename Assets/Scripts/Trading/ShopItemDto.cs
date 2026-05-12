using Assets.Scripts.TowerObjects;

namespace Assets.Scripts.Trading
{
    public class ShopItemDto
    {
        public int PlaceNumber { get; set; }
        public TowerObjectDto? Item { get; set; }
        public decimal? Price { get; set; }

        public bool IsEqual(ShopItemDto shopItemDto)
        {
            if (shopItemDto == null)
            {
                return false;
            }

            return PlaceNumber == shopItemDto.PlaceNumber
                && Price == shopItemDto.Price
                && (Item == null && Item == shopItemDto.Item
                    || Item != null && shopItemDto.Item != null && Item.IsEqual(shopItemDto.Item));
        }
    }
}