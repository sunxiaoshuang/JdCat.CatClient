using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    [Flags]
    public enum OrderSource
    {
        /// <summary>
        /// 收银台
        /// </summary>
        [Description("收银台")]
        Cashier = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        [Description("扫码点餐")]
        Scan = 2
    }
}
