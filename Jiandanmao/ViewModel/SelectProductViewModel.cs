
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
using Jiandanmao.Model;
using Jiandanmao.Extension;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// SelectProductViewModel 绑定UserControler -> SelectProduct.xaml Mvvm
    /// </summary>
    public class SelectProductViewModel : BaseViewModel
    {
        public SelectProductViewModel(Printer printer)
        {
            Printer = printer;
        }
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand CheckCommand => new AnotherCommandImplementation(Check);


        #endregion

        #region 界面绑定方法

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public ObservableCollection<ProductType> Types { get; } = new ObservableCollection<ProductType>();
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public Printer Printer { get; set; }


        public async void Loaded(object o)
        {
            //Init(o);
            var types = await Request.GetProducts(ApplicationObject.App.Business.ID);
            //res.Wait();
            //var types = res.Result;
            if (types != null && types.Count > 0)
            {
                types.ForEach(a => Types.Add(a));
                var type = types.First();
                type.IsCheck = true;
                var products = type.Products;
                if (products != null && products.Count > 0)
                {
                    foreach (var item in products)
                    {
                        Products.Add(item);
                    }
                }
                Products.ForEach(item =>
                {
                    if (!Printer.Foods.Any(a => a == item.ID)) return;
                    item.IsCheck = true;
                });
            }
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
        private void Check(object o)
        {
            var obj = (CheckBox)o;
            Products.ForEach(a => a.IsCheck = obj.IsChecked.Value);
        }
        #endregion
    }
}
