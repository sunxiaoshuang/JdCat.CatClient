using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jiandanmao.ViewModel
{
    public class FastFoodViewModel : BaseViewModel
    {
        public FastFoodViewModel()
        {
            Types = ApplicationObject.App.Types;
            Products = ApplicationObject.App.Products;
            SubscribeSystemMessage();
        }

        private TextBox txtKey;
        private ScrollViewer scrollorderProduct;

        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AllTypeCommand => new AnotherCommandImplementation(AllType);
        public ICommand TypeCheckedCommand => new AnotherCommandImplementation(TypeChecked);
        public ICommand AddProductCommand => new AnotherCommandImplementation(AddProductAsync);

        #endregion

        #region 属性

        public ObservableCollection<ProductType> _types;
        /// <summary>
        /// 商品类别列表
        /// </summary>
        public ObservableCollection<ProductType> Types { get => _types; set => this.MutateVerbose(ref _types, value, RaisePropertyChanged()); }

        public ObservableCollection<Product> _products;
        /// <summary>
        /// 商品列表
        /// </summary>
        public ObservableCollection<Product> Products { get => _products; set => this.MutateVerbose(ref _products, value, RaisePropertyChanged()); }

        public ObservableCollection<Product> _filterProducts;
        /// <summary>
        /// 筛选后的商品列表
        /// </summary>
        public ObservableCollection<Product> FilterProducts { get => _filterProducts; set => this.MutateVerbose(ref _filterProducts, value, RaisePropertyChanged()); }

        public bool _isAllProduct;
        /// <summary>
        /// 是否显示全部商品
        /// </summary>
        public bool IsAllProduct { get => _isAllProduct; set => this.MutateVerbose(ref _isAllProduct, value, RaisePropertyChanged()); }

        private TangOrder _order;
        /// <summary>
        /// 订单
        /// </summary>
        public TangOrder Order { get => _order; set => this.MutateVerbose(ref _order, value, RaisePropertyChanged()); }


        #endregion



        #region 界面绑定方法

        private void Loaded(object o)
        {
            //MaterialDesignThemes.Wpf.Transitions.TransitionerSlide dd = new MaterialDesignThemes.Wpf.Transitions.TransitionerSlide();
            //dd.OpeningEffect = new MaterialDesignThemes.Wpf.Transitions.TransitionEffect(MaterialDesignThemes.Wpf.Transitions.TransitionEffectKind.ExpandIn)
            var ctrl = (FastFood)o;
            txtKey = ctrl.txtProductkey;
            scrollorderProduct = ctrl.productScroll;
            LoadOrderAsync();
        }

        private void AllType(object o)
        {
            if (IsAllProduct) return;
            Types?.ForEach(a => a.IsCheck = false);
            IsAllProduct = true;
            Filter();
        }

        private void TypeChecked(object o)
        {
            var type = (ProductType)o;
            Types?.ForEach(a => a.IsCheck = false);
            IsAllProduct = false;
            type.IsCheck = true;
            Filter();
        }

        private void AddProductAsync(object o)
        {
            var product = (Product)o;
            if (product.Stock == 0) return;
            var format = product.Formats.First();
            var orderProduct = Order.TangOrderProducts.FirstOrDefault(a => a.ProductId == product.Id && (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);
            var exist = orderProduct != null;
            if (!exist || format.Id != orderProduct.FormatId)
            {
                orderProduct = new TangOrderProduct
                {
                    Feature = product.Feature,
                    FormatId = format.Id,
                    Name = product.Name,
                    OrderObjectId = Order.ObjectId,
                    Price = product.Formats.FirstOrDefault()?.Price ?? 0,
                    OriginalPrice = product.Formats.FirstOrDefault()?.Price ?? 0,
                    Discount = 10,
                    ProductId = product.Id,
                    ProductIdSet = product.ProductIdSet,
                    ProductStatus = TangOrderProductStatus.Order,
                    Status = EntityStatus.Normal
                };
                Order.TangOrderProducts.Add(orderProduct);
            }
            orderProduct.Quantity++;
            orderProduct.Amount = orderProduct.Quantity * orderProduct.Price;
            product.SelectedQuantity++;
            CalcOrderAmount();
            if (!exist)     // 如果是新增的产品，则滚动条设置滚动到最后
            {
                scrollorderProduct.ScrollToVerticalOffset(double.MaxValue);
            }
            // 保存订单与商品
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(Order);
                service.SaveOrderProduct(orderProduct);
            }

        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 商品筛选
        /// </summary>
        private void Filter()
        {
            var key = txtKey.Text?.Trim();
            var type = Types.FirstOrDefault(a => a.IsCheck);
            var products = ApplicationObject.App.Products.Where(a => (a.Scope & ActionScope.Store) > 0);
            if (type != null)
            {
                products = products.Where(a => a.ProductTypeId == type.Id);
            }
            if (!string.IsNullOrEmpty(key))
            {
                products = products.Where(a => a.DisplayName.Contains(key) || a.Pinyin.Contains(key) || a.FirstLetter.Contains(key));
            }
            FilterProducts = products.ToObservable();
        }
        /// <summary>
        /// 设置商品库存
        /// </summary>
        /// <param name="stock"></param>
        private void SetStock(ProductStockModel stock)
        {
            var product = Products.FirstOrDefault(a => a.Id == stock.ProductId);
            if (product == null) return;
            product.Stock = stock.Stock;
        }
        /// <summary>
        /// 计算订单金额
        /// </summary>
        private void CalcOrderAmount()
        {
            Order.TangOrderProducts?.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(a =>
            {
                Order.Amount += a.Amount;
                Order.OriginalAmount += a.OriginalPrice * a.Quantity;
            });

            Order.Amount = Math.Round(Order.Amount, 2);
            Order.OriginalAmount = Math.Round(Order.OriginalAmount, 2);
            Order.ProductQuantity = Order.TangOrderProducts?.Count(a => a.ProductStatus != TangOrderProductStatus.Return) ?? 0;
        }
        /// <summary>
        /// 载入订单
        /// </summary>
        private async void LoadOrderAsync()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var order = await service.GetFastOrderAsync();
                if(order == null || order.CreateTime < DateTime.Now.Date)
                {
                    order = await CreateOrderAsync();
                }
                Order = order;
            }
        }
        /// <summary>
        /// 创建新订单
        /// </summary>
        /// <returns></returns>
        private async Task<TangOrder> CreateOrderAsync()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var order = new TangOrder
                {
                    OrderStatus = TangOrderStatus.Ordering,
                    OrderSource = OrderSource.Cashier,
                    OrderMode = OrderCategory.FastFood,
                    StaffId = ApplicationObject.App.Staff.Id,
                    StaffName = ApplicationObject.App.Staff.Name,
                    StaffObjectId = ApplicationObject.App.Staff.ObjectId,
                    BusinessId = ApplicationObject.App.Business.Id,
                    TangOrderProducts = new ObservableCollection<TangOrderProduct>()
                };
                await service.SaveFastOrderAsync(order);
                return order;
            }
        }

        /// <summary>
        /// 订阅系统消息
        /// </summary>
        private void SubscribeSystemMessage()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                scope.Resolve<IOrderService>().Subscribe("SystemMessage", (channel, msg) =>
                {
                    var message = msg.ToString().Split('|');
                    switch (message[0])
                    {
                        case "StockChange":
                            SetStock(message[1].ToObject<ProductStockModel>());
                            break;
                        default:
                            Mainthread.BeginInvoke((Action)delegate ()
                            {
                                Filter();
                            });
                            break;
                    }
                });
            }
        }

        #endregion
    }
}