using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 达达送货方式
    /// </summary>
    public enum DadaCancelSource
    {
        /// <summary>
        /// 默认值
        /// </summary>
        Default = 0,
        /// <summary>
        /// 达达配送员取消
        /// </summary>
        Distributor = 1,
        /// <summary>
        /// 商家主动取消
        /// </summary>
        Business = 2,
        /// <summary>
        /// 系统或客服取消
        /// </summary>
        System = 3,
    }
}
