using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 客户端实体类基类
    /// </summary>
    [Serializable]
    public abstract class ClientBaseEntity : BaseEntity
    {
        /// <summary>
        /// 本地数据库唯一标识
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 是否已经同步
        /// </summary>
        public bool Sync { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityStatus Status { get; set; } = EntityStatus.Normal;
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
