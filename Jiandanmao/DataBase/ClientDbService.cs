using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.DataBase
{
    public class ClientDbService
    {
        public CatDbContext Context { get; set; }
        public ClientDbService(CatDbContext _context)
        {
            Context = _context;
        }

        public T Get<T>(int id) where T : ClientBaseEntity
        {
            return Context.Set<T>().FirstOrDefault(a => a.Id == id);
        }
        public async Task<int> AddAsync<T>(T entity) where T : ClientBaseEntity
        {
            Context.Set<T>().Add(entity);
            return await Context.SaveChangesAsync();
        }
        public int Add<T>(T entity) where T : ClientBaseEntity
        {
            Context.Set<T>().Add(entity);
            return Context.SaveChanges();
        }
        public int Delete<T>(T entity) where T : ClientBaseEntity, new()
        {
            return Delete<T>(entity.Id);
        }
        public async Task<int> DeleteAsync<T>(T entity) where T : ClientBaseEntity, new()
        {
            return await DeleteAsync<T>(entity.Id);
        }
        public async Task<int> DeleteAsync<T>(int id) where T : ClientBaseEntity, new()
        {
            var model = Context.Set<T>().FirstOrDefault(a => a.Id == id);
            if (model == null) return 0;
            model.IsDelete = true;
            model.IsSync = false;
            return await Context.SaveChangesAsync();
        }
        public int Delete<T>(int id) where T : ClientBaseEntity, new()
        {
            var model = Context.Set<T>().FirstOrDefault(a => a.Id == id);
            if (model == null) return 0;
            model.IsDelete = true;
            model.IsSync = false;
            return Context.SaveChanges();
        }

        /// <summary>
        /// 获取正在消费中的订单
        /// </summary>
        /// <param name="businessId">商户id</param>
        /// <returns></returns>
        public List<StoreOrder> GetUsingOrder(int businessId)
        {
            return Context.StoreOrders.Where(a => !a.IsDelete && a.BusinessId == businessId && (a.Status & StoreOrderStatus.Using) > 0).ToList();
        }

    }
}
