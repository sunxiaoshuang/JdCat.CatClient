using System.Windows;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// CateringDesk.xaml 的交互逻辑
    /// </summary>
    public partial class CateringDesk : UserControl
    {
        public CateringDesk()
        {
            InitializeComponent();
        }
        public void Pre_Click(object sender, RoutedEventArgs e)
        {
            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - 50);
        }
        public void Next_Click(object sender, RoutedEventArgs e)
        {
            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + 50);
        }
    }
}
