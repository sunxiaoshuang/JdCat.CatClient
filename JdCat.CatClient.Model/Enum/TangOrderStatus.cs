using System;
using System.Collections.Generic;
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
        Ordering = 1,
        /// <summary>
        /// 用餐中
        /// </summary>
        Eating = 2,
        /// <summary>
        /// 已结算
        /// </summary>
        Settled = 4,
        /// <summary>
        /// 反结算
        /// </summary>
        UnSettled = 8,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 16,
        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 32,


        /// <summary>
        /// 已完成的订单
        /// </summary>
        Finish = 4 + 8,
        /// <summary>
        /// 有效的订单
        /// </summary>
        Valid = 2 + 4 + 8,
        /// <summary>
        /// 无效的订单
        /// </summary>
        Invalid = 1 + 16 + 32
    }
}
