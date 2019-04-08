
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Entity;
using JdCat.CatClient.Common;

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
        /// <summary>
        /// 是否提交修改
        /// </summary>
        public bool IsSubmit { get; set; }

        #endregion

        #region 界面绑定方法

        public ObservableCollection<ProductType> Types { get; } = new ObservableCollection<ProductType>();
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public Printer Printer { get; set; }


        public async void Loaded(object o)
        {
            //Init(o);
            var types = await Request.GetProductsAsync(ApplicationObject.App.Business.ID);
            //res.Wait();
            //var types = res.Result;
            if (types != null && types.Count > 0)
            {
                // 选中第一个分类
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
                types.ForEach(a => {
                    Types.Add(a);
                    // 勾选已经选择过的商品
                    if (a.Products!= null && a.Products.Count > 0)
                    {
                        a.Products.ForEach(item =>
                        {
                            if (!Printer.Foods.Any(b => b == item.ID)) return;
                            item.IsCheck = true;
                        });
                    }
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
        public override void Submit(object o)
        {
            IsSubmit = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        #endregion
    }
}
