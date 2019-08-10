using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 第三方订单商品
    /// </summary>
    public class ThirdOrderProduct : BaseEntity
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public string Code{ get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// sku编码
        /// </summary>
        public string SkuId { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 包装盒数量
        /// </summary>
        public double BoxNum { get; set; }
        /// <summary>
        /// 包装盒单价
        /// </summary>
        public double BoxPrice { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 折扣（不打折为1.0）
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public string Spec { get; set; }
        /// <summary>
        /// 口袋
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// 所属订单id
        /// </summary>
        public int ThirdOrderId { get; set; }
        /// <summary>
        /// 所属订单实体
        /// </summary>
        public virtual ThirdOrder ThirdOrder { get; set; }
        /// <summary>
        /// 关联的商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 备用属性
        /// </summary>
        public List<Product> Tag1 { get; set; }

        /// <summary>
        /// 获取商品描述
        /// </summary>
        /// <returns></returns>
        public string GetDesc()
        {
            var str = string.Empty;
            if(!string.IsNullOrEmpty(Spec))
            {
                str += Spec + ",";
            }
            if (!string.IsNullOrEmpty(Description))
            {
                str += Description;
            }
            return str.Trim(',');
        }
    }
}
