using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 堂食订单
    /// </summary>
    [Table("StoreOrder")]
    public class StoreOrder : ClientBaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 当日流水码
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleQuantity { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double OldAmount { get; set; }
        /// <summary>
        /// 实付款
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public ICollection<StoreOrderProduct> OrderProducts { get; set; }
        /// <summary>
        /// 餐桌id
        /// </summary>
        public int DeskId { get; set; }
        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string DeskName { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public StoreOrderStatus Status { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int BusinessId { get; set; }

    }
}
