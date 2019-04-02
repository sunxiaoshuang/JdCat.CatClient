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
    public class PaymentType: BaseEntity
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 支付编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 显示图标
        /// </summary>
        public SystemIcon Icon { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public PaymentTypeStatus TypeStatus { get; set; }
    }
}
