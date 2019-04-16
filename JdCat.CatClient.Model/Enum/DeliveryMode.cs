using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 送货方式
    /// </summary>
    public enum DeliveryMode
    {
        /// <summary>
        /// 第三方平台
        /// </summary>
        [Description("第三方平台")]
        Third = 0,
        /// <summary>
        /// 自己配送
        /// </summary>
        [Description("自己配送")]
        Own = 1,
        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        Self = 2
    }
}
