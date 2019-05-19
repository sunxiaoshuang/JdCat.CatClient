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
using JdCatModel = JdCat.CatClient.Model;
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
        //public ICommand AmountResetCommand => new AnotherCommandImplementation(AmountReset);
        public ICommand CheckPaymentCommand => new AnotherCommandImplementation(CheckPayment);
        //public ICommand ClickMoneyCommand => new AnotherCommandImplementation(ClickMoney);
        //public ICommand ActualChangedCommand => new AnotherCommandImplementation(ActualChanged);
        public ICommand SubmitPaymentCommand => new AnotherCommandImplementation(SubmitPayment);
        public ICommand RemarkChangeCommand => new AnotherCommandImplementation(RemarkChange);
        public ICommand PrePrintCommand => new AnotherCommandImplementation(PrePrint);
        public ICommand DeskStatusChangedCommand => new AnotherCommandImplementation(DeskStatusChanged);
        public ICommand RefreshDeskCommand => new AnotherCommandImplementation(RefreshDesk);
        public ICommand OrderCatCommand => new AnotherCommandImplementation(OrderCat);
        public ICommand EditPeopleNumberCommand => new AnotherCommandImplementation(EditPeopleNumber);
        public ICommand EnterToAddCommand => new AnotherCommandImplementation(EnterToAdd);
        public ICommand PreferentialChangedCommand => new AnotherCommandImplementation(PreferentialChanged);
        public ICommand DiscountChangedCommand => new AnotherCommandImplementation(DiscountChanged);
        public ICommand ReceivedChangedCommand => new AnotherCommandImplementation(ReceivedChanged);




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

        private ListObject<Product> _productObject = new ListObject<Product>(15);
        public ListObject<Product> ProductObject { get => _productObject; set => this.MutateVerbose(ref _productObject, value, RaisePropertyChanged()); }
        #endregion

        public ObservableCollection<string> _orderRemarks;
        /// <summary>
        /// 订单备注
        /// </summary>
        public ObservableCollection<string> OrderRemarks { get => _orderRemarks; set => this.MutateVerbose(ref _orderRemarks, value, RaisePropertyChanged()); }

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
        /// 优惠金额
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

        #endregion

        #endregion


        #region 界面绑定方法
        private bool isLoaded = false;
        private void Loaded(object o)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                Init(o);
            }
        }
        private void DeskLoaded(object o)
        {

        }
        private void OrderLoaded(object o)
        {
            OrderController = (ChineseFoodOrder)o;
            txtProductKey = OrderController.txtProductkey;
        }
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
                StaffName = ApplicationObject.App.Staff?.Name
            };
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                order.MealFee = mealFee * order.PeopleNumber;
                service.SaveOrder(order);
            }
            SelectedDesk.Order = order;
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
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(order);
                service.SaveOrderProduct(orderProduct);
            }
            // 添加完新商品后，发布变更通知
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private void Reduce(object o)
        {
            var product = (TangOrderProduct)o;
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
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(order);
                service.UpdateOrderProduct(product);
            }
            // 减少商品后，发布变更通知
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private void Increase(object o)
        {
            var product = (TangOrderProduct)o;
            var order = SelectedDesk.Order;
            product.Quantity++;
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.Id == product.ProductId);
            good.SelectedQuantity++;
            product.Amount = product.Quantity * product.Price;
            CalcOrderAmount();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Update(order);
                service.UpdateOrderProduct(product);
            }
            // 增加商品后，发布变更通知
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
        }
        private async void ProductClick(object o)
        {
            var order = SelectedDesk.Order;
            var orderProduct = (TangOrderProduct)o;
            if (orderProduct.ProductStatus == TangOrderProductStatus.Return) return;
            order.TangOrderProducts.ForEach(a => a.IsSelected = false);

            orderProduct.IsSelected = true;
            var viewModel = new ChineseFoodDetailViewModel(order, orderProduct, ApplicationObject.App.Products.First(a => a.Id == orderProduct.ProductId));
            var detail = new ChineseFoodDetail { DataContext = viewModel };

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
                SubmitOrder();
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
                ReSubmitOrder();
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
            var amount = Math.Round(SelectedDesk.Order.Amount, 2);
            var paymentItem = PaymentTypes.FirstOrDefault(a => a.IsSelected);
            var payment = paymentItem?.Target;
            var order = SelectedDesk.Order;
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
            order.PaymentTypeObjectId = payment.ObjectId;
            order.PaymentTypeId = payment.Id;
            order.PaymentTypeName = payment.Name;
            order.PayTime = DateTime.Now;
            order.ReceivedAmount = ReceivedAmount;
            order.GiveAmount = GiveAmount;
            order.PaymentRemark = PaymentRemark;
            order.PreferentialAmount = PreferentialAmount;
            order.OrderDiscount = OrderDiscount;
            order.StaffId = ApplicationObject.App.Staff.Id;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.Payment(order);
            }
            PrintOrder(new PrintOption
            {
                Title = "结算单",
                Order = order,
                Type = 1,
                Mode = PrintMode.Payment,
                Products = order.TangOrderProducts.Where(a => a.ProductStatus != TangOrderProductStatus.Return)
            });
            SelectedDesk.Order = null;
            paymentItem.IsSelected = false;
            PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Finish });
            BackDesk();
            SnackbarTips("结算成功");
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
        }

        private void EnterToAdd(object o)
        {
            if (ProductObject.OriginalList.Count != 1) return;
            AddProduct(ProductObject.OriginalList[0]);
            var tb = (TextBox)o;
            tb.Text = string.Empty;
        }

        private void PreferentialChanged(object o)
        {
            PreferentialAmount = ValidateInput((TextBox)o);
            CalcPayment();
        }
        private void DiscountChanged(object o)
        {
            OrderDiscount = ValidateInput((TextBox)o);
            CalcPayment();
        }
        private void ReceivedChanged(object o)
        {
            ReceivedAmount = ValidateInput((TextBox)o);
            GiveAmount = Math.Round(ReceivedAmount - ActualAmount, 2);
        }


        #endregion

        #region 私有方法

        /// <summary>
        /// 计算订单总额
        /// </summary>
        private void CalcOrderAmount()
        {
            var order = SelectedDesk.Order;
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
        private void SubmitOrder()
        {
            var order = SelectedDesk.Order;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.SubmitOrder(order);
            }
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
        private void ReSubmitOrder()
        {
            var order = SelectedDesk.Order;
            var products = order.TangOrderProducts.Where(a => a.ProductStatus == TangOrderProductStatus.Add).ToList();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.ReSubmitOrder(order);
            }
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
        /// 验证输入，返回0时验证失败
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private double ValidateInput(TextBox txt)
        {
            var text = txt.Text.Trim();
            //if (string.IsNullOrEmpty(text)) txt.Text = text = "0";
            if (!double.TryParse(text, out double num))
            {
                var reg = Regex.Match(text, @"\d+");
                txt.Text = reg.Value;
                //txt.SelectionStart = int.MaxValue;
                return 0;
            }
            //txt.SelectionStart = int.MaxValue;
            return num;
        }
        /// <summary>
        /// 计算支付
        /// </summary>
        private void CalcPayment()
        {
            // 计算优惠后再折扣的金额
            var amount = OrderAmount - PreferentialAmount;
            var total = Math.Round(amount * OrderDiscount / 10, 2);

            ActualAmount = ReceivedAmount = total;
            GiveAmount = 0;
        }
        /// <summary>
        /// 计算找零
        /// </summary>
        //private void CalcBalance()
        //{
        //    var total = ActualAmount;
        //    var preferential = double.Parse(txtPreferential.Text);
        //    Balance = Math.Round(total + preferential - SelectedDesk.Order.Amount, 2);
        //}
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
        private void ResetDeskStatus()
        {
            var desks = ApplicationObject.App.Desks;
            if (ProductTypes == null || ProductTypes.Count == 0) ProductTypes = ApplicationObject.App.Types;
            UsingDeskCount = desks.Count(a => a.Order != null);
            FreeDeskCount = desks.Count(a => a.Order == null);
            DeskCount = desks.Count();
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
            IEnumerable<Product> products = ApplicationObject.App.Products.Where(a => (a.Scope & ActionScope.Store) > 0);
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
                    ResetDeskStatus();
                });
            }
        }

        #endregion


        class SubscribeObj
        {
            public int DeskId { get; set; }
            public string OrderObjectId { get; set; }
            public SubscribeMode Mode { get; set; }
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
