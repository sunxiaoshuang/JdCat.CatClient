using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
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
        private static string _typeName;
        private static bool _isInit;
        static BaseRedisService()
        {
            _typeName = typeof(T).Name;
        }

        public BaseRedisService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config)
        {
            _connectionMultiplexer = connectionMultiplexer;
            DatabaseConfig = config;
            //InitDatabase();
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
        /// 初始化数据库
        /// </summary>
        private void InitDatabase()
        {
            if (_isInit) return;

            // todo

            _isInit = true;
        }

        protected string AddKeyPrefix(string key)
        {
            return $"{DatabaseConfig.KeyPrefix}:{_typeName}:{key}";
        }

        public void Save(T entity)
        {
            entity.ObjectId = Guid.NewGuid().ToString().ToLower();
            entity.CreateTime = DateTime.Now;
            entity.ModifyTime = DateTime.Now;
            entity.Status = EntityStatus.Normal;

            var key = AddKeyPrefix(entity.ObjectId);
            var val = UtilHelper.Serialize(entity);
            Database.StringSet(key, val);
            var listKey = AddKeyPrefix("List");
            Database.ListRightPush(listKey, entity.ObjectId);
        }

        public void Update(T entity)
        {
            entity.ModifyTime = DateTime.Now;
            entity.Sync = false;
            var key = AddKeyPrefix(entity.ObjectId);
            var val = UtilHelper.Serialize(entity);
            Database.StringSet(key, val);
        }

        public void Remove(T entity)
        {
            entity.Status = EntityStatus.Deleted;
            Update(entity);
        }

        public T Get(string objectId)
        {
            var key = AddKeyPrefix(objectId);
            return UtilHelper.Deserialize<T>(Database.StringGet(key));
        }

        public List<T> Get(params string[] objectIds)
        {
            var keys = objectIds.Select(a => (RedisKey)AddKeyPrefix(a)).ToArray();
            var vals = Database.StringGet(keys);
            var entitys = vals.Select(a => UtilHelper.Deserialize<T>(a)).ToList();
            return entitys;
        }

        public List<T> GetAll()
        {
            var key = AddKeyPrefix("List");
            var objectIds = Database.ListRange(key, 0, -1);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get(objectIds.Select(a => a.ToString()).ToArray())
                .Where(a => a.Status == EntityStatus.Normal)
                .ToList();
        }

        public List<T> GetRange(PagingQuery paging)
        {
            var key = AddKeyPrefix("List");
            var objectIds = Database.ListRange(key, paging.Skip, paging.Skip + paging.PageSize);
            if (objectIds == null || objectIds.Count() == 0) return null;
            return Get(objectIds.Select(a => a.ToString()).ToArray());
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
