
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
using Autofac;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using System.Collections.Generic;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.Common;

namespace Jiandanmao.ViewModel
{
    public class ProductStockViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AllCommand => new AnotherCommandImplementation(All);
        public ICommand SelectTypeCommand => new AnotherCommandImplementation(SelectType);
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        public ICommand SearchCommand => new AnotherCommandImplementation(Search);
        public ICommand ClearCommand => new AnotherCommandImplementation(Clear);

        #endregion

        #region 属性

        private ObservableCollection<ProductType> _productTypes;
        /// <summary>
        /// 商品类别集合
        /// </summary>
        public ObservableCollection<ProductType> ProductTypes { get => _productTypes; set => this.MutateVerbose(ref _productTypes, value, RaisePropertyChanged()); }

        private ObservableCollection<Product> _products;
        /// <summary>
        /// 商品集合
        /// </summary>
        public ObservableCollection<Product> Products { get => _products; set => this.MutateVerbose(ref _products, value, RaisePropertyChanged()); }

        private ListObject<Product> _productObject = new ListObject<Product>(20);
        public ListObject<Product> ProductObject { get => _productObject; set => this.MutateVerbose(ref _productObject, value, RaisePropertyChanged()); }

        private bool _isAllProduct;
        /// <summary>
        /// 是否选中全部商品
        /// </summary>
        public bool IsAllProduct { get => _isAllProduct; set => this.MutateVerbose(ref _isAllProduct, value, RaisePropertyChanged()); }
        

        #endregion

        #region 界面绑定方法

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
                void start()
                {
                    Mainthread.BeginInvoke((Action)delegate ()// 异步更新界面
                    {
                        args.Session.Close(false);

                        ProductTypes = ApplicationObject.App.Types;
                        Products = ApplicationObject.App.Products.Where(a => (a.Scope & ActionScope.Store) > 0).ToObservable();

                        ProductObject.OriginalList = Products;
                    });
                }

                new Thread(start).Start(); // 启动线程

            });
        }

        private void All(object o)
        {
            IsAllProduct = true;
            ProductTypes.ForEach(a => a.IsCheck = false);
            ProductObject.OriginalList = Products;
        }

        private void SelectType(object o)
        {
            var type = (ProductType)o;
            if (type.IsCheck) return;
            IsAllProduct = false;
            ProductTypes.ForEach(a => a.IsCheck = false);
            type.IsCheck = true;
            ProductObject.OriginalList = type.Products?.Where(a => (a.Scope & ActionScope.Store) > 0).ToObservable();
        }

        private async void Save(object o)
        {
            var dialog = (DialogHost)o;
            var product = (Product)dialog.DataContext;
            var num = Convert.ToDouble(((TextBox)dialog.Tag).Text);
            product.Stock = num;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                var stock = new ProductStockModel { ProductId = product.Id, Stock = num };
                await service.SetProductStocksAsync(stock);
                service.PubSubscribe("SystemMessage", $"StockChange|{stock.ToJson()}");
            }
            DialogHost.CloseDialogCommand.Execute(o, null);
        }

        private void Search(object o)
        {
            var key = ((TextBox)o).Text?.Trim();
            IsAllProduct = true;
            ProductTypes.ForEach(a => a.IsCheck = false);

            var products = ApplicationObject.App.Products.Where(a => (a.Scope & ActionScope.Store) > 0);
            if (!string.IsNullOrEmpty(key))
            {
                products = products.Where(a => a.DisplayName.Contains(key) || a.Pinyin.Contains(key) || a.FirstLetter.Contains(key));
            }
            ProductObject.OriginalList = products.ToObservable();
        }

        private void Clear(object o)
        {
            ((TextBox)o).Text = string.Empty;
        }


        #endregion
    }
}
