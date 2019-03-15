using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 支付方式状态
    /// </summary>
    public enum PaymentTypeStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 隐藏
        /// </summary>
        Hidden = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2
    }
}
