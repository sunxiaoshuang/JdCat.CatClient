using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 订单类别
    /// </summary>
    public enum OrderCategory
    {
        /// <summary>
        /// 外卖
        /// </summary>
        [Description("外卖")]
        TakeOut = 0,
        /// <summary>
        /// 快餐
        /// </summary>
        [Description("快餐")]
        FastFood = 1,
        /// <summary>
        /// 中餐
        /// </summary>
        [Description("中餐")]
        ChineseFood = 2
    }
}
