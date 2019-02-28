using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    [Flags]
    public enum OrderStatus : int
    {
        /// <summary>
        /// 已付款（前：商家待接单，后：已付款），有效订单
        /// </summary>
        [Description("已付款")]
        Payed = 1,
        /// <summary>
        /// 拒绝接单（前：已拒单，后：已拒单），无效订单
        /// </summary>
        [Description("已拒单")]
        Cancel = 2,
        /// <summary>
        /// 商家已接单（前：商家已接单，后：待配送），有效订单
        /// </summary>
        [Description("待配送")]
        Receipted = 4,
        /// <summary>
        /// 待取货（前：骑手待取货，后：待配送），有效订单
        /// </summary>
        [Description("待配送")]
        DistributorReceipt = 8,
        /// <summary>
        /// 配送中（前：骑手配送中，后：配送中），有效订单
        /// </summary>
        [Description("配送中")]
        Distribution = 16,
        /// <summary>
        /// 配送异常，无效订单
        /// </summary>
        [Description("配送异常")]
        Exception = 32,
        /// <summary>
        /// 已送达（前：待确认收货，后：已送达），有效订单
        /// </summary>
        [Description("已送达")]
        Achieve = 64,
        /// <summary>
        /// 已完成（前：订单已完成，后：用户已确认收货），有效订单
        /// </summary>
        [Description("用户已确认收货")]
        Finish = 128,
        /// <summary>
        /// 未付款，无效订单
        /// </summary>
        [Description("未付款")]
        NotPay = 256,
        /// <summary>
        /// 已评价（前：订单已完成，后：已评价），有效订单
        /// </summary>
        [Description("已评价")]
        Appraised = 512,
        /// <summary>
        /// 已关闭，无效订单
        /// </summary>
        [Description("已关闭")]
        Close = 1024,
        /// <summary>
        /// 订单配送已取消
        /// </summary>
        [Description("订单配送已取消")]
        CallOff = 2048,
        /// <summary>
        /// 支付超时
        /// </summary>
        [Description("支付超时")]
        Overtime = 4096,
        /// <summary>
        /// 无效订单
        /// </summary>
        [Description("无效订单")]
        Invalid = 2 + 32 + 256 + 1024 + 4096
    }
}
