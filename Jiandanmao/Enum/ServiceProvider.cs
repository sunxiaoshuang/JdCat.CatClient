using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 配送服务商
    /// </summary>
    public enum ServiceProvider
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("未设置")]
        None = 0,
        /// <summary>
        /// 自己配送
        /// </summary>
        [Description("自己配送")]
        Self = 1,
        /// <summary>
        /// 达达配送
        /// </summary>
        [Description("达达配送")]
        Dada = 2
    }
}
