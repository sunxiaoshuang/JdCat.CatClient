using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    [Serializable]
    public class TangOrderProduct: BaseEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
        /// <summary>
        /// 商品特性
        /// </summary>
        public ProductFeature Feature { get; set; }
        /// <summary>
        /// 所属商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string OrderObjectId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        public TangOrder TangOrder { get; set; }
        /// <summary>
        /// 商品规格id
        /// </summary>
        public int FormatId { get; set; }
    }
}
