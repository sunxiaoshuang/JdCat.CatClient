using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 产品状态
    /// </summary>
    public enum ProductStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Description("初始化")]
        Init = 0,
        /// <summary>
        /// 已上架
        /// </summary>
        [Description("已上架")]
        Sale = 1,
        /// <summary>
        /// 已下架
        /// </summary>
        [Description("已下架")]
        NotSale = 2,
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Delete = 3
    }
}
