using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 第三方订单优惠信息
    /// </summary>
    public class ThirdOrderActivity : BaseEntity
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public int ActiveId { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double ReduceFee { get; set; }
        /// <summary>
        /// 第三方承担金额
        /// </summary>
        public double ThirdCharge { get; set; }
        /// <summary>
        /// 门店承担金额
        /// </summary>
        public double PoiCharge { get; set; }
        /// <summary>
        /// 优惠备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 优惠方式
        /// </summary>
        public int Type { get; set; }


        /// <summary>
        /// 所属订单id
        /// </summary>
        public int ThirdOrderId { get; set; }
        /// <summary>
        /// 所属订单实体
        /// </summary>
        public virtual ThirdOrder ThirdOrder { get; set; }
    }
}
