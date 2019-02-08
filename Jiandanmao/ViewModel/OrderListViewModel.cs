
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class OrderListViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        private ObservableCollection<OrderListInfoViewModel> _items;
        public ObservableCollection<OrderListInfoViewModel> Items => _items;

        private int pageSize = 20;
        private int pageIndex = 1;
        private int recordCount = 0;

        #endregion



        #region 界面绑定方法

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public OrderListViewModel()
        {

        }

        public void Loaded(object o)
        {
            Init(o);
        }


        /// <summary>
        /// 初始化，读取在线配置信息
        /// </summary>
        private void Init(object o)
        {
            var loadingDialog = new LoadingDialog();

            var result = DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                ThreadStart start = delegate ()
                {
                    Mainthread.BeginInvoke((Action)delegate ()// 异步更新界面
                    {
                        //Thread.Sleep(1000);
                        args.Session.Close(false);
                    });

                };

                new Thread(start).Start(); // 启动线程

            });

        }

        private 
        #endregion
    }
}
