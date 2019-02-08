using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 优惠券状态
    /// </summary>
    public enum CouponStatus
    {
        [Description("未使用")]
        NotUse = 1,
        [Description("已使用")]
        Used = 2,
        [Description("已过期")]
        Expire = 3,
        [Description("上架")]
        Up = 4,
        [Description("下架")]
        Down = 5,
        [Description("已领完")]
        Over = 6,
        [Description("未开始")]
        NotBegin = 7
    }
}
