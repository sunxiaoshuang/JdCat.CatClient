using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 系统备注
    /// </summary>
    public class SystemMark : ClientBaseEntity
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标签类型
        /// </summary>
        public MarkCategory Category { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public virtual Business Business { get; set; }

    }
}
