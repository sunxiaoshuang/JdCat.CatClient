using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    [Flags]
    public enum TangOrderStatus
    {
        /// <summary>
        /// 正在点单
        /// </summary>
        [Description("正在点单")]
        Ordering = 1,
        /// <summary>
        /// 用餐中
        /// </summary>
        [Description("用餐中")]
        Eating = 2,
        /// <summary>
        /// 已结算
        /// </summary>
        [Description("已结算")]
        Settled = 4,
        /// <summary>
        /// 反结算
        /// </summary>
        [Description("反结算")]
        UnSettled = 8,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 16,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 32,


        /// <summary>
        /// 已完成的订单
        /// </summary>
        Finish = 4 + 8 + 32,
        /// <summary>
        /// 有效的订单
        /// </summary>
        Valid = 2 + 4 + 8,
        /// <summary>
        /// 无效的订单
        /// </summary>
        Invalid = 1 + 16 + 32,
        /// <summary>
        /// 未完成的订单
        /// </summary>
        Unfinish = 1 + 2
    }
}
