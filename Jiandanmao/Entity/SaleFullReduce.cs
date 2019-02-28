
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 满减活动
    /// </summary>
    public class SaleFullReduce : BaseEntity
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活动开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 活动结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 是否永久有效
        /// </summary>
        public bool IsForeverValid { get; set; }
        /// <summary>
        /// 最低消费金额
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// 减少金额
        /// </summary>
        public decimal ReduceMoney { get; set; }
        /// <summary>
        /// 是否被删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 商户Id
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        /// <summary>
        /// 活动修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 参与满减的订单
        /// </summary>
        public ICollection<Order> Orders { get; set; }

        /// <summary>
        /// 当前的活动是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsActiveValid()
        {
            if (this.IsDelete) return false;
            if (this.IsForeverValid) return true;
            var now = DateTime.Now;
            if (this.StartDate > now || this.EndDate < now) return false;
            return true;
        }
    }
}
