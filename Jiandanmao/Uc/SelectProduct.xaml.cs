using Jiandanmao.Extension;
using Jiandanmao.Model;
using Jiandanmao.ViewModel;
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
    /// SelectProduct.xaml 的交互逻辑
    /// </summary>
    public partial class SelectProduct : UserControl
    {
        public SelectProduct()
        {
            InitializeComponent();
        }

        public SelectProductViewModel ViewModel { get => (SelectProductViewModel)DataContext; }

        private void CardType_MouseUp(object sender, MouseEventArgs e)
        {
            ViewModel.Products.Clear();
            ViewModel.Types.ForEach(a => a.IsCheck = false);
            var type = (ProductType)((FrameworkElement)sender).DataContext;
            type.IsCheck = true;
            foreach (var item in type.Products)
            {
                ViewModel.Products.Add(item);
                if (!ViewModel.Printer.Foods.Any(a => a == item.ID)) continue;
                item.IsCheck = true;
            }
        }
        
    }
}
