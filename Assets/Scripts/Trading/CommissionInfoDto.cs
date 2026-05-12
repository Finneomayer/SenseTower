using System;

namespace Assets.Scripts.Trading
{
    public class CommissionInfoDto
    {
        public Guid ObjectClassId { get; set; }
        public decimal Price { get; set; }

        public bool IsEqual(CommissionInfoDto commissionInfo)
        {
            return commissionInfo != null
                && commissionInfo.ObjectClassId == ObjectClassId
                && commissionInfo.Price == Price;
        }
    }
}