using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
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
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Remove(T entity);
        void Update(T entity);
        T Get(string objectId);
        List<T> Get(params string[] objectIds);
        List<T> GetAll();
        List<T> GetRange(PagingQuery paging);
    }
}
