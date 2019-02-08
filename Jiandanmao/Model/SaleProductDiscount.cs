
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Model
{
    /// <summary>
    /// 产品折扣表
    /// </summary>
    public class SaleProductDiscount : BaseEntity
    {
        /// <summary>
        /// 打折商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        public ActivityStatus Status { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OldPrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 活动价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 循环周期
        /// </summary>
        public WeekdayState Cycle { get; set; }
        /// <summary>
        /// 时间段1
        /// </summary>
        public string StartTime1 { get; set; }
        /// <summary>
        /// 时间段1
        /// </summary>
        public string EndTime1 { get; set; }
        /// <summary>
        /// 时间段2
        /// </summary>
        public string StartTime2 { get; set; }
        /// <summary>
        /// 时间段2
        /// </summary>
        public string EndTime2 { get; set; }
        /// <summary>
        /// 时间段3
        /// </summary>
        public string StartTime3 { get; set; }
        /// <summary>
        /// 时间段3
        /// </summary>
        public string EndTime3 { get; set; }
        /// <summary>
        /// 设置方式，1:按折扣，2:按活动价
        /// </summary>
        public string SettingType { get; set; }
        /// <summary>
        /// 每单上限，-1表示无上限
        /// </summary>
        public int UpperLimit { get; set; }
        /// <summary>
        /// 关联商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品对象
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 商户对象
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
