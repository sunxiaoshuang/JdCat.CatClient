using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Entity
{
    public abstract class ClientBaseEntity : IEntity
    {
        /// <summary>
        /// sqlite数据库唯一标识
        /// </summary>
        [Key]
        [Autoincrement]
        public int Id { get; set; }
        /// <summary>
        /// 生成一个guid，用作与服务器的数据库的关联
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [SqlDefaultValue(DefaultValue = "lower(hex(randomblob(16)))")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [SqlDefaultValue(DefaultValue = "datetime(current_timestamp,'localtime')")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 数据是否已经同步
        /// </summary>
        public bool IsSync { get; set; }
        /// <summary>
        /// 数据是否被删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
