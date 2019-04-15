using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 支付方式
    /// </summary>
    [Serializable]
    public class PaymentType: ClientBaseEntity
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 支付类别
        /// </summary>
        public PaymentCategory Category { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
