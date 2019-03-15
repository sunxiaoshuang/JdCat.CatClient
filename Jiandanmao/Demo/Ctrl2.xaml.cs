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

namespace Jiandanmao.Demo
{
    /// <summary>
    /// Ctrl2.xaml 的交互逻辑
    /// </summary>
    public partial class Ctrl2 : UserControl
    {
        public Ctrl2()
        {
            InitializeComponent();
            this.FocusableChanged += Ctrl2_FocusableChanged;
            
        }

        private void Ctrl2_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
