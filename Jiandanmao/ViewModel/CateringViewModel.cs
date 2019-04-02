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
using JdCat.CatClient.Common;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class CateringViewModel : BaseViewModel
    {

        public object ThisContorler;
        /// <summary>
        /// 订单与餐桌过度
        /// </summary>
        public Transitioner Transitioner { get; set; }
        /// <summary>
        /// 菜单与结算过度
        /// </summary>
        public Transitioner Transitioner2 { get; set; }
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand StoreOrderLoadedCommand => new AnotherCommandImplementation(StoreOrderLoaded);

        #region 餐桌命令

        public ICommand SubmitNumberCommand => new AnotherCommandImplementation(SubmitNumber);
        public ICommand CancleCommand => new AnotherCommandImplementation(Cancle);
        public ICommand SelectTypeCommand => new AnotherCommandImplementation(SelectType);
        public ICommand AllDeskCommand => new AnotherCommandImplementation(AllDesk);
        public ICommand SearchCommand => new AnotherCommandImplementation(Search);

        /// <summary>
        /// 餐桌类型
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; set; } = ApplicationObject.App.DeskTypes;
        private ObservableCollection<Desk> _desks;
        /// <summary>
        /// 餐桌
        /// </summary>
        public ObservableCollection<Desk> Desks
        {
            get { return _desks; }
            set
            {
                this.MutateVerbose(ref _desks, value, RaisePropertyChanged());
            }
        }

        private Desk _desk;
        /// <summary>
        /// 选择的餐桌
        /// </summary>
        public Desk Desk
        {
            get { return _desk; }
            set
            {
                this.MutateVerbose(ref _desk, value, RaisePropertyChanged());
                // 每次赋值餐桌，重新加载一遍菜单
                ResetMenu();
            }
        }
        /// <summary>
        /// 点餐控件viewmodel
        /// </summary>
        public CateringProductViewModel _productViewModel;
        public CateringProductViewModel ProductViewModel
        {
            get { return _productViewModel; }
            set
            {
                this.MutateVerbose(ref _productViewModel, value, RaisePropertyChanged());
                // 每次赋值餐桌，重新加载一遍菜单
                ResetMenu();
            }
        }

        private int _peopleNumber = 2;
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleNumber
        {
            get
            {
                return _peopleNumber;
            }
            set
            {
                this.MutateVerbose(ref _peopleNumber, value, RaisePropertyChanged());
            }
        }
        #endregion

        #region 订单命令
        public ICommand DeleteOrderCommand => new AnotherCommandImplementation(DeleteOrder);
        public ICommand AddProductCommand => new AnotherCommandImplementation(AddProduct);
        public ICommand PayCommand => new AnotherCommandImplementation(Pay);
        public ICommand BackDeskCommand => new AnotherCommandImplementation(BackDesk);



        #endregion

        #region 订单页声明

        public ICommand ProductSearchCommand => new AnotherCommandImplementation(ProductSearch);
        public ICommand ClearProductKeyCommand => new AnotherCommandImplementation(ClearProductKey);



        #endregion




        #region 界面绑定方法

        public void Loaded(object o)
        {
            Init(o);
        }
        public void StoreOrderLoaded(object o)
        {
            var control = o as CateringOrder;
            Transitioner2 = control.transition2;
        }

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
                    // 餐桌初始化
                    if (DeskTypes == null || DeskTypes.Count == 0) return;
                        DeskTypes.ForEach(a => a.IsCheck = false);
                        Desks = ApplicationObject.App.Desks;
                    // 菜单初始化
                    ProductViewModel = new CateringProductViewModel
                        {
                            Types = new ObservableCollection<CateringProductViewModel.ProductTypeViewModel>()
                        };
                        ApplicationObject.App.Types.ForEach(type =>
                        {
                            var viewmodel = new CateringProductViewModel.ProductTypeViewModel
                            {
                                ProductType = type,
                                Products = new ObservableCollection<CateringProductViewModel.ProductViewModel>()
                            };
                            type.Products.ForEach(product => viewmodel.Products.Add(new CateringProductViewModel.ProductViewModel { Product = product }));
                            ProductViewModel.Types.Add(viewmodel);
                        });
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }

        public override void Submit(object o)
        {
            var desk = (Desk)o;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                //var service = scope.Resolve<ClientDbService>();
                //service.Delete(desk.Order);
            }
            Transitioner.SelectedIndex = 0;
            desk.Order = null;
            base.Submit(o);
        }

        private async void SubmitNumber(object o)
        {
            //Transitioner.SelectedIndex = 1;
            //if (Desk.Order == null)
            //{
            //    using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            //    {
            //        var service = scope.Resolve<ClientDbService>();
            //        var order = new StoreOrder
            //        {
            //            Code = UtilHelper.CreateOrderCode(ApplicationObject.App.Business.ID),
            //            BusinessId = ApplicationObject.App.Business.ID,
            //            DeskId = Desk.Id,
            //            DeskName = Desk.Name,
            //            PeopleQuantity = PeopleNumber,
            //            Status = StoreOrderStatus.Ordering
            //        };
            //        await service.AddAsync(order);
            //        Desk.Order = order;
            //    }
            //}
            //DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void Cancle(object o)
        {
            Transitioner.SelectedIndex = 0;
        }

        private void SelectType(object o)
        {
            var type = (DeskType)o;
            DeskTypes.ForEach(a => a.IsCheck = false);
            type.IsCheck = true;
            Desks = type.Desks;
        }

        private void AllDesk(object o)
        {
            DeskTypes.ForEach(a => a.IsCheck = false);
            Desks = ApplicationObject.App.Desks;
        }

        private void Search(object o)
        {
            var key = ((TextBox)o).Text?.Trim().ToLower();
            var type = DeskTypes.FirstOrDefault(a => a.IsCheck);
            ObservableCollection<Desk> desks;
            if (type == null)
            {
                desks = ApplicationObject.App.Desks;
            }
            else
            {
                desks = type.Desks;
            }
            if (key == null)
            {
                Desks = desks;
            }
            else
            {
                Desks = desks.Where(a => a.Name.ToLower().Contains(key)).ToObservable();
            }
        }

        private void ProductSearch(object o)
        {
            var txt = (TextBox)o;
            var key = txt.Text?.Trim();

        }

        private void ClearProductKey(object o)
        {
            var txt = (TextBox)o;
            txt.Text = string.Empty;

        }

        private async void DeleteOrder(object o)
        {
            SubmitParameter = o;
            await Confirm("确定删除订单吗？");
        }

        /// <summary>
        /// 加菜
        /// </summary>
        /// <param name="o"></param>
        private void AddProduct(object o)
        {

        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="o"></param>
        private void Pay(object o)
        {

        }

        /// <summary>
        /// 返回餐桌
        /// </summary>
        /// <param name="o"></param>
        private void BackDesk(object o)
        {
            Transitioner.SelectedIndex = 0;
        }

        /// <summary>
        /// 重新加载菜单
        /// </summary>
        private void ResetMenu()
        {
            //if (Desk == null) return;
            //var order = Desk.Order;
            //ProductViewModel.Types.ForEach(a => a.IsCheck = false);
            //ProductViewModel.Products = ProductViewModel.All;
            //ProductViewModel.All.ForEach(a => a.Quantity = 0);
            //if (order == null || order.OrderProducts == null) return;
            //order.OrderProducts.ForEach(a => {
            //    var product = ProductViewModel.All.FirstOrDefault(b => b.Product.ID == a.ProductId);
            //    if (product == null) return;
            //    product.Quantity += (int)a.Quantity.Value;
            //});
        }
        #endregion
    }
}
