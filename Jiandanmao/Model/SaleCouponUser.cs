
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Model
{
    /// <summary>
    /// 用户领取的优惠券
    /// </summary>
    public class SaleCouponUser : BaseEntity
    {
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 价值
        /// </summary>
        public double Value { get; set; }

        private CouponStatus _status;
        /// <summary>
        /// 优惠券状态
        /// </summary>
        public CouponStatus Status
        {
            get
            {
                if(_status == CouponStatus.NotUse)
                {
                    if(ValidDay == null || ValidDay <= 0)
                    {
                        var now = DateTime.Now;
                        if (EndDate.Value < now)
                        {
                            _status = CouponStatus.Expire;
                        }
                    }
                    else
                    {
                        if(ExpireDay < 0)
                        {
                            _status = CouponStatus.Expire;
                        }
                    }
                }
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        /// 最低消费，-1代表不需要最低消费
        /// </summary>
        public double MinConsume { get; set; } = -1;
        /// <summary>
        /// 优惠券有效期开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 优惠券有效期结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 优惠券有效天数
        /// </summary>
        public int? ValidDay { get; set; }
        /// <summary>
        /// 到期天数
        /// </summary>
        public int? ExpireDay
        {
            get
            {
                if (ValidDay == null || ValidDay <= 0) return null;
                return (int)(CreateTime.Value.Date.AddDays(ValidDay.Value + 1) - DateTime.Now.Date).TotalDays;
            }
        }
        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? UseTime { get; set; }
        /// <summary>
        /// 所属用户
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }
        /// <summary>
        /// 商户优惠券id
        /// </summary>
        public int CouponId { get; set; }
        public SaleCoupon Coupon { get; set; }
        /// <summary>
        /// 关联的订单id
        /// </summary>
        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
