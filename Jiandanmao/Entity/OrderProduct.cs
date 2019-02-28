using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 订单商品表
    /// </summary>
    public class OrderProduct : BaseEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 商品价格（折扣价）
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal? OldPrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 描述（规格 + 属性）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// 商品规格id
        /// </summary>
        public int? FormatId { get; set; }
        public virtual ProductFormat Format { get; set; }
        /// <summary>
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
        /// <summary>
        /// 商品特性
        /// </summary>
        public ProductFeature Feature { get; set; }
        /// <summary>
        /// 备用属性，记录套餐商品
        /// </summary>
        public List<Product> Tag1 { get; set; }

    }
}
