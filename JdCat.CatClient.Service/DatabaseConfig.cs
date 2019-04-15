using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Service
{
    /// <summary>
    /// Redis数据库配置
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 键前缀
        /// </summary>
        public string KeyPrefix { get; set; }
        /// <summary>
        /// 远程服务器api
        /// </summary>
        public string Api { get; set; }
    }
}
