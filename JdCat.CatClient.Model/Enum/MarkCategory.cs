using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    public enum MarkCategory
    {
        /// <summary>
        /// 口味
        /// </summary>
        Flavor = 1,
        /// <summary>
        /// 整单备注
        /// </summary>
        OrderMark = 2,
        /// <summary>
        /// 退菜原因
        /// </summary>
        RefundFoodReason = 3,
        /// <summary>
        /// 取消配送原因
        /// </summary>
        CancelDeliveryReason = 4,
        /// <summary>
        /// 退款原因
        /// </summary>
        RefundMoneyReason = 5,
        /// <summary>
        /// 取消订单原因
        /// </summary>
        CancelOrderReason = 6,
        /// <summary>
        /// 支付备注
        /// </summary>
        PayRemark = 7
    }
}
