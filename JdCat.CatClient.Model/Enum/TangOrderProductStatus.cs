using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    [Flags]
    public enum TangOrderProductStatus
    {
        /// <summary>
        /// 正在下单的商品
        /// </summary>
        Order = 1,
        /// <summary>
        /// 已下单商品
        /// </summary>
        Ordered = 2,
        /// <summary>
        /// 正在加菜的商品
        /// </summary>
        Add = 4,
        /// <summary>
        /// 已下单的加菜商品
        /// </summary>
        Added = 8,
        /// <summary>
        /// 退菜商品
        /// </summary>
        Return = 16,

        /// <summary>
        /// 可累加
        /// </summary>
        Cumulative = 1 + 4,
        /// <summary>
        /// 可退菜
        /// </summary>
        CanReturn = 2 + 8
    }
}
