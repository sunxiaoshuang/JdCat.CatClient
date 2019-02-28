using Jiandanmao.ViewModel;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// Catering.xaml 的交互逻辑
    /// </summary>
    public partial class Catering : UserControl
    {
        public Catering(CateringViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.Transitioner = transitioner;
        }
    }
}
