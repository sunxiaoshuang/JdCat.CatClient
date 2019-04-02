using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
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
    /// 
    /// </summary>
    public class UtilService : BaseRedisService<RedisEntity>, IUtilService
    {
        public UtilService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config) : base(connectionMultiplexer, config)
        {
        }

        public void InitDatabase(int id)
        {
            DatabaseConfig.KeyPrefix += $":{id}";

            // 餐位费
            var key = AddKeyPrefix("MealFee");
            if (!Database.KeyExists(key))
            {
                Database.StringSet(key, 3);
            }
            // 口味
            key = AddKeyPrefix("Flavors");
            if (!Database.KeyExists(key))
            {
                Database.ListRightPush(key, new RedisValue[] { "不辣", "微辣", "中辣", "很辣" });
            }
            // 员工编号
            key = AddKeyPrefix("StaffCode");
            if (!Database.KeyExists(key))
            {
                Database.StringSet(key, 0);
            }
            // 支付类型编号
            key = AddKeyPrefix("PaymentTypeCode");
            if (!Database.KeyExists(key))
            {
                Database.StringSet(key, 0);
                // 初始化三种支付类型
                var payments = new List<PaymentType> {
                    new PaymentType{ BusinessId = id, Code = $"M{Database.StringIncrement(key).ToString().PadLeft(4, '0')}", CreateTime = DateTime.Now, Icon = SystemIcon.Money, Name = "现金", ObjectId = Guid.NewGuid().ToString().ToLower(), ModifyTime = DateTime.Now, Status = EntityStatus.Normal, TypeStatus = PaymentTypeStatus.Normal },
                    new PaymentType{ BusinessId = id, Code = $"M{Database.StringIncrement(key).ToString().PadLeft(4, '0')}", CreateTime = DateTime.Now, Icon = SystemIcon.Alipay, Name = "支付宝", ObjectId = Guid.NewGuid().ToString().ToLower(), ModifyTime = DateTime.Now, Status = EntityStatus.Normal, TypeStatus = PaymentTypeStatus.Normal },
                    new PaymentType{ BusinessId = id, Code = $"M{Database.StringIncrement(key).ToString().PadLeft(4, '0')}", CreateTime = DateTime.Now, Icon = SystemIcon.Wechat, Name = "微信", ObjectId = Guid.NewGuid().ToString().ToLower(), ModifyTime = DateTime.Now, Status = EntityStatus.Normal, TypeStatus = PaymentTypeStatus.Normal }
                };
                foreach (var item in payments)
                {
                    var paymentKey = AddKeyPrefix(item.ObjectId, typeof(PaymentType).Name);
                    Database.StringSet(paymentKey, JsonConvert.SerializeObject(item));
                }
                var listKey = AddKeyPrefix("List", typeof(PaymentType).Name);
                Database.ListRightPush(listKey, payments.Select(a => (RedisValue)a.ObjectId).ToArray());
            }
        }

        public double GetMealFee()
        {
            var key = AddKeyPrefix("MealFee");
            var result = Database.StringGet(key);
            return (double)result;
        }
        public void SetMealFee(double mealFee)
        {
            var key = AddKeyPrefix("MealFee");
            Database.StringSet(key, mealFee);
        }
        public List<string> GetFlavors()
        {
            var key = AddKeyPrefix("Flavors");
            var list = Database.ListRange(key);
            if (list == null || list.Count() == 0) return null;
            return list.Select(a => a.ToString()).ToList();
        }
        public void SetFlavors(IEnumerable<string> flavors)
        {
            var key = AddKeyPrefix("Flavors");
            Database.KeyDelete(key);
            Database.ListRightPush(key, flavors.Select(a => (RedisValue)a).ToArray());
        }
        public void SetLoginBusiness(int id)
        {
            var key = $"{DatabaseConfig.KeyPrefix}:Business";
            Database.SetAdd(key, id);
        }
    }
}
