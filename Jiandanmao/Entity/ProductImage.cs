
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 商品图片表，目前只支持一个商品一张图片
    /// </summary>
    public class ProductImage : BaseEntity
    {
        /// <summary>
        /// 文件长度
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 图片扩展名
        /// </summary>
        public string ExtensionName { get; set; } = "jpg";
        /// <summary>
        /// 图片名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public ImageType Type { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// 图片地址，根据配置，将image组件的url格式化为正确的网络地址
        /// </summary>
        public string Source { get; set; }
    }
}
