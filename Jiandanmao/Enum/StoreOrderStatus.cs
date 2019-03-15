using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 堂食订单状态
    /// </summary>
    [Flags]
    public enum StoreOrderStatus : int
    {
        /// <summary>
        /// 未定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 正在点餐
        /// </summary>
        Ordering = 1,
        /// <summary>
        /// 正在用餐
        /// </summary>
        Eating = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 4,
        /// <summary>
        /// 使用中
        /// </summary>
        Using = 1 + 2
    }
}
