using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 送货方式
    /// </summary>
    public enum DadaStatus
    {
        /// <summary>
        /// 待接单
        /// </summary>
        PendingOrder = 1,
        /// <summary>
        /// 待取货
        /// </summary>
        DistributorReceipt = 2,
        /// <summary>
        /// 配送中
        /// </summary>
        Distribution = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 4,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 5,
        /// <summary>
        /// 已过期
        /// </summary>
        Expire = 7,
        /// <summary>
        /// 指派单
        /// </summary>
        AssignmentList = 8,
        /// <summary>
        /// 妥投异常之物品返回中
        /// </summary>
        Returning = 9,
        /// <summary>
        /// 妥投异常之物品返回完成
        /// </summary>
        Returned = 10,
        /// <summary>
        /// 创建达达运单失败
        /// </summary>
        Fail = 1000
    }
}
