using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 一周状态
    /// </summary>
    public enum WeekdayState
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// 星期一
        /// </summary>
        [Description("星期一")]
        Monday = 1,
        /// <summary>
        /// 星期二
        /// </summary>
        [Description("星期二")]
        Tuesday = 2,
        /// <summary>
        /// 星期三
        /// </summary>
        [Description("星期三")]
        Wednesday = 4,
        /// <summary>
        /// 星期四
        /// </summary>
        [Description("星期四")]
        Thursday = 8,
        /// <summary>
        /// 星期五
        /// </summary>
        [Description("星期五")]
        Friday = 16,
        /// <summary>
        /// 星期六
        /// </summary>
        [Description("星期六")]
        Saturday = 32,
        /// <summary>
        /// 星期日
        /// </summary>
        [Description("星期日")]
        Sunday = 64,
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 127
    }
}
