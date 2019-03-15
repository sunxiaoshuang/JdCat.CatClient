using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Service
{
    /// <summary>
    /// 员工服务
    ///     1. 员工实体
    ///     2. 员工登录名关联
    ///     3. 员工列表
    /// </summary>
    public class StaffService : BaseRedisService<Staff>, IStaffService
    {
        public StaffService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config) : base(connectionMultiplexer, config)
        {
        }

        public void SaveStaff(Staff entity)
        {
            if (!entity.IsExist())
            {
                entity.Code = CreateStaffCode();
            }
            Save(entity);
            var key = AddKeyPrefix($"Alise:{entity.Alise}");
            Database.StringSet(key, entity.ObjectId);
        }

        public bool IsExistAlise(string alise)
        {
            var key = AddKeyPrefix($"Alise:{alise}");
            return Database.KeyExists(key);
        }


        /// <summary>
        /// 创建员工编号
        /// </summary>
        /// <returns></returns>
        private string CreateStaffCode()
        {
            var lastObj = GetLastEntity();
            var max = 0;
            if(lastObj != null)
            {
                max = Convert.ToInt32(lastObj.Code.Split('-')[1]);
            }
            max = max + 1;
            return $"{DatabaseConfig.StaffPrefix}-{max.ToString().PadLeft(7, '0')}";
        }

    }
}
