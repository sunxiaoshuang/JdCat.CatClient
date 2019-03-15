using Jiandanmao.ViewModel;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// CateringOrder.xaml 的交互逻辑
    /// </summary>
    public partial class CateringOrder : UserControl
    {
        public CateringOrder()
        {
            var viewModel = DataContext as CateringViewModel;
            //MaterialDesignThemes.Wpf.Transitions.TransitionEffectKind.SlideInFromLeft
            InitializeComponent();
        }
    }
}
