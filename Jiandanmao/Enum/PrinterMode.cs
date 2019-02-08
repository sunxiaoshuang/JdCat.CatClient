using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 活动状态
    /// </summary>
    public enum PrinterMode
    {
        /// <summary>
        /// 一菜一打
        /// </summary>
        [Description("一菜一打")]
        Food = 0,
        /// <summary>
        /// 一份一打
        /// </summary>
        [Description("一份一打")]
        Share = 1,
        /// <summary>
        /// 一单一打
        /// </summary>
        [Description("一单一打")]
        Order = 2
    }
}
