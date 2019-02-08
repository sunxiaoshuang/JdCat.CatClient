using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum OrderCategory
    {
        /// <summary>
        /// 外卖
        /// </summary>
        [Description("外卖")]
        TakeOut = 0,
        /// <summary>
        /// 堂食
        /// </summary>
        [Description("堂食")]
        Here = 1
    }
}
