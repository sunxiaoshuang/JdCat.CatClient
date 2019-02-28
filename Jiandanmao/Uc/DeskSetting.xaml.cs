using Jiandanmao.Entity;
using Jiandanmao.Extension;
using Jiandanmao.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// Desk.xaml 的交互逻辑
    /// </summary>
    public partial class DeskSetting : UserControl
    {
        public DeskSetting()
        {
            InitializeComponent();
        }

        public DeskViewModel ViewModel { get => (DeskViewModel)DataContext; }

        private void TypeChange(object sender, RoutedEventArgs e)
        {
            var type = (DeskType)((FrameworkElement)sender).DataContext;
            if (type.IsCheck) return;
            ViewModel.Types.ForEach(a => a.IsCheck = false);
            type.IsCheck = true;
            ViewModel.Desks = type.Desks;
        }
    }
}
