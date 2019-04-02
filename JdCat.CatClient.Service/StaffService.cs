using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using Newtonsoft.Json;
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
            var codeKey = AddKeyPrefix("StaffCode", typeof(RedisEntity).Name);
            entity.Code = $"{DatabaseConfig.StaffPrefix}-{Database.StringIncrement(codeKey).ToString().PadLeft(7, '0')}";
            Save(entity);
            var key = AddKeyPrefix($"Alise:{entity.Alise}");
            Database.StringSet(key, entity.ObjectId);
        }

        public bool IsExistAlise(string alise)
        {
            var key = AddKeyPrefix($"Alise:{alise}");
            return Database.KeyExists(key);
        }

        public Staff GetStaffByAlise(string alise)
        {
            var businessIds = Database.SetMembers($"{DatabaseConfig.KeyPrefix}:Business");
            if (businessIds.Length == 0) return null;
            foreach (var id in businessIds)
            {
                var key = $"{DatabaseConfig.KeyPrefix}:{id}:Staff:Alise:{alise}";
                var result = Database.StringGet(key);
                if (result.IsNull) continue;
                var staffKey = $"{DatabaseConfig.KeyPrefix}:{id}:Staff:{result}";
                return JsonConvert.DeserializeObject<Staff>(Database.StringGet(staffKey));
            }
            return null;
        }

    }
}
