using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    [Serializable]
    public class TangOrder : BaseEntity
    {
        /// <summary>
        /// 订单当日编号（为0表示还没有下单）
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleNumber { get; set; }
        /// <summary>
        /// 消费
        /// </summary>
        public double Tips { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public TangOrderStatus Status { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public OrderSource OrderSource { get; set; }
        /// <summary>
        /// 支付方式id（远程）
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// 支付方式id（本地）
        /// </summary>
        public string PaymentTypeObjectId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentType PaymentType { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 所属员工id（远程）
        /// </summary>
        public int StaffId { get; set; }
        /// <summary>
        /// 所属员工id（本地）
        /// </summary>
        public string StaffObjectId { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        public Staff Staff { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public List<TangOrderProduct> TangOrderProducts { get; set; }
    }
}
