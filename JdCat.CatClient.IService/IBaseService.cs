using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IBaseService
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="entity"></param>
        void Save<TEntity>(TEntity entity) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 保存远程读取的数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitys"></param>
        Task SaveRemoteDataAsync<TEntity>(IEnumerable<TEntity> entitys) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 移除对象实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Remove<TEntity>(TEntity entity) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 更新对象实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Update<TEntity>(TEntity entity) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 根据id获取实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string objectId) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取实体对象的记录数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<long> GetLengthAsync<TEntity>() where TEntity : ClientBaseEntity;
        /// <summary>
        /// 根据id获取实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync<TEntity>(string objectId) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 根据id列表获取实体对象列表
        /// </summary>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        List<TEntity> Get<TEntity>(params string[] objectIds) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 根据id列表获取实体对象列表
        /// </summary>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetAsync<TEntity>(params string[] objectIds) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 根据编码读取实体对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<TEntity> GetEntityByCodeAsync<TEntity>(string code) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取指定页数的实体对象，
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="paging">分页对象</param>
        /// <param name="reversal">是否反向获取</param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetAsync<TEntity>(PagingQuery paging, bool reversal = true, EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取类型所有的对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="status"></param>
        /// <returns></returns>
        List<TEntity> GetAll<TEntity>(EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取类型所有的对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetAllAsync<TEntity>(EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取指定页码的对象实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="paging"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<TEntity> GetRange<TEntity>(PagingQuery paging, EntityStatus status = EntityStatus.Normal) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 获取指定实体列表的长度
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        long Length<TEntity>() where TEntity : ClientBaseEntity;
        /// <summary>
        /// 指定的列表数据同步完成
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        void SyncFinish<TEntity>(IEnumerable<TEntity> entities) where TEntity : ClientBaseEntity;
        /// <summary>
        /// 发布订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        void PubSubscribe(string channel, string message);
        /// <summary>
        /// 发布订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        void PubSubscribe(string channel, byte[] message);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="action"></param>
        void Subscribe(string channel, Action<string, object> action);
    }
}
