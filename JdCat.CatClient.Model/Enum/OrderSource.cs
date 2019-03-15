using System;
using System.Collections.Generic;
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
        Cashier = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        Scan = 2
    }
}
