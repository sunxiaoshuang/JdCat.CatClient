using Jiandanmao.Entity;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// AddDeskType.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeskType : UserControl
    {
        public DeskType Type { get; set; }
        public AddDeskType(DeskType type)
        {
            InitializeComponent();
            Type = type;
            DataContext = type.Clone();
        }
    }
}
