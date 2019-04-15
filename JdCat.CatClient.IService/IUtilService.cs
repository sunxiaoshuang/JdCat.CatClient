using JdCat.CatClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IUtilService : IBaseService
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
        /// 根据用户名获取员工信息
        /// </summary>
        /// <param name="alise"></param>
        /// <returns></returns>
        Task<Staff> GetStaffByAliseAsync(string alise);
        /// <summary>
        /// 保存员工信息
        /// </summary>
        /// <param name="staffs"></param>
        /// <returns></returns>
        Task SaveStaffAsync(IEnumerable<Staff> staffs);
        /// <summary>
        /// 保存登陆过的商户
        /// </summary>
        /// <param name="business"></param>
        Task SetLoginBusinessAsync(Business business);
        /// <summary>
        /// 读取所有的商品类别以及类别下的商品
        /// </summary>
        /// <returns></returns>
        Task<List<ProductType>> GetProductTypeAsync();
        /// <summary>
        /// 读取所有的区域以及区域以下的餐台
        /// </summary>
        /// <returns></returns>
        Task<List<DeskType>> GetDeskTypesAsync();

    }
}
