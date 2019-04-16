using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    public class StaffPost : ClientBaseEntity
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 岗位拥有权限
        /// </summary>
        public StaffPostAuth Authority { get; set; }
        /// <summary>
        /// 岗位所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 岗位下的员工
        /// </summary>
        public virtual ICollection<Staff> Staffs { get; set; }
    }
}
