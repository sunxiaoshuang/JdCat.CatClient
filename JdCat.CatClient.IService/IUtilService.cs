using JdCat.CatClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IUtilService : IBaseService<RedisEntity>
    {
        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <param name="id">商户id</param>
        void InitDatabase(int id);
        /// <summary>
        /// 获取系统设置的餐位费
        /// </summary>
        /// <returns></returns>
        double GetMealFee();
        /// <summary>
        /// 设置系统餐位费
        /// </summary>
        /// <param name="mealFee"></param>
        /// <returns></returns>
        void SetMealFee(double mealFee);
        /// <summary>
        /// 获取口味列表
        /// </summary>
        /// <returns></returns>
        List<string> GetFlavors();
        /// <summary>
        /// 设置口味列表
        /// </summary>
        /// <param name="flavors"></param>
        void SetFlavors(IEnumerable<string> flavors);
        /// <summary>
        /// 保存登陆过的商户id
        /// </summary>
        /// <param name="id"></param>
        void SetLoginBusiness(int id);

    }
}
