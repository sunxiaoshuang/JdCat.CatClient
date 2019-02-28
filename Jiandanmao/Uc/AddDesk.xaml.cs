using Jiandanmao.Entity;
using System.Windows.Controls;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// AddDesk.xaml 的交互逻辑
    /// </summary>
    public partial class AddDesk : UserControl
    {
        public Desk Desk { get; set; }
        public AddDesk(Desk desk)
        {
            InitializeComponent();
            Desk = desk;
            DataContext = desk.Clone();
        }
    }
}
