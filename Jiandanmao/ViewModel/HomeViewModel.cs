
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
    /// HomeViewModel 绑定UserControler -> Home.xaml Mvvm
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);


        #endregion



        #region 界面绑定方法

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public HomeViewModel()
        {

        }

        public void Loaded(Object o)
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
                        Thread.Sleep(1000);
                        args.Session.Close(false);
                    });

                };

                new Thread(start).Start(); // 启动线程

            });

        }
        #endregion
    }
}
