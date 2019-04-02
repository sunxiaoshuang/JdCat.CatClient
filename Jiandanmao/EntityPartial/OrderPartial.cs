using JdCat.CatClient.Common;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Jiandanmao.Entity
{
    public partial class Order
    {
        /// <summary>
        /// 是否已获取订单详情
        /// </summary>
        [JsonIgnore]
        public bool IsDetail { get; set; }
        /// <summary>
        /// 打印订单
        /// </summary>
        public ICommand PrintCommand => new AnotherCommandImplementation(Print);
        /// <summary>
        /// 查看订单详情
        /// </summary>
        public ICommand CatCommand => new AnotherCommandImplementation(Cat);

        private async void Print(object obj)
        {
            var order = await Request.GetOrderDetail(ID);
            ApplicationObject.Print(order, int.Parse(obj.ToString()));
        }


        private async void Cat(object obj)
        {
            Order order = this;
            if (!this.IsDetail)
            {
                order = await Request.GetOrderDetail(ID);
                order.IsDetail = true;
                ((OrderListViewModel)((OrderList)obj).DataContext).Items.Replace(this, order);
            }

            var orderInfo = new OrderInfo(order);

            await DialogHost.Show(orderInfo, "RootDialog");
        }
    }
}
