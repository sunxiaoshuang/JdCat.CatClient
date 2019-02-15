
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
using System.Collections.Generic;
using Jiandanmao.Extension;
using Jiandanmao.Model;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class PrinterSettingViewModel : BaseViewModel
    {
        #region  声明

        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AddCommand => new AnotherCommandImplementation(Add);


        private ObservableCollection<Printer> _printers = new ObservableCollection<Printer>();
        public ObservableCollection<Printer> Printers
        {
            get { return _printers; }
            set { this.MutateVerbose(ref _printers, value, RaisePropertyChanged()); }
        }

        #endregion



        #region 界面绑定方法

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public void Loaded(object o)
        {
            Init(o);
        }

        private void Init(object o)
        {
            var loadingDialog = new LoadingDialog();

            var result = DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                ThreadStart start = delegate ()
                {
                    Mainthread.BeginInvoke((Action)delegate ()
                    {
                        args.Session.Close(false);
                        Printers = ApplicationObject.App.Printers;

                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }

        public async void Add(object o)
        {
            await DialogHost.Show(new AddPrinter(new Printer { Quantity = 1, Format = 80, Port = 9100, Id = "0", Type = 1, State = 1 }), "RootDialog");
        }

        #endregion


        public IEnumerable<string> PrinterType
        {
            get
            {
                yield return "前台打印机";
                yield return "后厨打印机";
            }
        }
        public IEnumerable<string> PrinterMode
        {
            get
            {
                yield return "一菜一打";
                yield return "一份一打";
                yield return "一单一打";
            }
        }
        public IEnumerable<string> PrinterState
        {
            get
            {
                yield return "正常";
                yield return "停用";
            }
        }
        public IEnumerable<string> PrinterFormat
        {
            get
            {
                yield return "80mm";
                yield return "58mm";
            }
        }
    }
}
