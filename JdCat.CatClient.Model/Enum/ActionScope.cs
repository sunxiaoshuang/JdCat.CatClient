using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 订单范围
    /// </summary>
    [Flags]
    public enum ActionScope
    {
        /// <summary>
        /// 外卖
        /// </summary>
        [Description("外卖")]
        Takeout = 1,
        /// <summary>
        /// 堂食
        /// </summary>
        [Description("堂食")]
        Store = 2
    }
}
