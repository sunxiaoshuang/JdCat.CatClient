using JdCat.CatClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IStaffService : IBaseService<Staff>
    {
        /// <summary>
        /// 保存员工信息
        /// </summary>
        /// <param name="entity"></param>
        void SaveStaff(Staff entity);
        /// <summary>
        /// 是否存在登录名
        /// </summary>
        /// <param name="alise"></param>
        /// <returns></returns>
        bool IsExistAlise(string alise);
    }
}
