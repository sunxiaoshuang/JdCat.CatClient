using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    [Serializable]
    public class BaseEntity
    {
        /// <summary>
        /// 远程数据库唯一标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
