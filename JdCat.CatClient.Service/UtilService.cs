using JdCat.CatClient.Common;
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
    public class UtilService : BaseRedisService, IUtilService
    {
        public UtilService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config) : base(connectionMultiplexer, config)
        {
        }

        public void InitDatabase(int id)
        {
            DatabaseConfig.KeyPrefix += $":{id}";

            // 餐位费
            var key = AddKeyPrefix<RedisEntity>("MealFee");
            if (!Database.KeyExists(key))
            {
                Database.StringSet(key, 1);
            }
        }

        public double GetMealFee()
        {
            var key = AddKeyPrefix<RedisEntity>("MealFee");
            var result = Database.StringGet(key);
            return (double)result;
        }

        public void SetMealFee(double mealFee)
        {
            var key = AddKeyPrefix<RedisEntity>("MealFee");
            Database.StringSet(key, mealFee);
        }

        public async Task<Staff> GetStaffByAliseAsync(string alise)
        {
            var businessIds = Database.ListRange(AddKeyPrefix<Business>("List"));
            if (businessIds.Length == 0) return null;
            var typeName = typeof(Staff).Name;
            foreach (var item in businessIds)
            {
                var key = $"{DatabaseConfig.KeyPrefix}:{item}:{typeName}:{alise}";
                var id = await Database.StringGetAsync(key);
                if (id.IsNullOrEmpty) continue;
                var staff = await Database.StringGetAsync($"{DatabaseConfig.KeyPrefix}:{item}:{typeName}:{id}");
                if (staff.IsNullOrEmpty) continue;
                var entity = JsonConvert.DeserializeObject<Staff>(staff);
                var post = await Database.StringGetAsync($"{DatabaseConfig.KeyPrefix}:{item}:{typeof(StaffPost).Name}:{entity.StaffPostId}");
                entity.StaffPost = JsonConvert.DeserializeObject<StaffPost>(post);
                return entity;
            }
            return null;
        }

        public async Task SaveStaffAsync(IEnumerable<Staff> staffs)
        {
            await SaveRemoteDataAsync(staffs);
            if (staffs == null || staffs.Count() == 0) return;
            var pairs = new List<KeyValuePair<RedisKey, RedisValue>>();
            var typeName = typeof(Staff).Name;
            foreach (var item in staffs)
            {
                pairs.Add(new KeyValuePair<RedisKey, RedisValue>(AddKeyPrefix(item.Alise, typeName), item.ObjectId));
            }
            await Database.StringSetAsync(pairs.ToArray());
        }

        public async Task SetLoginBusinessAsync(Business business)
        {
            await SaveRemoteDataAsync(new List<Business> { business });
            await Database.StringSetAsync(AddKeyPrefix<Business>(business.Code), business.ObjectId);

        }

        public async Task<List<ProductType>> GetProductTypeAsync()
        {
            var types = await GetAllAsync<ProductType>();
            if (types == null) return null;
            var products = await GetAllAsync<Product>();
            if (products == null) return types;
            types.ForEach(a => a.Products = products.Where(b => b.ProductTypeId == a.Id).ToObservable());
            return types;
        }

        public async Task<List<DeskType>> GetDeskTypesAsync()
        {
            var types = await GetAllAsync<DeskType>();
            if (types == null) return null;
            var desks = await GetAllAsync<Desk>();
            if (desks == null) return types;
            types.ForEach(a => a.Desks = desks.Where(b => b.DeskTypeId == a.Id).ToObservable());
            return types;
        }


    }
}
