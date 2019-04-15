using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 产品特色
    /// </summary>
    [Flags]
    public enum ProductFeature : int
    {
        /// <summary>
        /// 默认值
        /// </summary>
        [Description("默认值")]
        None = 0,
        /// <summary>
        /// 招牌
        /// </summary>
        [Description("招牌")]
        Signature = 1,
        /// <summary>
        /// 套餐
        /// </summary>
        [Description("套餐")]
        SetMeal = 2
    }
}
