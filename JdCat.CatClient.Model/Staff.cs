using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 员工
    /// </summary>
    [Serializable]
    public class Staff: BaseEntity
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 员工登录帐号
        /// </summary>
        public string Alise { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 员工编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }


    }
}
