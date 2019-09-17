using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using MaterialDesignThemes.Wpf.Transitions;

using Autofac;
using Jiandanmao.Enum;
using Jiandanmao.Helper;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.IService;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JdCat.CatClient.Common;
using System.ComponentModel;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// 中餐厅点单vm
    /// </summary>
    public class ChineseFoodViewModel : BaseViewModel
    {
        public ChineseFoodViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            SnackbarMessageQueue = snackbarMessageQueue;
        }
        private ChineseFood ThisController;
        private ChineseFoodOrder OrderController;
        private TextBox txtPreferential;    // 优惠金额输入框
        private TextBox txtDiscount;        // 折扣金额输入框
        private TextBox txtProductKey;      // 商品搜索输入框

        private double mealFee;             // 餐位费
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand DeskLoadedCommand => new AnotherCommandImplementation(DeskLoaded);
        public ICommand OrderLoadedCommand => new AnotherCommandImplementation(OrderLoaded);
        public ICommand AllDeskCommand => new AnotherCommandImplementation(AllDesk);
        public ICommand ClickTypeCommand => new AnotherCommandImplementation(ClickType);
        public ICommand SearchDeskCommand => new AnotherCommandImplementation(SearchDesk);
        public ICommand OpenDeskCommand => new AnotherCommandImplementation(OpenDesk);
        public ICommand SubmitNumberCommand => new AnotherCommandImplementation(SubmitNumber);
        public ICommand TypePreCommand => new AnotherCommandImplementation(TypePre);
        public ICommand TypeNextCommand => new AnotherCommandImplementation(TypeNext);
        public ICommand AllProductsCommand => new AnotherCommandImplementation(AllProducts);
        public ICommand ClickProductTypeCommand => new AnotherCommandImplementation(ClickProductType);
        public ICommand AddProductCommand => new AnotherCommandImplementation(AddProduct);
        public ICommand ReduceCommand => new AnotherCommandImplementation(Reduce);
        public ICommand IncreaseCommand => new AnotherCommandImplementation(Increase);
        public ICommand ProductClickCommand => new AnotherCommandImplementation(ProductClick);
        public ICommand BackDeskCommand => new AnotherCommandImplementation(BackDesk);
        public ICommand ProductSearchCommand => new AnotherCommandImplementation(ProductSearch);
        public ICommand SubmitOrderCommand => new AnotherCommandImplementation(SubmitOrder);
        public ICommand DeleteOrderCommand => new AnotherCommandImplementation(DeleteOrder);
        public ICommand ReAddProductCommand => new AnotherCommandImplementation(ReAddProduct);
        public ICommand SubmitReAddProductCommand => new AnotherCommandImplementation(SubmitReAddProduct);
        public ICommand ClearProductKeyCommand => new AnotherCommandImplementation(ClearProductKey);
        public ICommand PaymentLoadedCommand => new AnotherCommandImplementation(PaymentLoaded);
        public ICommand PayCommand => new AnotherCommandImplementation(Pay);
        public ICommand CheckPaymentCommand => new AnotherCommandImplementation(CheckPayment);
        public ICommand SubmitPaymentCommand => new AnotherCommandImplementation(SubmitPayment);
        public ICommand RemarkChangeCommand => new AnotherCommandImplementation(RemarkChange);
        public ICommand PrePrintCommand => new AnotherCommandImplementation(PrePrint);
        public ICommand DeskStatusChangedCommand => new AnotherCommandImplementation(DeskStatusChanged);
        public ICommand RefreshDeskCommand => new AnotherCommandImplementation(RefreshDesk);
        public ICommand OrderCatCommand => new AnotherCommandImplementation(OrderCat);
        public ICommand EditPeopleNumberCommand => new AnotherCommandImplementation(EditPeopleNumber);
        public ICommand EnterToAddCommand => new AnotherCommandImplementation(EnterToAdd);
        public ICommand EscToClearCommand => new AnotherCommandImplementation(EscToClear);
        public ICommand NumToAddCommand => new AnotherCommandImplementation(NumToAdd);
        public ICommand PreferentialChangedCommand => new AnotherCommandImplementation(PreferentialChanged);
        public ICommand DiscountChangedCommand => new AnotherCommandImplementation(DiscountChanged);
        public ICommand ReceivedChangedCommand => new AnotherCommandImplementation(ReceivedChanged);
        public ICommand MixPayCommand => new AnotherCommandImplementation(MixPayAsync);
        public ICommand ChangeDeskCommand => new AnotherCommandImplementation(ChangeDeskAsync);
        public ICommand FenOrderCommand => new AnotherCommandImplementation(FenOrderAsync);



        #region 属性声明
        private bool _isAll = true;
        /// <summary>
        /// 是否选中全部餐桌
        /// </summary>
        public bool IsAll { get => _isAll; set => this.MutateVerbose(ref _isAll, value, RaisePropertyChanged()); }
        private int _menuButton;
        /// <summary>
        /// 菜单按钮类别
        /// </summary>
        public int MenuButton { get => _menuButton; set => this.MutateVerbose(ref _menuButton, value, RaisePropertyChanged()); }


        #region 餐台页对象

        /// <summary>
        /// 餐桌类别
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; set; } = ApplicationObject.App.DeskTypes;
        private ObservableCollection<Desk> _desks;
        /// <summary>
        /// 所有餐桌
        /// </summary>
        public ObservableCollection<Desk> Desks { get => _desks; set => this.MutateVerbose(ref _desks, value, RaisePropertyChanged()); }
        private Desk _selectedDesk;
        /// <summary>
        /// 已选中的餐桌
        /// </summary>
        public Desk SelectedDesk { get => _selectedDesk; set => this.MutateVerbose(ref _selectedDesk, value, RaisePropertyChanged()); }

        private int _deskCount;
        /// <summary>
        /// 所有餐桌的数量
        /// </summary>
        public int DeskCount { get => _deskCount; set => this.MutateVerbose(ref _deskCount, value, RaisePropertyChanged()); }
        private int _usingDeskCount;
        /// <summary>
        /// 使用中的餐桌数量
        /// </summary>
        public int UsingDeskCount { get => _usingDeskCount; set => this.MutateVerbose(ref _usingDeskCount, value, RaisePropertyChanged()); }
        private int _freeDeskCount;
        /// <summary>
        /// 空闲的餐桌数量
        /// </summary>
        public int FreeDeskCount { get => _freeDeskCount; set => this.MutateVerbose(ref _freeDeskCount, value, RaisePropertyChanged()); }

        private ObservableCollection<SelectItem<DeskStatus>> _deskStateButton = new ObservableCollection<SelectItem<DeskStatus>> {
            new SelectItem<DeskStatus>(true, "Focus", DeskStatus.None),
            new SelectItem<DeskStatus>(false, "", DeskStatus.Free),
            new SelectItem<DeskStatus>(false, "", DeskStatus.Using)
        };
        public ObservableCollection<SelectItem<DeskStatus>> DeskStateButton { get => _deskStateButton; set => this.MutateVerbose(ref _deskStateButton, value, RaisePropertyChanged()); }

        private string _deskKey;

        #endregion

        #region 菜单页对象

        private bool _isAllProduct = true;
        public bool IsAllProduct { get => _isAllProduct; set => this.MutateVerbose(ref _isAllProduct, value, RaisePropertyChanged()); }
        private ObservableCollection<ProductType> _productTypes;
        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set
            {
                var types = new ObservableCollection<ProductType>();
                value.ForEach(type =>
                {
                    if (type.Products.Count(a => (a.Scope & ActionScope.Store) > 0) == 0) return;
                    types.Add(type);
                });
                this.MutateVerbose(ref _productTypes, types, RaisePropertyChanged());
            }
        }
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products { get => _products; set => this.MutateVerbose(ref _products, value, RaisePropertyChanged()); }

        private ListObject<Product> _productObject = new ListObject<Product>(20);
        public ListObject<Product> ProductObject { get => _productObject; set => this.MutateVerbose(ref _productObject, value, RaisePropertyChanged()); }
        #endregion

        public ObservableCollection<string> _orderRemarks;
        /// <summary>
        /// 订单备注
        /// </summary>
        public ObservableCollection<string> OrderRemarks { get => _orderRemarks; set => this.MutateVerbose(ref _orderRemarks, value, RaisePropertyChanged()); }

        public ObservableCollection<string> _payRemarks;
        /// <summary>
        /// 支付备注
        /// </summary>
        public ObservableCollection<string> PayRemarks { get => _payRemarks; set => this.MutateVerbose(ref _payRemarks, value, RaisePropertyChanged()); }

        public ObservableCollection<SelectItem<PaymentType>> _paymentTypes;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        public ObservableCollection<SelectItem<PaymentType>> PaymentTypes { get => _paymentTypes; set => this.MutateVerbose(ref _paymentTypes, value, RaisePropertyChanged()); }

        #region 结算页对象

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
        /// <summary>
        /// 折扣金额
        /// </summary>
        public double DiscountAmount { get; set; }

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

        #endregion

        #endregion


        #region 界面绑定方法
        private bool isLoaded = false;
        /// <summary>
        /// 中餐页面加载完成事件
        /// </summary>
        /// <param name="o"></param>
        private void Loaded(object o)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                Init(o);
            }
        }
        /// <summary>
        /// 餐桌加载完成事件
        /// </summary>
        /// <param name="o"></param>
        private void DeskLoaded(object o)
        {

        }
        /// <summary>
        /// 订单页面加载完成事件
        /// </summary>
        /// <param name="o"></param>
        private void OrderLoaded(object o)
        {
            OrderController = (ChineseFoodOrder)o;
            txtProductKey = OrderController.txtProductkey;
        }
        /// <summary>
        /// 支付页面加载完成事件
        /// </summary>
        /// <param name="o"></param>
        private void PaymentLoaded(object o)
        {
            var control = (ChineseFoodPayment)o;
            txtPreferential = control.txtPreferential;
            txtDiscount = control.txtDiscount;
        }
        private async void Init(object o)
        {
            // 订阅系统消息
            SubscribeSystemMessage();
            // 订阅订单变更消息
            SubscribeChanged();
            if (ApplicationObject.App.ClientData.IsHost)
            {
                // 订阅打印订单消息
                SubscribePrint();
            }
            // 餐桌
            Desks = ApplicationObject.App.Desks;

            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                // 餐位费
                mealFee = service.GetMealFee();
                PaymentTypes = service.GetAll<PaymentType>()?.Select(a => new SelectItem<PaymentType>(false, a.Name, a)).ToObservable();
                // 订单备注
                var marks = await service.GetAllAsync<SystemMark>();
                if (marks != null)
                {
                    OrderRemarks = marks.Where(a => a.Category == MarkCategory.OrderMark).Select(a => a.Name).ToObservable();
                    PayRemarks = marks.Where(a => a.Category == MarkCategory.PayRemark).Select(a => a.Name).ToObservable();
                }
            }
            // 未完成订单
            ReloadDeskOrder();
            ThisController = (ChineseFood)o;
        }
        private void TypePre(object o)
        {
            var scroll = (ScrollViewer)o;
            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - 50);
        }
        private void TypeNext(object o)
        {
            var scroll = (ScrollViewer)o;
            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + 50);
        }
        private void AllDesk(object o)
        {
            if (IsAll) return;
            DeskTypes?.ForEach(a => a.IsCheck = false);
            IsAll = true;
            FilterDesk();
        }
        private void ClickType(object o)
        {
            var type = (DeskType)o;
            DeskTypes?.ForEach(a => a.IsCheck = false);
            IsAll = false;
            type.IsCheck = true;
            FilterDesk();
        }
        private void SearchDesk(object o)
        {
            _deskKey = ((TextBox)o).Text.Trim().ToLower();
            FilterDesk();
        }
        private void OpenDesk(object o)
        {
            SelectedDesk = (Desk)o;
            if (SelectedDesk.Order == null)
            {
                DialogHost.Show(new DeskNumber { DataContext = this });
                return;
            }
            EntryOrderPage();
        }
        private void SubmitNumber(object o)
        {
            var order = new TangOrder
            {
                PeopleNumber = Convert.ToInt32(o),
                BusinessId = ApplicationObject.App.Business.Id,
                DeskId = SelectedDesk.Id,
                DeskName = SelectedDesk.Name,
                OrderStatus = TangOrderStatus.Ordering,
                OrderSource = OrderSource.Cashier,
                OrderMode = OrderCategory.ChineseFood,
                StaffId = ApplicationObject.App.Staff?.Id ?? 0,
                Staff = ApplicationObject.App.Staff,
                StaffName = ApplicationObject.App.Staff?.Name,
                CreateTime = DateTime.Now
            };
            SelectedDesk.Order = order;
            CalcOrderAmount();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.SaveOrder(order);
            }
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Add });
            EntryOrderPage();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private void AllProducts(object o)
        {
            if (IsAllProduct) return;
            ProductTypes?.ForEach(a => a.IsCheck = false);
            IsAllProduct = true;
            FilterProducts();
        }
        private void ClickProductType(object o)
        {
            var type = (ProductType)o;
            ProductTypes?.ForEach(a => a.IsCheck = false);
            IsAllProduct = false;
            type.IsCheck = true;
            FilterProducts();
        }
        private void AddProduct(object o)
        {
            var product = (Product)o;
            if (product.Stock == 0) return;
            var order = SelectedDesk.Order;
            var format = product.Formats.First();
            if (order.TangOrderProducts == null) order.TangOrderProducts = new ObservableCollection<TangOrderProduct>();
            var orderProduct = order.TangOrderProducts?.FirstOrDefault(a => a.ProductId == product.Id && (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);
            var exist = orderProduct != null;
            if (!exist || format.Id != orderProduct.FormatId)
            {
                orderProduct = new TangOrderProduct
                {
                    Feature = product.Feature,
                    FormatId = format.Id,
                    Name = product.Name,
                    OrderObjectId = SelectedDesk.Order.ObjectId,
                    Price = product.Formats.FirstOrDefault()?.Price ?? 0,
                    OriginalPrice = product.Formats.FirstOrDefault()?.Price ?? 0,
                    Discount = 10,
                    ProductId = product.Id,
                    ProductIdSet = product.ProductIdSet,
                    ProductStatus = SelectedDesk.Order.OrderStatus == TangOrderStatus.Ordering ? TangOrderProductStatus.Order : TangOrderProductStatus.Add,
                    Status = EntityStatus.Normal
                };
                order.TangOrderProducts.Add(orderProduct);
                //LoadProductsForSetmeal(orderProduct);
            }
            orderProduct.Quantity++;
            orderProduct.Amount = orderProduct.Quantity * orderProduct.Price;
            product.SelectedQuantity++;
            CalcOrderAmount();
            if (!exist)     // 如果是新增的产品，则滚动条设置滚动到最后
            {
                OrderController.productScroll.ScrollToVerticalOffset(double.MaxValue);
            }

            SaveOrderProductChange(order, orderProduct);

            //using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            //{
            //    var service = scope.Resolve<IOrderService>();
            //    service.Update(order);
            //    service.SaveOrderProduct(orderProduct);
            //}
            //// 添加完新商品后，发布变更通知
            //PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private void Reduce(object o)
        {
            TangOrderProduct product = null;
            if (o is TangOrderProduct)
            {
                product = (TangOrderProduct)o;
            }
            if (product == null || product.Quantity <= 0) return;
            var order = SelectedDesk.Order;
            product.Quantity--;
            if (product.Quantity == 0)
            {
                SelectedDesk.Order.TangOrderProducts.Remove(product);
            }
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == product.ProductId);
            good.SelectedQuantity--;
            product.Amount = product.Quantity * product.Price;
            CalcOrderAmount();
            SaveOrderProductChange(order, product);
        }
        private void Increase(object o)
        {
            TangOrderProduct product = null;
            if (o is TangOrderProduct)
            {
                product = (TangOrderProduct)o;
            }
            if (product == null) return;
            var order = SelectedDesk.Order;
            product.Quantity++;
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == product.ProductId);
            good.SelectedQuantity++;
            product.Amount = product.Quantity * product.Price;
            CalcOrderAmount();
            SaveOrderProductChange(order, product);
        }
        private async void ProductClick(object o)
        {
            var order = SelectedDesk.Order;
            var orderProduct = (TangOrderProduct)o;
            if (orderProduct.ProductStatus == TangOrderProductStatus.Return) return;
            order.TangOrderProducts.ForEach(a => a.IsSelected = false);

            orderProduct.IsSelected = true;
            var viewModel = new ChineseFoodDetailViewModel(order, orderProduct, ApplicationObject.App.Products.First(a => a.Id == orderProduct.ProductId));
            var detail = new ChineseFoodDetail { DataContext = viewModel, Tag = Desks };

            await DialogHost.Show(detail, "RootDialog");
            if (viewModel.IsSubmit)
            {
                CalcOrderAmount();
                using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IOrderService>();
                    service.Update(order);
                }
                PrePay();
                PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
            }
        }
        private void BackDesk(object o = null)
        {
            ThisController.transitioner.SelectedIndex = 0;
            ResetDeskStatus();
        }
        private void ProductSearch(object o)
        {
            IsAllProduct = true;
            ProductTypes.ForEach(a => a.IsCheck = false);
            FilterProducts();
        }
        private async void SubmitOrder(object o)
        {
            var order = SelectedDesk.Order;
            if (order.TangOrderProducts == null || order.TangOrderProducts.Count == 0)
            {
                MessageTips("请点菜后再下单！");
                return;
            }
            await Confirm("确定下单吗？");
            if (IsConfirm)
            {
                await SubmitOrderAsync();
            }
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private async void DeleteOrder(object o)
        {
            var order = SelectedDesk.Order;
            if (order.OrderStatus != TangOrderStatus.Ordering) return;
            await Confirm("确定删除订单吗？");
            if (!IsConfirm) return;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.DeleteOrder(order);
            }
            SelectedDesk.Order = null;
            SnackbarTips("删除成功");
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Delete });
            BackDesk();
        }
        private void ReAddProduct(object o)
        {
            OrderController.transition2.SelectedIndex = 0;
            MenuButton = 2;
        }
        private async void SubmitReAddProduct(object o)
        {
            var order = SelectedDesk.Order;
            if (order.TangOrderProducts == null || order.TangOrderProducts.Count(a => a.ProductStatus == TangOrderProductStatus.Add) == 0)
            {
                MessageTips("请选中菜品后再确定加菜！");
                return;
            }
            await Confirm("确定加菜吗？");
            if (IsConfirm)
            {
                await ReSubmitOrderAsync();
            }
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private void ClearProductKey(object o)
        {
            var txt = (TextBox)o;
            txt.Text = string.Empty;
            txt.Focus();
        }
        private void Pay(object o)
        {
            var unOrderingGood = SelectedDesk.Order.TangOrderProducts?.Count(a => (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);   // 未下单的商品数
            if (unOrderingGood > 0)
            {
                MessageTips("订单中有未下单的商品，请处理后再结算！");
                return;
            }
            var transition = (Transitioner)o;
            transition.SelectedIndex = 1;
            MenuButton = 1;
            PrePay();
            //txtActual.Text = SelectedDesk.Order.Amount.ToString();
        }
        //private void AmountReset(object o)
        //{
        //    var txt = (TextBox)o;
        //    txt.Text = 0.ToString();
        //    txt.SelectAll();
        //    txt.Focus();
        //}
        private void CheckPayment(object o)
        {
            var selectedItem = (SelectItem)o;
            PaymentTypes.ForEach(a => a.IsSelected = false);
            selectedItem.IsSelected = !selectedItem.IsSelected;
        }
        //private void ClickMoney(object o)
        //{
        //    //var money = double.Parse(o.ToString());
        //    //var actual = double.Parse(txtActual.Text);
        //    //txtActual.Text = (money + actual).ToString();
        //    //txtActual.Text = o.ToString();
        //    //CalcBalance();
        //}
        //private void ActualChanged(object o)
        //{
        //    var txt = (TextBox)o;
        //    var text = txt.Text.Trim();
        //    if (string.IsNullOrEmpty(text)) txt.Text = text = "0";
        //    if (!double.TryParse(text, out double distance))
        //    {
        //        var reg = Regex.Match(text, @"\d+");
        //        txt.Text = reg.Value;
        //        txt.SelectionStart = int.MaxValue;
        //        return;
        //    }
        //    txt.Text = distance.ToString();
        //    if (txt == txtPreferential)
        //    {
        //        //txtActual.Text = (SelectedDesk.Order.Amount - double.Parse(text)).ToString();
        //    }
        //    txt.SelectionStart = int.MaxValue;
        //    CalcBalance();
        //}
        private async void SubmitPayment(object o)
        {
            var order = SelectedDesk.Order;

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
            await Confirm("确定收款吗？");
            if (!IsConfirm) return;
            order.TangOrderPayments = new ObservableCollection<TangOrderPayment> { new TangOrderPayment { Amount = ActualAmount, Name = payment.Name, OrderObjectId = order.ObjectId, PaymentTypeId = payment.Id, PaymentTypeObjectId = payment.ObjectId } };
            await FinishOrder();
        }
        private void RemarkChange(object o)
        {
            var order = (TangOrder)o;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(order);
            }
        }
        private void PrePrint(object o)
        {
            PrintOrder(new PrintOption
            {
                Title = "预结单",
                Order = SelectedDesk.Order,
                Type = 1,
                Mode = PrintMode.PreOrder,
                Products = SelectedDesk.Order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return)
            });
        }
        private void DeskStatusChanged(object o)
        {
            DeskStateButton.ForEach(a => { a.Content = ""; a.IsSelected = false; });
            var target = (SelectItem)o;
            target.Content = "Focus";
            target.IsSelected = true;
            FilterDesk();
        }
        private void RefreshDesk(object o)
        {
            ReloadDeskOrder();
        }
        private void OrderCat(object o)
        {
            var num = 1;
            var desk = (Desk)o;
            if (desk.Order.TangOrderProducts == null) return;
            foreach (var item in desk.Order.TangOrderProducts)
            {
                LoadProductsForSetmeal(item);
            }
            var products = desk.Order.TangOrderProducts.Select(a => new { Sort = num++, a.Name, a.Quantity, a.Amount, a.ProductStatus, Set = a.SetProducts });

            DialogHost.Show(new ChineseFoodOrderCat { DataContext = new { desk.Name, Products = products } }, "RootDialog");
        }
        private async void EditPeopleNumber(object o)
        {
            var dialog = new ModifyNumber { Number = SelectedDesk.Order.PeopleNumber };
            await DialogHost.Show(dialog, "RootDialog");
            if (!dialog.IsSubmit) return;
            SelectedDesk.Order.PeopleNumber = dialog.Number;
            CalcOrderAmount();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(SelectedDesk.Order);
            }
            PrePay();
        }
        private void EnterToAdd(object o)
        {
            if (ProductObject.OriginalList.Count != 1) return;
            AddProduct(ProductObject.OriginalList[0]);
            //var tb = (TextBox)o;
            //tb.Text = string.Empty;
        }
        private void EscToClear(object o)
        {
            var tb = (TextBox)o;
            tb.Text = string.Empty;
        }
        private void NumToAdd(object o)
        {
            var num = Convert.ToInt32(o);
            if (ProductObject.List.Count <= num - 1) return;
            AddProduct(ProductObject.List[num - 1]);
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
        private async void MixPayAsync(object o)
        {
            SelectedDesk.Order.ActualAmount = ActualAmount;
            var vm = new MixPaymentViewModel(SelectedDesk.Order);
            await DialogHost.Show(new MixPayment { DataContext = vm });
            if (!vm.IsConfirm) return;
            SelectedDesk.Order.TangOrderPayments = vm.Payments;
            await FinishOrder();
        }
        private async void ChangeDeskAsync(object o)
        {
            var vm = new ChineseFoodChangeDeskViewModel(Desks);
            await DialogHost.Show(new ChineseFoodChangeDesk { DataContext = vm });
            if (!vm.IsConfirm) return;
            // 切换餐台订单
            var deskChange = vm.DeskChange;
            var deskTarget = vm.DeskTarget;
            var order = deskChange.Order;
            order.DeskId = deskTarget.Id;
            order.DeskName = deskTarget.Name;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(order);
            }
            deskTarget.Order = deskChange.Order;
            deskChange.Order = null;
            PubSubscribe(new SubscribeObj { DeskId = deskChange.Id, Mode = SubscribeMode.Delete, Sign = ApplicationObject.App.ClientData.Sign });
            PubSubscribe(new SubscribeObj { DeskId = deskTarget.Id, Mode = SubscribeMode.Change, Sign = ApplicationObject.App.ClientData.Sign, OrderObjectId = order.ObjectId });
            // 打印转台单（只有一单一打的打印机才出单）
            var bufferArr = new List<byte[]> {
                PrinterCmdUtils.AlignCenter(),
                PrinterCmdUtils.FontSizeSetBig(2),
                "转台单".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FontSizeSetBig(1),
                PrinterCmdUtils.AlignLeft(),
                $"转台时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.SplitLine("-"),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FontSizeSetBig(2),
                $"原餐台：{deskChange.Name}".ToByte(),
                PrinterCmdUtils.NextLine(),
                $"目标餐台：{deskTarget.Name}".ToByte(),
                PrinterCmdUtils.NextLine(),
                $"订单流水：{order.Identifier}".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FeedPaperCutAll()
            };
            foreach (var printer in ApplicationObject.App.Printers.Where(a => a.Device.State == 1 && a.Device.Type == 2 && a.Device.Mode == 2 && (a.Device.Scope & ActionScope.Store) > 0))
            {
                printer.Print(bufferArr);
            }
        }

        private async void FenOrderAsync(object o)
        {
            var vm = new ChineseFoodFenOrderViewModel(Desks, SelectedDesk.Order, o as TangOrderProduct);
            await DialogHost.Show(new ChineseFoodFenOrder { DataContext = vm });
            if (!vm.IsConfirm) return;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                await service.FenOrderAsync(vm.Good, SelectedDesk.Order, vm.Desk.Order);
                CalcOrderAmount(SelectedDesk.Order);
                CalcOrderAmount(vm.Desk.Order);
                service.Update(SelectedDesk.Order);
                service.Update(vm.Desk.Order);
            }
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, Mode = SubscribeMode.Change, OrderObjectId = SelectedDesk.Order.ObjectId });
            PubSubscribe(new SubscribeObj { DeskId = vm.Desk.Id, Mode = SubscribeMode.Change, OrderObjectId = vm.Desk.Order.ObjectId });
            // 打印
            var bufferArr = new List<byte[]> {
                PrinterCmdUtils.AlignCenter(),
                PrinterCmdUtils.FontSizeSetBig(2),
                "分菜单".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FontSizeSetBig(1),
                PrinterCmdUtils.AlignLeft(),
                $"分菜时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.SplitLine("-"),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FontSizeSetBig(2),
                $"原餐台：{SelectedDesk.Name}".ToByte(),
                PrinterCmdUtils.NextLine(),
                $"目标餐台：{vm.Desk.Name}".ToByte(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FontSizeSetBig(2),
                $"菜品：{vm.Good.Name}".ToByte(),
                PrinterCmdUtils.FontSizeSetBig(1),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.NextLine(),
                PrinterCmdUtils.FeedPaperCutAll()
            };
            foreach (var printer in ApplicationObject.App.Printers.Where(a => a.Device.State == 1 && a.Device.Type == 2 && a.Device.Mode == 2 && (a.Device.Scope & ActionScope.Store) > 0))
            {
                printer.Print(bufferArr);
            }

        }


        #endregion

        #region 私有方法

        /// <summary>
        /// 计算订单总额
        /// </summary>
        private void CalcOrderAmount(TangOrder tangOrder = null)
        {
            var order = tangOrder ?? SelectedDesk.Order;
            order.MealFee = mealFee * order.PeopleNumber;
            order.OriginalAmount = order.Amount = order.MealFee;
            order.TangOrderProducts?.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(a =>
            {
                order.Amount += a.Amount;
                order.OriginalAmount += a.OriginalPrice * a.Quantity;
            });

            order.Amount = Math.Round(order.Amount, 2);
            order.OriginalAmount = Math.Round(order.OriginalAmount, 2);
            order.ProductQuantity = order.TangOrderProducts?.Count(a => a.ProductStatus != TangOrderProductStatus.Return) ?? 0;
        }
        /// <summary>
        /// 重新加载餐桌未完成的订单
        /// </summary>
        private void ReloadDeskOrder()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                ApplicationObject.App.Desks.ForEach(a => a.Order = null);
                var unfinish = service.GetUnfinishOrder()?.Where(a => a.BusinessId == ApplicationObject.App.Business.Id).ToList();
                unfinish?.ForEach(order =>
                {
                    var desk = ApplicationObject.App.Desks.FirstOrDefault(a => a.Id == order.DeskId);
                    if (desk == null) return;
                    desk.Order = order;
                });
                ResetDeskStatus();
            }
        }
        /// <summary>
        /// 下单
        /// </summary>
        private async Task SubmitOrderAsync()
        {
            var order = SelectedDesk.Order;
            // 库存验证
            var stocks = ValidateStock(order.TangOrderProducts, out bool isSuccess);
            if (!isSuccess) return;
            // 创建订单
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.SubmitOrder(order);
            }
            // 设置库存
            await SetStockAsync(stocks);
            // 打印订单
            PrintOrder(new PrintOption
            {
                Title = "预结单",
                Order = order,
                Mode = PrintMode.PreOrder,
                Products = order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return)
            });
            SnackbarTips("下单成功");
            BackDesk();
        }
        /// <summary>
        /// 加菜
        /// </summary>
        private async Task ReSubmitOrderAsync()
        {
            var order = SelectedDesk.Order;
            var products = order.TangOrderProducts.Where(a => a.ProductStatus == TangOrderProductStatus.Add).ToList();
            // 验证库存
            var stocks = ValidateStock(products, out bool isSuccess);
            if (!isSuccess) return;
            //保存订单
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.ReSubmitOrder(order);
            }
            // 保存库存
            await SetStockAsync(stocks);
            // 打印加菜单
            PrintOrder(new PrintOption
            {
                Title = "加菜单",
                Order = order,
                Type = 2,
                Mode = PrintMode.Add,
                Products = products
            });
            SnackbarTips("加菜成功");
            BackDesk();
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
        /// <summary>
        /// 结账
        /// </summary>
        private void PrePay()
        {
            var order = SelectedDesk.Order;
            OrderAmount = order.Amount;
            PreferentialAmount = 0;
            OrderDiscount = 10;
            ActualAmount = OrderAmount;
            ReceivedAmount = OrderAmount;
            GiveAmount = 0;
            PaymentRemark = string.Empty;
        }
        /// <summary>
        /// 计算支付
        /// </summary>
        private void CalcPayment()
        {
            /* 1. 首先计算订单中可打折商品的总价
             * 2. 计算折扣总额
             * 3. 计算实际收费
             * */
            var order = SelectedDesk.Order;
            var discountProductAmount = 0d;
            order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(item =>
            {
                var product = ApplicationObject.App.Products?.FirstOrDefault(b => b.Id == item.ProductId);
                if (product == null || !product.IsDiscount) return;
                discountProductAmount += item.Amount;
            });
            DiscountAmount = discountProductAmount * (10 - OrderDiscount) / 10;


            var amount = OrderAmount - DiscountAmount - PreferentialAmount;
            var total = Math.Round(amount, 2);

            ActualAmount = ReceivedAmount = total;
            GiveAmount = 0;
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
        /// 筛选餐桌
        /// </summary>
        private void FilterDesk()
        {
            IEnumerable<Desk> desks;
            if (ApplicationObject.App.Desks == null || ApplicationObject.App.Desks.Count == 0) return;
            if (IsAll)
            {
                desks = ApplicationObject.App.Desks;
            }
            else
            {
                var type = DeskTypes.FirstOrDefault(a => a.IsCheck);
                desks = type.Desks;
            }
            var status = DeskStateButton.FirstOrDefault(a => a.IsSelected);
            switch (status.Target)
            {
                case DeskStatus.None:
                    break;
                case DeskStatus.Using:
                    desks = desks.Where(a => a.Order != null);
                    break;
                case DeskStatus.Free:
                    desks = desks.Where(a => a.Order == null);
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(_deskKey))
            {
                desks = desks.Where(a => a.Name.Contains(_deskKey));
            }
            Desks = desks.ToObservable();
        }
        /// <summary>
        /// 重置餐桌状态
        /// </summary>
        private void ResetDeskStatus()
        {
            var desks = ApplicationObject.App.Desks;
            if (ProductTypes == null || ProductTypes.Count == 0) ProductTypes = ApplicationObject.App.Types;
            UsingDeskCount = desks.Count(a => a.Order != null);
            FreeDeskCount = desks.Count(a => a.Order == null);
            DeskCount = desks.Count();
        }
        /// <summary>
        /// 设置商品库存
        /// </summary>
        /// <param name="stock"></param>
        private void SetStock(ProductStockModel stock)
        {
            var product = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == stock.ProductId);
            if (product == null) return;
            product.Stock = stock.Stock;
        }
        /// <summary>
        /// 进入订单页面
        /// </summary>
        private void EntryOrderPage()
        {
            var order = SelectedDesk.Order;
            ThisController.transitioner.SelectedIndex = 1;              // 订单页
            ProductTypes.ForEach(a => a.IsCheck = false);
            IsAllProduct = true;
            txtProductKey.Text = string.Empty;
            FilterProducts();
            ProductObject.OriginalList?.ForEach(a => a.SelectedQuantity = 0);
            order.TangOrderProducts?.ForEach(item =>
            {
                var product = ProductObject.OriginalList?.FirstOrDefault(b => b.Id == item.ProductId);
                if (product == null) return;
                product.SelectedQuantity += item.Quantity;
            });
            if (order.OrderStatus == TangOrderStatus.Eating)
            {
                // 如果是正在用餐的状态，检查是否存在没有下单的商品，如果存在，则进入点菜模块
                var unOrderingGood = order.TangOrderProducts?.Count(a => (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);   // 未下单的商品数
                if (unOrderingGood == 0)
                {
                    OrderController.transition2.SelectedIndex = 1;      // 结账模块
                    MenuButton = 1;                                     // 结账功能区
                    PrePay();
                    return;
                }
                OrderController.transition2.SelectedIndex = 0;      // 点菜模块
                MenuButton = 2;                                     // 加菜功能区
                return;
            }
            // 如果不是用餐状态，则进入点餐模块
            OrderController.transition2.SelectedIndex = 0;
            MenuButton = 0;         // 点餐功能区
        }
        /// <summary>
        /// 商品筛选
        /// </summary>
        private void FilterProducts()
        {
            var key = txtProductKey.Text?.Trim();
            var type = ProductTypes.FirstOrDefault(a => a.IsCheck);
            var products = ApplicationObject.App.Products.Where(a => (a.Scope & ActionScope.Store) > 0);
            if (type != null)
            {
                products = products.Where(a => a.ProductTypeId == type.Id);
            }
            if (!string.IsNullOrEmpty(key))
            {
                products = products.Where(a => a.DisplayName.Contains(key) || a.Pinyin.Contains(key) || a.FirstLetter.Contains(key));
            }
            ProductObject.OriginalList = products.ToObservable();
        }
        private long _lastModifyTime;       // 最后修改时间戳
        /// <summary>
        /// 保存订单商品更改
        /// </summary>
        private void SaveOrderProductChange(TangOrder order, TangOrderProduct product)
        {
            _lastModifyTime = DateTime.Now.ToTimeStamp();
            Task.Run(() =>
            {
                Thread.Sleep(200);
                // 200毫秒后保存数据库
                Mainthread.BeginInvoke((Action)delegate ()
                {
                    var curTime = DateTime.Now.ToTimeStamp();
                    if (curTime - _lastModifyTime < 200) return;

                    using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                    {
                        var service = scope.Resolve<IOrderService>();
                        service.Update(order);
                        service.SaveOrderProduct(product);
                    }
                    PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
                });
            });
        }
        /// <summary>
        /// 下单时，验证商品库存
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
        /// 给套餐商品加载套餐上列表
        /// </summary>
        private void LoadProductsForSetmeal(TangOrderProduct good)
        {
            if (good.Feature != ProductFeature.SetMeal || good.SetProducts.Count > 0) return;
            var ids = good.ProductIdSet.Split(',').Select(a => int.Parse(a)).ToList();
            var products = ApplicationObject.App.Products.Where(a => ids.Contains(a.Id)).ToList();
            foreach (var product in products)
            {
                good.SetProducts.Add(product);
            }
        }
        private async Task FinishOrder()
        {
            var order = SelectedDesk.Order;
            if (!IsConfirm) return;
            order.PayTime = DateTime.Now;
            order.ActualAmount = ActualAmount;
            order.PaymentRemark = PaymentRemark;
            order.PreferentialAmount = PreferentialAmount;
            order.OrderDiscount = OrderDiscount;
            order.StaffId = ApplicationObject.App.Staff.Id;
            order.CashierName = ApplicationObject.App.ClientData.Name;
            order.TangOrderActivity = new ObservableCollection<TangOrderActivity>();
            if (PreferentialAmount > 0)
            {
                order.TangOrderActivity.Add(new TangOrderActivity
                {
                    Amount = PreferentialAmount,
                    Remark = "整单立减",
                    Type = OrderActivityType.OrderPreferential,
                    TangOrderObjectId = order.ObjectId
                });
            }
            if (DiscountAmount > 0)
            {
                order.TangOrderActivity.Add(new TangOrderActivity
                {
                    Amount = DiscountAmount,
                    Remark = "整单折扣",
                    Type = OrderActivityType.OrderDiscount,
                    TangOrderObjectId = order.ObjectId
                });
            }
            foreach (var item in order.TangOrderProducts)
            {
                if (item.OriginalPrice != item.Price)
                {
                    order.TangOrderActivity.Add(new TangOrderActivity
                    {
                        Amount = Math.Round(item.OriginalPrice - item.Price, 2),
                        Remark = item.Name + "折扣优惠",
                        Type = OrderActivityType.ProductDiscount,
                        TangOrderObjectId = order.ObjectId
                    });
                }
            }
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                await service.PaymentAsync(order);
            }
            SelectedDesk.Order = null;
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Finish });
            BackDesk();
            SnackbarTips("结算成功");
            PrintOrder(new PrintOption
            {
                Title = "结算单",
                Order = order,
                Type = 1,
                Mode = PrintMode.Payment,
                Products = order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return)
            });
        }


        private string orderChangeChannel = "orderChangeChannel";
        /// <summary>
        /// 订阅，每次订单状态改变时，收到消息
        /// </summary>
        private void SubscribeChanged()
        {
            var env = ApplicationObject.App.DataBase.BeginLifetimeScope();
            env.Resolve<IOrderService>().Subscribe(orderChangeChannel, (channel, msg) =>
            {
                var data = JsonConvert.DeserializeObject<SubscribeObj>(msg.ToString());
                if (data.Sign == ApplicationObject.App.ClientData.Sign) return;
                var desk = ApplicationObject.App.Desks.FirstOrDefault(a => a.Id == data.DeskId);
                if (desk == null) return;
                if (data.Mode == SubscribeMode.Finish || data.Mode == SubscribeMode.Delete)
                {
                    desk.Order = null;
                    ResetDeskStatus();
                    return;
                }
                using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IOrderService>();
                    var order = service.Get<TangOrder>(data.OrderObjectId);
                    if (data.Mode == SubscribeMode.Change)
                    {
                        order.TangOrderProducts = service.GetOrderProduct(data.OrderObjectId)?.ToObservable();
                    }
                    desk.Order = order;
                }
                ResetDeskStatus();
            });
            env.Dispose();
        }
        /// <summary>
        /// 订单状态改变时，发布消息
        /// </summary>
        /// <param name="msg"></param>
        private void PubSubscribe(SubscribeObj msg)
        {
            msg.Sign = ApplicationObject.App.ClientData.Sign;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                try
                {
                    var service = scope.Resolve<IOrderService>();
                    service.PubSubscribe(orderChangeChannel, JsonConvert.SerializeObject(msg));
                }
                catch (Exception ex)
                {
                    MessageTips(ex.ToString());
                }
            }
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
                //Mainthread.BeginInvoke((Action)delegate ()
                //{
                //});
                Mainthread.InvokeAsync(() =>
                {
                    //option.Order.CashierName = ApplicationObject.App.ClientData.Name;
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
                        case "ResetDeskStatus":
                            ResetDeskStatus();
                            break;
                        case "StockChange":
                            SetStock(message[1].ToObject<ProductStockModel>());
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        #endregion


        class SubscribeObj
        {
            /// <summary>
            /// 餐台id
            /// </summary>
            public int DeskId { get; set; }
            /// <summary>
            /// 订单id
            /// </summary>
            public string OrderObjectId { get; set; }
            /// <summary>
            /// 订单改变模式
            /// </summary>
            public SubscribeMode Mode { get; set; }
            /// <summary>
            /// 标识符
            /// </summary>
            public string Sign { get; set; }
        }
        enum SubscribeMode
        {
            Add = 0,
            Change = 1,
            Finish = 2,
            Delete = 3
        }

    }
}
