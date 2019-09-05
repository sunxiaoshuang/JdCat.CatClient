using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 订单活动
    /// </summary>
    public class OrderActivity : BaseEntity
    {
        /// <summary>
        /// 活动金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 活动类别
        /// </summary>
        public OrderActivityType Type { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// 活动备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 所属订单id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 所属订单实体
        /// </summary>
        public virtual Order Order { get; set; }
    }
}
