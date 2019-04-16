using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model.Enum
{
    /// <summary>
    /// 配送服务商
    /// </summary>
    public enum LogisticsType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未知")]
        None = 0,
        /// <summary>
        /// 自己配送
        /// </summary>
        [Description("商家自送")]
        Self = 1,
        /// <summary>
        /// 达达配送
        /// </summary>
        [Description("达达配送")]
        Dada = 2,
        /// <summary>
        /// 美团配送
        /// </summary>
        [Description("美团配送")]
        Meituan = 3,
        /// <summary>
        /// 蜂鸟配送
        /// </summary>
        [Description("蜂鸟配送")]
        Fengniao = 4,
        /// <summary>
        /// 点我达配送
        /// </summary>
        [Description("点我达配送")]
        Dianwoda = 5,
        /// <summary>
        /// 一城飞客
        /// </summary>
        [Description("一城飞客配送")]
        Yichengfeike = 6
    }
}
