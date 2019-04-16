using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 支付类别
    /// </summary>
    public enum PaymentCategory
    {
        /// <summary>
        /// 现金
        /// </summary>
        Money = 1,
        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay = 2,
        /// <summary>
        /// 微信
        /// </summary>
        Wexin = 4,
        /// <summary>
        /// 其他类型
        /// </summary>
        Other = 8
    }
}
