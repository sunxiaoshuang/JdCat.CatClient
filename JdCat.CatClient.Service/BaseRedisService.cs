using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Service
{
    /// <summary>
    /// Redis服务基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRedisService : IBaseService
    {

        public BaseRedisService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config)
        {
            _connectionMultiplexer = connectionMultiplexer;
            DatabaseConfig = config;
        }

        /// <summary>
        /// Redis连接
        /// </summary>
        private IConnectionMultiplexer _connectionMultiplexer;

        private IDatabase _database;
        /// <summary>
        /// 访问的数据库
        /// </summary>
        protected IDatabase Database
        {
            get
            {
                if (_database == null) _database = _connectionMultiplexer.GetDatabase();
                return _database;
            }
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        protected DatabaseConfig DatabaseConfig { get; }

        /// <summary>
        /// Redis对象添加前缀
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string AddKeyPrefix<TEntity>(string key)
        {
            return AddKeyPrefix(key, typeof(TEntity).Name);
        }

        /// <summary>
        /// Redis对象添加前缀
        /// </summary>
        /// <param name="key"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected string AddKeyPrefix(string key, string typeName)
        {
            return $"{DatabaseConfig.KeyPrefix}:{typeName}:{key}";
        }

        public void Save<TEntity>(TEntity entity) where TEntity : ClientBaseEntity
        {
            if (!string.IsNullOrEmpty(entity.ObjectId))
            {
                throw new Exception("无法新增已存在[ObjectId]的实体对象");
            }

            entity.ObjectId = Guid.NewGuid().ToString().ToLower();
            entity.CreateTime = DateTime.Now;
            entity.ModifyTime = DateTime.Now;
            entity.Status = EntityStatus.Normal;

            var key = AddKeyPrefix<TEntity>(entity.ObjectId);
            var val = JsonConvert.SerializeObject(entity);
            Database.StringSet(key, val);
            var listKey = AddKeyPrefix<TEntity>("List");
            Database.ListRightPush(listKey, entity.ObjectId);
        }

        public async Task SaveRemoteDataAsync<TEntity>(IEnumerable<TEntity> entitys) where TEntity : ClientBaseEntity
        {
            var listKey = AddKeyPrefix<TEntity>("List");
            await Database.KeyDeleteAsync(listKey);
            if (entitys == null || entitys.Count() == 0) return;
            var typeName = typeof(TEntity).Name;
            var redisArr = new List<KeyValuePair<RedisKey, RedisValue>>();
            foreach (var item in entitys)
            {
                item.ObjectId = item.Id.ToString();
                redisArr.Add(new KeyValuePair<RedisKey, RedisValue>(AddKeyPrefix(item.ObjectId, typeName), JsonConvert.SerializeObject(item)));
            }
            await Database.StringSetAsync(redisArr.ToArray());
            await Database.ListRightPushAsync(listKey, entitys.Select(a => (RedisValue)a.ObjectId).ToArray());
        }

        public void Update<TEntity>(TEntity entity) where TEntity : ClientBaseEntity
        {
            entity.ModifyTime = DateTime.Now;
            entity.Sync = false;
            var key = AddKeyPrefix<TEntity>(entity.ObjectId);
            var val = JsonConvert.SerializeObject(entity);
            Database.StringSet(key, val);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : ClientBaseEntity
        {
            entity.Status = EntityStatus.Deleted;
            Update(entity);
        }

        public TEntity Get<TEntity>(string objectId) where TEntity : ClientBaseEntity
        {
            var result = Database.StringGet(AddKeyPrefix<TEntity>(objectId));
            if (result.IsNullOrEmpty) return null;
            return JsonConvert.DeserializeObject<TEntity>(result);
        }
        public async Task<long> GetLengthAsync<TEntity>() where TEntity : ClientBaseEntity
        {
            return await Database.ListLengthAsync(AddKeyPrefix<TEntity>("List"));
        }
        public async Task<TEntity> GetAsync<TEntity>(string objectId) where TEntity : ClientBaseEntity
        {
            var result = await Database.StringGetAsync(AddKeyPrefix<TEntity>(objectId));
            if (result.IsNullOrEmpty) return null;
            return JsonConvert.DeserializeObject<TEntity>(result);
        }

        public List<TEntity> Get<TEntity>(params string[] objectIds) where TEntity : ClientBaseEntity
        {
            var typeName = typeof(TEntity).Name;
            var keys = objectIds.Select(a => (RedisKey)AddKeyPrefix(a, typeName)).ToArray();
            var vals = Database.StringGet(keys);
            var entitys = vals.Select(a => JsonConvert.DeserializeObject<TEntity>(a)).ToList();
            return entitys;
        }
        public async Task<List<TEntity>> GetAsync<TEntity>(params string[] objectIds) where TEntity : ClientBaseEntity
        {
            var typeName = typeof(TEntity).Name;
            var keys = objectIds.Select(a => (RedisKey)AddKeyPrefix(a, typeName)).ToArray();
            var vals = await Database.StringGetAsync(keys);
            var entitys = vals.Select(a => JsonConvert.DeserializeObject<TEntity>(a)).ToList();
            return entitys;
        }
        public async Task<TEntity> GetEntityByCodeAsync<TEntity>(string code) where TEntity : ClientBaseEntity
        {
            var objectId = await Database.StringGetAsync(AddKeyPrefix<TEntity>(code));
            if (objectId.IsNullOrEmpty) return null;
            return await GetAsync<TEntity>(objectId);
        }

        public async Task<List<TEntity>> GetAsync<TEntity>(PagingQuery paging, bool reversal = true, EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity
        {
            long start, end;
            if (reversal)
            {
                var len = paging.RecordCount == 0 ? await GetLengthAsync<TEntity>() : paging.RecordCount;
                paging.RecordCount = (int)len;
                start = len - paging.PageIndex * paging.PageSize;
            }
            else
            {
                start = (paging.PageIndex - 1) * paging.PageSize;
            }
            end = start + paging.PageSize - 1;
            var objectIds = await Database.ListRangeAsync(AddKeyPrefix<TEntity>("List"), start, end);
            if (objectIds == null || objectIds.Count() == 0) return null;
            var entitys = (await GetAsync<TEntity>(objectIds.Select(a => a.ToString()).ToArray()))
                .Where(a => (a.Status & status) > 0)
                .ToList();
            return entitys;
        }

        public List<TEntity> GetAll<TEntity>(EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity
        {
            var key = AddKeyPrefix<TEntity>("List");
            var objectIds = Database.ListRange(key, 0, -1);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get<TEntity>(objectIds.Select(a => a.ToString()).ToArray())
                .Where(a => (a.Status & status) > 0)
                .ToList();
        }
        public async Task<List<TEntity>> GetAllAsync<TEntity>(EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity
        {
            var key = AddKeyPrefix<TEntity>("List");
            var objectIds = await Database.ListRangeAsync(key, 0, -1);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return (await GetAsync<TEntity>(objectIds.Select(a => a.ToString()).ToArray()))
                .Where(a => (a.Status & status) > 0)
                .ToList();
        }

        public List<TEntity> GetRange<TEntity>(PagingQuery paging, EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity
        {
            var key = AddKeyPrefix<TEntity>("List");
            var objectIds = Database.ListRange(key, paging.Skip, paging.Skip + paging.PageSize);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get<TEntity>(objectIds.Select(a => a.ToString()).ToArray())
                .Where(a => (a.Status & status) > 0)
                .ToList();
        }

        public long Length<TEntity>() where TEntity : ClientBaseEntity
        {
            var key = AddKeyPrefix<TEntity>("List");
            return Database.ListLength(key);
        }

        public void SyncFinish<TEntity>(IEnumerable<TEntity> entities) where TEntity : ClientBaseEntity
        {
            var typeName = typeof(TEntity).Name;
            var key = string.Empty;
            var pairs = entities.Select(item =>
            {
                item.Sync = true;
                return new KeyValuePair<RedisKey, RedisValue>(AddKeyPrefix(item.ObjectId, typeName), JsonConvert.SerializeObject(item));
            }).ToArray();
            Database.StringSet(pairs);
        }

        public void PubSubscribe(string channel, string message)
        {
            _connectionMultiplexer.GetSubscriber().Publish(channel, message);
        }

        public void PubSubscribe(string channel, byte[] message)
        {
            _connectionMultiplexer.GetSubscriber().Publish(channel, message);
        }

        public void Subscribe(string channel, Action<string, object> action)
        {
            var subscribe = _connectionMultiplexer.GetSubscriber();
            subscribe.Subscribe(channel, (a, b) =>
            {
                action(a, b);
            });
        }

        /// <summary>
        /// 返回列表最后一个元素的对象
        /// </summary>
        /// <returns></returns>
        protected TEntity GetLastEntity<TEntity>() where TEntity : ClientBaseEntity
        {
            var key = AddKeyPrefix<TEntity>("List");
            var len = Database.ListLength(key);
            if (len == 0) return null;
            var id = Database.ListGetByIndex(key, len - 1).ToString();
            return Get<TEntity>(id);
        }


    }
}
