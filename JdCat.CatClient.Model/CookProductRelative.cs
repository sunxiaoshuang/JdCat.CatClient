using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 厨师商品关系
    /// </summary>
    public class CookProductRelative : ClientBaseEntity
    {
        /// <summary>
        /// 员工id
        /// </summary>
        public int StaffId { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }

    }
}
