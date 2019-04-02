using JdCat.CatClient.Model;
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Code
{
    [Serializable]
    public class PrintOption
    {
        /// <summary>
        /// 【0：前后台均出票，1：前台出票，2：后台出票】
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 打印标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 打印模式
        /// </summary>
        public PrintMode Mode { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public TangOrder Order { get; set; }
        /// <summary>
        /// 打印菜品
        /// </summary>
        public IEnumerable<TangOrderProduct> Products { get; set; }

    }
}
