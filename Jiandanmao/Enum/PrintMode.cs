using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Enum
{
    public enum PrintMode
    {
        /// <summary>
        /// 预结单
        /// </summary>
        PreOrder = 0,
        /// <summary>
        /// 结算单
        /// </summary>
        Payment = 1,
        /// <summary>
        /// 加菜单
        /// </summary>
        Add = 2,
        /// <summary>
        /// 退菜单
        /// </summary>
        Return = 3
    }
}
