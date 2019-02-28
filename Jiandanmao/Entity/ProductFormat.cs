using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 商品规格表
    /// </summary>
    public class ProductFormat : BaseEntity
    {
        /// <summary>
        /// 规格名称，产品只有一个规格时，不是必填
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 规格编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 库存，默认：-1，无限库存
        /// </summary>
        public decimal? Stock { get; set; } = -1;
        /// <summary>
        /// 包装盒价格
        /// </summary>
        public decimal? PackingPrice { get; set; }
        /// <summary>
        /// 包装盒数量
        /// </summary>
        public decimal? PackingQuantity { get; set; }
        /// <summary>
        /// UPC码
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// SKU码
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 位置码
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 分类所属商家id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品对象
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// 订单商品集合
        /// </summary>
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
