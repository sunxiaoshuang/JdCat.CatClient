using JdCat.CatClient.Model;

using System.Windows.Controls;

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
        }
    }
}
