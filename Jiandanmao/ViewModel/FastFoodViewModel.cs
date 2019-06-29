using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Enum;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
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
        public FastFoodViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            this.SnackbarMessageQueue = snackbarMessageQueue;
            Types = ApplicationObject.App.Types;
            Products = ApplicationObject.App.Products;
            SubscribeSystemMessage();
            CreateOrder();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                // 支付类别
                PaymentTypes = service.GetAll<PaymentType>()?.Select(a => new SelectItem<PaymentType>(false, a.Name, a)).ToObservable();
                // 系统备注
                var marks = service.GetAll<SystemMark>();
                if (marks != null)
                {
                    // 订单备注
                    OrderRemarks = marks.Where(a => a.Category == MarkCategory.OrderMark).Select(a => a.Name).ToObservable();
                    // 支付备注
                    PayRemarks = marks.Where(a => a.Category == MarkCategory.PayRemark).Select(a => a.Name).ToObservable();
                }
            }
        }

        private TextBox txtKey;
        private ScrollViewer scroller;
        private Transitioner transitioner;

        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AllTypeCommand => new AnotherCommandImplementation(AllType);
        public ICommand TypeCheckedCommand => new AnotherCommandImplementation(TypeChecked);
        public ICommand AddProductCommand => new AnotherCommandImplementation(AddProduct);
        public ICommand ClearCommand => new AnotherCommandImplementation(Clear);
        public ICommand HoogupCommand => new AnotherCommandImplementation(HoogupAsync);
        public ICommand PayCommand => new AnotherCommandImplementation(Pay);
        public ICommand PreferentialChangedCommand => new AnotherCommandImplementation(PreferentialChanged);
        public ICommand DiscountChangedCommand => new AnotherCommandImplementation(DiscountChanged);
        public ICommand ReceivedChangedCommand => new AnotherCommandImplementation(ReceivedChanged);
        public ICommand SubmitPaymentCommand => new AnotherCommandImplementation(SubmitPaymentAsync);
        public ICommand BackCommand => new AnotherCommandImplementation(Back);
        public ICommand ClickPaymentCommand => new AnotherCommandImplementation(ClickPayment);
        public ICommand MixPayCommand => new AnotherCommandImplementation(MixPayAsync);
        public ICommand PickOrderCommand => new AnotherCommandImplementation(PickOrderAsync);
        public ICommand ProductSearchCommand => new AnotherCommandImplementation(ProductSearch);
        public ICommand EnterToAddCommand => new AnotherCommandImplementation(EnterToAdd);
        public ICommand EscToClearCommand => new AnotherCommandImplementation(EscToClear);
        public ICommand NumToAddCommand => new AnotherCommandImplementation(NumToAdd);
        public ICommand ReduceCommand => new AnotherCommandImplementation(Reduce);
        public ICommand IncreaseCommand => new AnotherCommandImplementation(Increase);
        public ICommand ProductClickCommand => new AnotherCommandImplementation(ProductClickAsync);
        public ICommand ClearProductKeyCommand => new AnotherCommandImplementation(ClearProductKey);

        #endregion

        #region 属性

        #region 订单页属性

        private TangOrder _order;
        /// <summary>
        /// 订单
        /// </summary>
        public TangOrder Order { get => _order; set => this.MutateVerbose(ref _order, value, RaisePropertyChanged()); }

        #endregion

        #region 菜单页属性

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

        private ListObject<Product> _pager = new ListObject<Product>(20);
        /// <summary>
        /// 分页对象
        /// </summary>
        public ListObject<Product> Pager { get => _pager; set => this.MutateVerbose(ref _pager, value, RaisePropertyChanged()); }

        //public ObservableCollection<Product> _filterProducts;
        ///// <summary>
        ///// 筛选后的商品列表
        ///// </summary>
        //public ObservableCollection<Product> FilterProducts { get => _filterProducts; set => this.MutateVerbose(ref _filterProducts, value, RaisePropertyChanged()); }

        public bool _isAllProduct;
        /// <summary>
        /// 是否显示全部商品
        /// </summary>
        public bool IsAllProduct { get => _isAllProduct; set => this.MutateVerbose(ref _isAllProduct, value, RaisePropertyChanged()); }

        public ObservableCollection<string> _orderRemarks;
        /// <summary>
        /// 订单备注
        /// </summary>
        public ObservableCollection<string> OrderRemarks { get => _orderRemarks; set => this.MutateVerbose(ref _orderRemarks, value, RaisePropertyChanged()); }

        #endregion

        #region 支付页属性

        private double _orderAmount;
        /// <summary>
        /// 订单金额
        /// </summary>
        public double OrderAmount { get => _orderAmount; set => this.MutateVerbose(ref _orderAmount, value, RaisePropertyChanged()); }

        private double _preferentialAmount;
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double PreferentialAmount
        {
            get => _preferentialAmount; set
            {

                this.MutateVerbose(ref _preferentialAmount, value, RaisePropertyChanged());
            }
        }

        private double _orderDiscount;
        /// <summary>
        /// 订单折扣
        /// </summary>
        public double OrderDiscount { get => _orderDiscount; set => this.MutateVerbose(ref _orderDiscount, value, RaisePropertyChanged()); }

        private double _actualAmount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public double ActualAmount { get => _actualAmount; set => this.MutateVerbose(ref _actualAmount, value, RaisePropertyChanged()); }

        private double _receivedAmount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public double ReceivedAmount { get => _receivedAmount; set => this.MutateVerbose(ref _receivedAmount, value, RaisePropertyChanged()); }

        private double _giveAmount;
        /// <summary>
        /// 找零
        /// </summary>
        public double GiveAmount { get => _giveAmount; set => this.MutateVerbose(ref _giveAmount, value, RaisePropertyChanged()); }

        private string _raymentRemark;
        /// <summary>
        /// 支付备注
        /// </summary>
        public string PaymentRemark { get => _raymentRemark; set => this.MutateVerbose(ref _raymentRemark, value, RaisePropertyChanged()); }

        public ObservableCollection<SelectItem<PaymentType>> _paymentTypes;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        public ObservableCollection<SelectItem<PaymentType>> PaymentTypes { get => _paymentTypes; set => this.MutateVerbose(ref _paymentTypes, value, RaisePropertyChanged()); }

        public ObservableCollection<string> _payRemarks;
        /// <summary>
        /// 支付备注
        /// </summary>
        public ObservableCollection<string> PayRemarks { get => _payRemarks; set => this.MutateVerbose(ref _payRemarks, value, RaisePropertyChanged()); }

        #endregion

        #endregion



        #region 界面绑定方法

        private void Loaded(object o)
        {
            //MaterialDesignThemes.Wpf.Transitions.TransitionerSlide dd = new MaterialDesignThemes.Wpf.Transitions.TransitionerSlide();
            //dd.OpeningEffect = new MaterialDesignThemes.Wpf.Transitions.TransitionEffect(MaterialDesignThemes.Wpf.Transitions.TransitionEffectKind.ExpandIn)
            var ctrl = (FastFood)o;
            txtKey = ctrl.txtProductkey;
            scroller = ctrl.productScroll;
            transitioner = ctrl.transitioner;
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

        private void AddProduct(object o)
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
                    Price = format.Price,
                    OriginalPrice = format.Price,
                    Discount = 10,
                    ProductId = product.Id,
                    ProductIdSet = product.ProductIdSet,
                    ProductStatus = TangOrderProductStatus.Order
                };
                Order.TangOrderProducts.Add(orderProduct);
            }
            orderProduct.Quantity++;
            orderProduct.Amount = orderProduct.Quantity * orderProduct.Price;
            product.SelectedQuantity++;
            CalcOrderAmount();
            if (!exist)     // 如果是新增的产品，则滚动条设置滚动到最后
            {
                scroller.ScrollToVerticalOffset(double.MaxValue);
            }
        }

        private void Clear(object o)
        {
            CreateOrder();
        }

        private async void HoogupAsync(object o)
        {
            if (Order.ProductQuantity == 0)
            {
                SnackbarTips("请选择商品后再挂单");
                return;
            }
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.SaveFastOrder(Order);
                await service.HoogupOrderAsync(Order);
            }
            CreateOrder();
            Back(null);
        }

        private void Pay(object o)
        {
            if (Order.ProductQuantity == 0)
            {
                SnackbarTips("请选择商品后再收款");
                return;
            }
            if (transitioner.SelectedIndex == 1) return;
            transitioner.SelectedIndex = 1;
            OrderDiscount = 10;
            PreferentialAmount = 0;
            CalcPayment();
        }

        private void PreferentialChanged(object o)
        {
            var txt = (TextBox)o;
            PreferentialAmount = GetTextBoxNumber((TextBox)o);
            CalcPayment();
        }

        private void DiscountChanged(object o)
        {
            var txt = (TextBox)o;
            OrderDiscount = GetTextBoxNumber((TextBox)o);
            CalcPayment();
        }

        private void ReceivedChanged(object o)
        {
            var txt = (TextBox)o;
            ReceivedAmount = GetTextBoxNumber((TextBox)o);
            GiveAmount = Math.Round(ReceivedAmount - ActualAmount, 2);
        }

        private async void SubmitPaymentAsync(object o)
        {
            var paymentItem = PaymentTypes.FirstOrDefault(a => a.IsSelected);
            var payment = paymentItem?.Target;
            if (payment == null)
            {
                MessageTips("请选择收款方式！");
                return;
            }
            if (GiveAmount < 0)
            {
                MessageTips("找赎金额不能小于零！");
                return;
            }
            if(ActualAmount < 0)
            {
                MessageTips("实收金额不能小于零！");
                return;
            }
            await Confirm("确定收款吗？");
            if (!IsConfirm) return;
            Order.TangOrderPayments = new List<TangOrderPayment> { new TangOrderPayment { Amount = ActualAmount, Name = payment.Name, PaymentTypeId = payment.Id, PaymentTypeObjectId = payment.ObjectId } };
            await FinishOrderAsync();
        }

        private void Back(object o)
        {
            transitioner.SelectedIndex = 0;
            PaymentTypes?.ForEach(a => a.IsSelected = false);
        }

        private void ClickPayment(object o)
        {
            var selectedItem = (SelectItem)o;
            PaymentTypes.ForEach(a => a.IsSelected = false);
            selectedItem.IsSelected = !selectedItem.IsSelected;
        }

        private async void MixPayAsync(object o)
        {
            Order.ActualAmount = ActualAmount;
            var vm = new MixPaymentViewModel(Order);
            await DialogHost.Show(new MixPayment { DataContext = vm });
            if (!vm.IsConfirm) return;
            Order.TangOrderPayments = vm.Payments.ToList();
            await FinishOrderAsync();
        }

        private async void PickOrderAsync(object o)
        {
            var vm = new FastFoodHoogupViewModel();
            await DialogHost.Show(new FastFoodHoogup { DataContext = vm }, "RootDialog");
            if (vm.SelectedOrder == null) return;
            Order = vm.SelectedOrder;
            Products.ForEach(product =>
            {
                product.SelectedQuantity = Order.TangOrderProducts.Where(b => b.ProductId == product.Id).Sum(b => b.Quantity);
            });
            CalcPayment();
        }

        private void ProductSearch(object o)
        {
            IsAllProduct = true;
            Types.ForEach(a => a.IsCheck = false);
            Filter();
        }

        private void EnterToAdd(object o)
        {
            if (Pager.List.Count != 1) return;
            AddProduct(Pager.List[0]);
        }

        private void EscToClear(object o)
        {
            var tb = (TextBox)o;
            tb.Text = string.Empty;
        }

        private void NumToAdd(object o)
        {
            var num = Convert.ToInt32(o);
            if (Pager.List.Count <= num - 1) return;
            AddProduct(Pager.List[num - 1]);
        }

        private void Reduce(object o)
        {
            TangOrderProduct product = null;
            if (o is TangOrderProduct)
            {
                product = (TangOrderProduct)o;
            }
            if (product == null || product.Quantity <= 0) return;
            product.Quantity--;
            if (product.Quantity == 0)
            {
                Order.TangOrderProducts.Remove(product);
            }
            var good = Products.FirstOrDefault(a => a.Id == product.ProductId);
            good.SelectedQuantity--;
            product.Amount = product.Quantity * product.Price;
            CalcOrderAmount();
        }

        private void Increase(object o)
        {
            TangOrderProduct product = null;
            if (o is TangOrderProduct)
            {
                product = (TangOrderProduct)o;
            }
            if (product == null) return;
            product.Quantity++;
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == product.ProductId);
            good.SelectedQuantity++;
            product.Amount = product.Quantity * product.Price;
            CalcOrderAmount();
        }

        private async void ProductClickAsync(object o)
        {
            var orderProduct = (TangOrderProduct)o;
            if (orderProduct.ProductStatus == TangOrderProductStatus.Return) return;
            Order.TangOrderProducts.ForEach(a => a.IsSelected = false);
            orderProduct.IsSelected = true;
            var viewModel = new ChineseFoodDetailViewModel(Order, orderProduct, ApplicationObject.App.Products.First(a => a.Id == orderProduct.ProductId));
            var detail = new ChineseFoodDetail { DataContext = viewModel };

            await DialogHost.Show(detail, "RootDialog");
            if (viewModel.IsSubmit)
            {
                CalcOrderAmount();
                CalcPayment();
            }
        }

        private void ClearProductKey(object o)
        {
            var txt = (TextBox)o;
            txt.Text = string.Empty;
            txt.Focus();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建订单
        /// </summary>
        private void CreateOrder()
        {
            Order = new TangOrder {
                OrderDiscount = 10,
                OrderStatus = TangOrderStatus.Ordering,
                OrderSource = OrderSource.Cashier,
                OrderMode = OrderCategory.FastFood,
                StaffId = ApplicationObject.App.Staff.Id,
                StaffObjectId = ApplicationObject.App.Staff.ObjectId,
                StaffName = ApplicationObject.App.Staff.Name,
                BusinessId = ApplicationObject.App.Business.Id,
                TangOrderProducts = new ObservableCollection<TangOrderProduct>()
            };
            Products.ForEach(product => product.SelectedQuantity = 0);
        }
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
            //FilterProducts = products.ToObservable();
            Pager.OriginalList = products.ToObservable();
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
            var amount = 0d;
            var originalAmount = 0d;
            Order.TangOrderProducts?.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(a =>
            {
                amount += a.Amount;
                originalAmount += a.OriginalPrice * a.Quantity;
            });

            Order.Amount = Math.Round(amount, 2);
            Order.OriginalAmount = Math.Round(originalAmount, 2);
            Order.ProductQuantity = Order.TangOrderProducts?.Count(a => a.ProductStatus != TangOrderProductStatus.Return) ?? 0;
        }
        private async Task FinishOrderAsync()
        {
            // 库存验证
            var stocks = ValidateStock(Order.TangOrderProducts, out bool isSuccess);
            if (!isSuccess) return;
            // 支付订单
            Order.PayTime = DateTime.Now;
            Order.ActualAmount = ActualAmount;
            Order.PaymentRemark = PaymentRemark;
            Order.PreferentialAmount = PreferentialAmount;
            Order.OrderDiscount = OrderDiscount;
            Order.StaffId = ApplicationObject.App.Staff.Id;
            Order.CashierName = ApplicationObject.App.ClientData.Name;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                await service.PaymentFastAsync(Order);
            }
            // 保存库存
            await SetStockAsync(stocks);
            // 打印订单
            PrintOrder(new PrintOption
            {
                Title = "订单",
                Order = Order,
                Type = 0,
                Mode = PrintMode.Payment,
                Products = Order.TangOrderProducts
            });
            CreateOrder();
            Back(null);
            SnackbarTips("收款成功");
        }
        /// <summary>
        /// 付款时，验证商品库存
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        private List<ProductStockModel> ValidateStock(IEnumerable<TangOrderProduct> list, out bool isSuccess)
        {
            var goods = list.GroupBy(a => a.ProductId).Select(a => new { Id = a.Key, Quantity = a.Sum(b => b.Quantity) }).ToList();
            var stocks = new List<ProductStockModel>();

            foreach (var good in goods)
            {
                var product = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == good.Id && a.Stock != -1);
                if (product == null) continue;
                if (product.Stock < good.Quantity)
                {
                    DialogHost.Show(new MessageDialog { Message = { Text = $"商品【{product.Name}】数量不足" } });
                    isSuccess = false;
                    return null;
                }
                stocks.Add(new ProductStockModel { ProductId = product.Id, Stock = product.Stock - good.Quantity });
            }
            isSuccess = true;
            return stocks;
        }
        /// <summary>
        /// 设置商品库存
        /// </summary>
        /// <param name="stocks"></param>
        private async Task SetStockAsync(IEnumerable<ProductStockModel> stocks)
        {
            if (stocks == null || stocks.Count() == 0) return;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var util = scope.Resolve<IUtilService>();
                await util.SetProductStocksAsync(stocks);
                stocks.ForEach(stock => util.PubSubscribe("SystemMessage", $"StockChange|{stock.ToJson()}"));
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
        /// <summary>
        /// 获取输入框中的数字
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="property"></param>
        private double GetTextBoxNumber(TextBox txt)
        {
            var val = 0d;
            if (string.IsNullOrEmpty(txt.Text))
            {
                val = 0;
                txt.Text = "0";
            }
            else
            {
                val = double.Parse(txt.Text);
            }
            Task.Run(() =>
            {
                Thread.Sleep(100);
                Mainthread.BeginInvoke((Action)delegate ()
                {
                    txt.CaretIndex = int.MaxValue;
                });
            });
            return val;
        }
        /// <summary>
        /// 计算支付
        /// </summary>
        private void CalcPayment()
        {
            OrderAmount = Order.Amount;
            var discountProductAmount = 0d;
            Order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(item =>
            {
                var product = Products?.FirstOrDefault(b => b.Id == item.ProductId);
                if (product == null || !product.IsDiscount) return;
                discountProductAmount += item.Amount;
            });
            var discountAmount = discountProductAmount * (10 - OrderDiscount) / 10;

            var amount = OrderAmount - discountAmount - PreferentialAmount;
            var total = Math.Round(amount, 2);

            ActualAmount = ReceivedAmount = total;
            GiveAmount = 0;
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        private void PrintOrder(PrintOption option)
        {
            var order = option.Order;
            order.CashierName = ApplicationObject.App.ClientData.Name;
            if (option.Products == null || option.Products.Count() == 0) return;
            // 加载订单产品套餐中的商品
            var products = ApplicationObject.App.Products;
            option.Products.ForEach(item =>
            {
                if (item.Tag != null || item.Feature != ProductFeature.SetMeal) return;
                var ids = item.ProductIdSet.Split(',').Select(a => int.Parse(a));
                item.Tag = products.Where(a => ids.Contains(a.Id)).ToList();
            });

            PubPrint(option);
        }
        private string orderPrintChannel = "orderPrintChannel";
        /// <summary>
        /// 订阅打印消息
        /// </summary>
        private void SubscribePrint()
        {
            var env = ApplicationObject.App.DataBase.BeginLifetimeScope();
            env.Resolve<IUtilService>().Subscribe(orderPrintChannel, (channel, msg) =>
            {
                var option = JsonConvert.DeserializeObject<PrintOption>(msg.ToString());
                Mainthread.InvokeAsync(() =>
                {
                    try
                    {
                        ApplicationObject.Print(option.Order, option.Type, option);
                    }
                    catch (Exception ex)
                    {
                        SnackbarTips(ex.Message);
                    }
                });

            });
            env.Dispose();
        }
        /// <summary>
        /// 发布打印消息
        /// </summary>
        /// <param name="msg"></param>
        private void PubPrint(object obj)
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.PubSubscribe(orderPrintChannel, JsonConvert.SerializeObject(obj));
            }
        }

        #endregion
    }
}