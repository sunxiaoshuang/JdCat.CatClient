using Jiandanmao.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// OrderInfo.xaml 的交互逻辑
    /// </summary>
    public partial class OrderInfo : UserControl
    {
        public OrderInfo(Order order)
        {
            InitializeComponent();
            this.DataContext = order;
            info.Content = $"#{order.Identifier} 配送信息";
            address.Content = order.ReceiverName + "-" + order.Phone + "-" + order.ReceiverAddress;
            fullReduce.Content = order.SaleFullReduce == null ? "" : ("-￥" + order.SaleFullReduce.ReduceMoney);
            coupon.Content = order.SaleCouponUser == null ? "" : ("-￥" + order.SaleCouponUser.Value);
        }
    }
}
