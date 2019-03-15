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
    public abstract class BaseEntity
    {
        /// <summary>
        /// 远程数据库唯一标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 本地数据库唯一标识
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 是否已经同步
        /// </summary>
        public bool Sync { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 对象是否已经存在于数据库
        /// </summary>
        /// <returns></returns>
        public bool IsExist()
        {
            return !string.IsNullOrEmpty(ObjectId);
        }
    }
}
