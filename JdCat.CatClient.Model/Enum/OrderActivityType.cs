using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 订单活动类别
    /// </summary>
    public enum OrderActivityType
    {
        /// <summary>
        /// 新客立减
        /// </summary>
        [Description("新客立减")]
        NewCustom = 1,
        /// <summary>
        /// 满减
        /// </summary>
        [Description("满减")]
        FullReduce = 2,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("优惠券")]
        Coupon = 3,
        /// <summary>
        /// 商品折扣
        /// </summary>
        [Description("商品折扣")]
        ProductDiscount = 4,
        /// <summary>
        /// 订单折扣
        /// </summary>
        [Description("订单折扣")]
        OrderDiscount = 5,
        /// <summary>
        /// 整单立减
        /// </summary>
        [Description("整单立减")]
        OrderPreferential = 6
    }
}
