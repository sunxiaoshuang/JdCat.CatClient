using Jiandanmao.Code;
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
using System.Windows.Threading;

namespace Jiandanmao.Uc
{
    /// <summary>
    /// DialogAsync.xaml 的交互逻辑
    /// </summary>
    public partial class DialogSync : UserControl
    {
        private Dispatcher Mainthread = Dispatcher.CurrentDispatcher;
        public DialogSync()
        {
            InitializeComponent();
            //Sync();
        }

        private async void Sync()
        {
            var data = await Request.SynchronousAsync(ApplicationObject.App.Business.Id);
        }
    }
}
