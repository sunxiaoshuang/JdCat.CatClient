using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 第三方订单
    /// </summary>
    [Table("ThirdOrder")]
    public class ThirdOrder : BaseEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单展示ID
        /// </summary>
        public long OrderIdView { get; set; }
        /// <summary>
        /// 商家ID
        /// </summary>
        public string PoiCode { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string PoiName { get; set; }
        /// <summary>
        /// 商家地址
        /// </summary>
        public string PoiAddress { get; set; }
        /// <summary>
        /// 商家电话
        /// </summary>
        public string PoiPhone { get; set; }
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecipientName { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string RecipientAddress { get; set; }
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string RecipientPhone { get; set; }
        /// <summary>
        /// 配送费
        /// </summary>
        public double ShippingFee { get; set; }
        /// <summary>
        /// 餐盒费
        /// </summary>
        public double PackageFee { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Caution { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 纳税人识别码
        /// </summary>
        public string TaxpayerId { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime Ctime { get; set; }
        /// <summary>
        /// 订单更新时间
        /// </summary>
        public DateTime Utime { get; set; }
        /// <summary>
        /// 用户预计送达时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 当日流水
        /// </summary>
        public int DaySeq { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 补打次数
        /// </summary>
        public int PrintTimes { get; set; }

        /// <summary>
        /// 打印方式
        /// </summary>
        public PrintOrderMode PrintType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// 订单来源[0:美团,1:饿了么]
        /// </summary>
        public int OrderSource { get; set; }
        /// <summary>
        /// 订单所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 商户实体
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public virtual ICollection<ThirdOrderProduct> ThirdOrderProducts { get; set; }
        /// <summary>
        /// 订单优惠活动
        /// </summary>
        public virtual ICollection<ThirdOrderActivity> ThirdOrderActivities { get; set; }
    }
}
