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
    public abstract class BaseRedisService<T> : IBaseService<T> where T : BaseEntity
    {
        private static Type _type;
        static BaseRedisService()
        {
            _type = typeof(T);
        }

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


        protected string AddKeyPrefix(string key, string typeName = null)
        {
            return $"{DatabaseConfig.KeyPrefix}:{typeName ?? _type.Name}:{key}";
        }

        public void Save(T entity)
        {
            Save(entity, _type);
        }
        public void Save<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Save(entity, typeof(TEntity));
        }
        private void Save<TEntity>(TEntity entity, Type type) where TEntity : BaseEntity
        {
            if (!string.IsNullOrEmpty(entity.ObjectId))
            {
                throw new Exception("无法新增已存在[ObjectId]的实体对象");
            }

            entity.ObjectId = Guid.NewGuid().ToString().ToLower();
            entity.CreateTime = DateTime.Now;
            entity.ModifyTime = DateTime.Now;
            entity.Status = EntityStatus.Normal;

            var key = AddKeyPrefix(entity.ObjectId, type.Name);
            var val = JsonConvert.SerializeObject(entity);
            Database.StringSet(key, val);
            var listKey = AddKeyPrefix("List", type.Name);
            Database.ListRightPush(listKey, entity.ObjectId);
        }

        public void Update(T entity)
        {
            Update(entity, _type);
        }
        public void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Update(entity, typeof(TEntity));
        }
        private void Update<TEntity>(TEntity entity, Type type) where TEntity : BaseEntity
        {
            entity.ModifyTime = DateTime.Now;
            entity.Sync = false;
            var key = AddKeyPrefix(entity.ObjectId, type.Name);
            var val = JsonConvert.SerializeObject(entity);
            Database.StringSet(key, val);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            entity.Status = EntityStatus.Deleted;
            Update(entity);
        }

        public T Get(string objectId)
        {
            var key = AddKeyPrefix(objectId);
            return JsonConvert.DeserializeObject<T>(Database.StringGet(key));
        }

        public TEntity Get<TEntity>(string objectId) where TEntity : BaseEntity
        {
            var key = AddKeyPrefix(objectId, typeof(TEntity).Name);
            return JsonConvert.DeserializeObject<TEntity>(Database.StringGet(key));
        }

        public List<T> Get(params string[] objectIds)
        {
            var keys = objectIds.Select(a => (RedisKey)AddKeyPrefix(a)).ToArray();
            var vals = Database.StringGet(keys);
            var entitys = vals.Select(a => JsonConvert.DeserializeObject<T>(a)).ToList();
            return entitys;
        }

        public List<T> GetAll(EntityStatus status = EntityStatus.Normal)
        {
            var key = AddKeyPrefix("List");
            var objectIds = Database.ListRange(key, 0, -1);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get(objectIds.Select(a => a.ToString()).ToArray())
                .Where(a => (a.Status & status) > 0)
                .ToList();
        }

        public List<T> GetRange(PagingQuery paging, EntityStatus status = EntityStatus.Normal)
        {
            var key = AddKeyPrefix("List");
            var objectIds = Database.ListRange(key, paging.Skip, paging.Skip + paging.PageSize);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get(objectIds.Select(a => a.ToString()).ToArray())
                .Where(a => (a.Status & status) > 0)
                .ToList();
        }

        public long Length()
        {
            var key = AddKeyPrefix("List");
            return Database.ListLength(key);
        }

        public long Length<TEntity>() where TEntity : BaseEntity
        {
            var key = AddKeyPrefix("List", typeof(TEntity).Name);
            return Database.ListLength(key);
        }

        public void SyncFinish<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
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
            subscribe.Subscribe(channel, (a, b) => {
                action(a, b);
            });
        }

        /// <summary>
        /// 返回列表最后一个元素的对象
        /// </summary>
        /// <returns></returns>
        protected T GetLastEntity()
        {
            var key = AddKeyPrefix("List");
            var len = Database.ListLength(key);
            if (len == 0) return null;
            var id = Database.ListGetByIndex(key, len - 1).ToString();
            return Get(id);
        }


    }
}
