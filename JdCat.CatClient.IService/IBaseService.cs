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
    public interface IBaseService<T> where T : BaseEntity
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="entity"></param>
        void Save(T entity);
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="entity"></param>
        void Save<TEntity>(TEntity entity) where TEntity : BaseEntity;
        /// <summary>
        /// 移除对象实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;
        /// <summary>
        /// 更新对象实体
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// 更新对象实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Update<TEntity>(TEntity entity) where TEntity : BaseEntity;
        /// <summary>
        /// 根据id获取实体对象
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        T Get(string objectId);
        /// <summary>
        /// 根据id获取实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string objectId) where TEntity : BaseEntity;
        /// <summary>
        /// 根据id列表获取实体对象列表
        /// </summary>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        List<T> Get(params string[] objectIds);
        /// <summary>
        /// 获取指定状态的列表所有实体对象
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<T> GetAll(EntityStatus status = EntityStatus.Normal);
        /// <summary>
        /// 获取指定页数的实体对象列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<T> GetRange(PagingQuery paging, EntityStatus status = EntityStatus.Normal);
        /// <summary>
        /// 获取列表的长度
        /// </summary>
        /// <returns></returns>
        long Length();
        /// <summary>
        /// 获取指定实体列表的长度
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        long Length<TEntity>() where TEntity : BaseEntity;
        /// <summary>
        /// 指定的列表数据同步完成
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        void SyncFinish<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
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
