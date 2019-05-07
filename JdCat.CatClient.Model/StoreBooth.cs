using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 门店档口
    /// </summary>
    [Serializable]
    public class StoreBooth : ClientBaseEntity
    {
        /// <summary>
        /// 档口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 档口与商品关系
        /// </summary>
        public virtual ICollection<BoothProductRelative> BoothProductRelatives { get; set; }
    }
}
