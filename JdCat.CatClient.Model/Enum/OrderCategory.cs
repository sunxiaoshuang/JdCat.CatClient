using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 订单类别
    /// </summary>
    public enum OrderMode
    {
        /// <summary>
        /// 未定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 快餐
        /// </summary>
        FastFood = 1, 
        /// <summary>
        /// 中餐
        /// </summary>
        ChineseFood = 2
    }
}
