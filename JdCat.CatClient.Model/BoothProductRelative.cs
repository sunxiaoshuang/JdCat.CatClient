using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 档口商品关系
    /// </summary>
    [Serializable]
    public class BoothProductRelative : ClientBaseEntity
    {
        /// <summary>
        /// 档口id
        /// </summary>
        public int StoreBoothId { get; set; }
        /// <summary>
        /// 档口实体
        /// </summary>
        public virtual StoreBooth StoreBooth { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品实体
        /// </summary>
        public virtual Product Product { get; set; }
    }
}
