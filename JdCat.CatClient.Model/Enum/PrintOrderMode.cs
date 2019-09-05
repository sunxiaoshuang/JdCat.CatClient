using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 打印方式
    /// </summary>
    [Flags]
    public enum PrintOrderMode
    {
        /// <summary>
        /// 前台打印
        /// </summary>
        Reception = 1,
        /// <summary>
        /// 后台打印
        /// </summary>
        Backstage = 2,
        /// <summary>
        /// 前后台均打印
        /// </summary>
        All = 3
    }
}
