using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using MaterialDesignThemes.Wpf.Transitions;
using Jiandanmao.Entity;
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
        private TextBox txtActual;          // 实际金额输入框
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
        public ICommand AmountResetCommand => new AnotherCommandImplementation(AmountReset);
        public ICommand CheckPaymentCommand => new AnotherCommandImplementation(CheckPayment);
        public ICommand ClickMoneyCommand => new AnotherCommandImplementation(ClickMoney);
        public ICommand ActualChangedCommand => new AnotherCommandImplementation(ActualChanged);
        public ICommand SubmitPaymentCommand => new AnotherCommandImplementation(SubmitPayment);
        public ICommand RemarkChangeCommand => new AnotherCommandImplementation(RemarkChange);
        public ICommand PrePrintCommand => new AnotherCommandImplementation(PrePrint);
        public ICommand DeskStatusChangedCommand => new AnotherCommandImplementation(DeskStatusChanged);
        public ICommand RefreshDeskCommand => new AnotherCommandImplementation(RefreshDesk);




        #region 属性声明
        private bool _isAll = true;
        /// <summary>
        /// 是否选中全部餐桌
        /// </summary>
        public bool IsAll { get => _isAll; set => this.MutateVerbose(ref _isAll, value, RaisePropertyChanged()); }
        private double _balance;
        /// <summary>
        /// 找零
        /// </summary>
        public double Balance { get => _balance; set => this.MutateVerbose(ref _balance, value, RaisePropertyChanged()); }
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
        private ObservableCollection<ProductType> _productTypes = ApplicationObject.App.Types?.ToObservable();
        public ObservableCollection<ProductType> ProductTypes { get => _productTypes; set => this.MutateVerbose(ref _productTypes, value, RaisePropertyChanged()); }
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products { get => _products; set => this.MutateVerbose(ref _products, value, RaisePropertyChanged()); }
        #endregion

        public ObservableCollection<SelectItem<JdCatModel.PaymentType>> _paymentTypes;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        public ObservableCollection<SelectItem<JdCatModel.PaymentType>> PaymentTypes { get => _paymentTypes; set => this.MutateVerbose(ref _paymentTypes, value, RaisePropertyChanged()); }

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
        }
        private void PaymentLoaded(object o)
        {
            var control = (ChineseFoodPayment)o;
            txtActual = control.txtActual;
        }

        private void Init(object o)
        {
            // 订阅订单变更消息
            SubscribeChanged();
            if (ApplicationObject.App.ClientData.IsHost)
            {
                // 订阅打印订单消息
                SubscribePrint();
            }
            // 餐桌
            if (Desks == null)
            {
                Desks = ApplicationObject.App.Desks;
            }
            // 商品
            if (Products == null)
            {
                Products = ApplicationObject.App.Products;
            }
            // 餐位费
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                mealFee = service.GetMealFee();
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
                BusinessId = ApplicationObject.App.Business.ID,
                DeskId = SelectedDesk.Id,
                DeskName = SelectedDesk.Name,
                OrderStatus = TangOrderStatus.Ordering,
                OrderSource = OrderSource.Cashier,
                OrderMode = OrderMode.ChineseFood,
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
            Products = ApplicationObject.App.Products;
        }
        private void ClickProductType(object o)
        {
            var type = (ProductType)o;
            ProductTypes?.ForEach(a => a.IsCheck = false);
            IsAllProduct = false;
            type.IsCheck = true;
            Products = type.Products;
        }
        private void AddProduct(object o)
        {
            var product = (Product)o;
            var order = SelectedDesk.Order;
            var format = product.Formats.First();
            if (order.TangOrderProducts == null) order.TangOrderProducts = new ObservableCollection<TangOrderProduct>();
            var orderProduct = order.TangOrderProducts?.FirstOrDefault(a => a.ProductId == product.ID && (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);
            var exist = orderProduct != null;
            if (!exist || format.ID != orderProduct.FormatId)
            {
                orderProduct = new TangOrderProduct
                {
                    Feature = product.Feature,
                    FormatId = format.ID,
                    Name = product.Name,
                    OrderObjectId = SelectedDesk.Order.ObjectId,
                    Price = product.Formats.FirstOrDefault()?.Price ?? 0,
                    OriginalPrice = product.Formats.FirstOrDefault()?.Price ?? 0,
                    Discount = 10,
                    ProductId = product.ID,
                    ProductIdSet = product.ProductIdSet,
                    ProductStatus = SelectedDesk.Order.OrderStatus == TangOrderStatus.Ordering ? TangOrderProductStatus.Order : TangOrderProductStatus.Add,
                    Status = EntityStatus.Normal
                };
                order.TangOrderProducts.Add(orderProduct);
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
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.ID == product.ProductId);
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
            var good = ApplicationObject.App.Products.FirstOrDefault(a => a.ID == product.ProductId);
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
            var viewModel = new ChineseFoodDetailViewModel(order, orderProduct, ApplicationObject.App.Products.First(a => a.ID == orderProduct.ProductId));
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
                PubSubscribe(new SubscribeObj { DeskId = SelectedDesk.Id, OrderObjectId = order.ObjectId, Mode = SubscribeMode.Change });
            }
        }
        private void BackDesk(object o = null)
        {
            ThisController.transitioner.SelectedIndex = 0;
            //ReloadDeskOrder();
        }
        private void ProductSearch(object o)
        {
            var textbox = (TextBox)o;
            var key = textbox.Text.Trim().ToLower();
            IsAllProduct = true;
            ProductTypes.ForEach(a => a.IsCheck = false);
            Products = ApplicationObject.App.Products?.Where(a => a.Name.Contains(key)).ToObservable();
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
            txt.Text = "";
            Products = ApplicationObject.App.Products;
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
        }
        private void AmountReset(object o)
        {
            var txt = (TextBox)o;
            txt.Text = 0.ToString();
            txt.SelectAll();
            txt.Focus();
            CalcBalance();
        }
        private void CheckPayment(object o)
        {
            var selectedItem = (SelectItem)o;
            PaymentTypes.ForEach(a => a.IsSelected = false);
            selectedItem.IsSelected = !selectedItem.IsSelected;
        }
        private void ClickMoney(object o)
        {
            var money = double.Parse(o.ToString());
            var actual = double.Parse(txtActual.Text);
            txtActual.Text = (money + actual).ToString();
            CalcBalance();
        }
        private void ActualChanged(object o)
        {
            var txt = (TextBox)o;
            var text = txt.Text.Trim();
            if (!double.TryParse(text, out double distance))
            {
                var reg = Regex.Match(text, @"\d+");
                txt.Text = reg.Value;
                txt.SelectionStart = int.MaxValue;
                return;
            }
            CalcBalance();
        }
        private async void SubmitPayment(object o)
        {
            var total = double.Parse(txtActual.Text);
            var amount = Math.Round(SelectedDesk.Order.Amount, 2);
            var payment = PaymentTypes.FirstOrDefault(a => a.IsSelected)?.Target;
            var order = SelectedDesk.Order;
            if (payment == null)
            {
                MessageTips("请选择收款方式！");
                return;
            }
            if (total < amount)
            {
                MessageTips("实收金额不能小于应收金额！");
                return;
            }
            await Confirm("确定收款吗？");
            if (!IsConfirm) return;
            order.PaymentTypeObjectId = payment.ObjectId;
            order.PaymentTypeName = payment.Name;
            order.PayTime = DateTime.Now;
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


        #endregion

        #region 私有方法

        /// <summary>
        /// 计算订单总额
        /// </summary>
        private void CalcOrderAmount()
        {
            var order = SelectedDesk.Order;
            order.MealFee = mealFee * order.PeopleNumber;
            order.Amount = order.MealFee;
            order.OriginalAmount = order.MealFee;
            order.TangOrderProducts?.Where(a => a.ProductStatus != TangOrderProductStatus.Return).ForEach(a =>
            {
                order.Amount += a.Amount;
                order.OriginalAmount += a.OriginalPrice * a.Quantity;
            });

            order.Amount = Math.Round(order.Amount, 2);
            order.OriginalAmount = Math.Round(order.OriginalAmount, 2);
            txtActual.Text = order.Amount.ToString();
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
                var unfinish = service.GetUnfinishOrder()?.Where(a => a.BusinessId == ApplicationObject.App.Business.ID).ToList();
                unfinish?.ForEach(order =>
                {
                    var desk = ApplicationObject.App.Desks.FirstOrDefault(a => a.Id == order.DeskId);
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
                if (item.Tag != null || item.Feature != JdCatModel.Enum.ProductFeature.SetMeal) return;
                var ids = item.ProductIdSet.Split(',').Select(a => int.Parse(a));
                item.Tag = products.Where(a => ids.Contains(a.ID)).ToList();
            });

            PubPrint(option);
        }
        /// <summary>
        /// 结账
        /// </summary>
        private void PrePay()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IPaymentTypeService>();
                PaymentTypes = service.GetAll()?.Select(a => new SelectItem<JdCatModel.PaymentType>(false, a.Name, a)).ToObservable();
            }
            txtActual.Text = SelectedDesk.Order.Amount.ToString();
        }
        /// <summary>
        /// 计算找零
        /// </summary>
        private void CalcBalance()
        {
            var total = double.Parse(txtActual.Text);
            Balance = Math.Round(total - SelectedDesk.Order.Amount, 2);
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
        private void ResetDeskStatus()
        {
            var desks = ApplicationObject.App.Desks;
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
            Balance = 0;                                                // 找零设置为零
            Products = ApplicationObject.App.Products;                  // 重新设置已选择的商品数量
            Products?.ForEach(a => a.SelectedQuantity = 0);
            order.TangOrderProducts?.ForEach(a =>
            {
                var product = Products?.FirstOrDefault(b => b.ID == a.ProductId);
                if (product == null) return;
                product.SelectedQuantity += a.Quantity;
            });
            if (order.OrderStatus == TangOrderStatus.Eating)
            {
                // 如果是正在用餐的状态，检查是否存在没有下单的商品，如果存在，则进入点菜模块
                var unOrderingGood = order.TangOrderProducts?.Count(a => (a.ProductStatus & TangOrderProductStatus.Cumulative) > 0);   // 未下单的商品数
                if (unOrderingGood == 0)
                {
                    OrderController.transition2.SelectedIndex = 1;      // 点菜模块
                    MenuButton = 1;                                     // 加菜功能区
                    PrePay();
                    return;
                }
                OrderController.transition2.SelectedIndex = 0;      // 结账模块
                MenuButton = 2;                                     // 结账功能区
                return;
            }
            // 如果不是用餐状态，则进入点餐模块
            OrderController.transition2.SelectedIndex = 0;
            MenuButton = 0;         // 点餐功能区
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
                    var order = service.Get(data.OrderObjectId);
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
                Mainthread.InvokeAsync(() => {
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
