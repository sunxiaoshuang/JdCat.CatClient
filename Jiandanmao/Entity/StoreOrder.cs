using Jiandanmao.Entity;
using System.Collections.Generic;

namespace Jiandanmao.Enum
{
    /// <summary>
    /// 堂食订单
    /// </summary>
    public class StoreOrder : ClientBaseEntity
    {
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
        public ICollection<OrderProduct> OrderProducts { get; set; }
        /// <summary>
        /// 餐桌id
        /// </summary>
        public int DeskId { get; set; }
        /// <summary>
        /// 餐桌
        /// </summary>
        public Desk Desk { get; set; }
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
        /// <summary>
        /// 门店
        /// </summary>
        public Business Business { get; set; }

    }
}
