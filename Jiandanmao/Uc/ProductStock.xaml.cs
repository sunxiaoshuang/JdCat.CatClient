using Jiandanmao.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// ProductStock.xaml 的交互逻辑
    /// </summary>
    public partial class ProductStock : UserControl
    {
        public ProductStock()
        {
            InitializeComponent();
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            scrollProductType.ScrollToHorizontalOffset(scrollProductType.HorizontalOffset + 50);
        }
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            scrollProductType.ScrollToHorizontalOffset(scrollProductType.HorizontalOffset - 50);
        }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            e.Handled = re.IsMatch(e.Text);
        }


    }
}
