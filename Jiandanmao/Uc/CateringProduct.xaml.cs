using System.Windows;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// CateringProduct.xaml 的交互逻辑
    /// </summary>
    public partial class CateringProduct : UserControl
    {
        public CateringProduct()
        {
            InitializeComponent();
        }

        public void Pre_Click(object sender, RoutedEventArgs e)
        {
            scrollProductType.ScrollToHorizontalOffset(scrollProductType.HorizontalOffset - 50);
        }
        public void Next_Click(object sender, RoutedEventArgs e)
        {
            scrollProductType.ScrollToHorizontalOffset(scrollProductType.HorizontalOffset + 50);
        }
    }
}
