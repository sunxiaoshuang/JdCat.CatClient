using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
using System.Threading.Tasks;
using Autofac;
using JdCat.CatClient.IService;
using System.Windows.Controls;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// 堂食订单列表
    /// </summary>
    public class TangOrderListViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand PrePageCommand => new AnotherCommandImplementation(PrePageAsync);
        public ICommand NextPageCommand => new AnotherCommandImplementation(NextPageAsync);
        public ICommand CatCommand => new AnotherCommandImplementation(Cat);
        public ICommand PrintAllCommand => new AnotherCommandImplementation(a => Print(a, 0));
        public ICommand PrintFrontCommand => new AnotherCommandImplementation(a => Print(a, 1));
        public ICommand PrintBackgroundCommand => new AnotherCommandImplementation(a => Print(a, 2));

        /// <summary>
        /// 分页
        /// </summary>
        public PagingQuery Paging { get; set; } = new PagingQuery { PageSize = 30, PageIndex = 1 };

        /// <summary>
        /// 订单列表
        /// </summary>
        public ObservableCollection<TangOrder> Items { get; set; } = new ObservableCollection<TangOrder>();

        private TangOrder _detail;
        /// <summary>
        /// 订单详细
        /// </summary>
        public TangOrder Detail { get => _detail; set => this.MutateVerbose(ref _detail, value, RaisePropertyChanged()); }

        private bool _preEnable;
        /// <summary>
        /// 是否可以上一页
        /// </summary>
        public bool PreEnable
        {
            get { return _preEnable; }
            set
            {
                this.MutateVerbose(ref _preEnable, value, RaisePropertyChanged());
            }
        }

        private bool _nextEnable;
        /// <summary>
        /// 是否可以下一页
        /// </summary>
        public bool NextEnable
        {
            get { return _nextEnable; }
            set
            {
                this.MutateVerbose(ref _nextEnable, value, RaisePropertyChanged());
            }
        }

        #endregion



        #region 界面绑定方法

        public void Loaded(object o)
        {
            Init(o);
        }

        private void Init(object o)
        {
            var loadingDialog = new LoadingDialog();

            var result = DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                void start()
                {
                    Mainthread.BeginInvoke((Action)async delegate ()// 异步更新界面
                    {
                        Paging.PageIndex = 1;
                        await LoadOrderAsync();

                        args.Session.Close(false);
                    });
                }

                new Thread(start).Start(); // 启动线程

            });

        }

        #endregion


        private async Task LoadOrderAsync()
        {
            Items.Clear();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var list = await service.GetRangeReverseAsync<TangOrder>(Paging);
                list?.Reverse();
                list?.ForEach(a => Items.Add(a));
            }
            NextEnable = Items.Count >= Paging.PageSize;
            PreEnable = Paging.PageIndex > 1;
        }

        private async void PrePageAsync(object obj)
        {
            if (Paging.PageIndex == 1) return;
            Paging.PageIndex--;
            await LoadOrderAsync();
        }

        private async void NextPageAsync(object obj)
        {
            Paging.PageIndex++;
            await LoadOrderAsync();
        }

        private async void Cat(object obj)
        {
            var order = obj as TangOrder;
            await LoadDetailAsync(order);
            await DialogHost.Show(new TangOrderInfo { DataContext = order }, "RootDialog");
        }

        private async void Print(object obj, int type)
        {
            var order = obj as TangOrder;
            await LoadDetailAsync(order);
            if (order.TangOrderProducts == null || order.TangOrderProducts.Count == 0) return;
            ApplicationObject.Print(order, type, new PrintOption { Mode = Enum.PrintMode.Repeat, Products = order.TangOrderProducts, Order = order, Title = "补打单", Type = type });
        }

        /// <summary>
        /// 加载订单详情
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task LoadDetailAsync(TangOrder order)
        {
            if (order.TangOrderProducts == null)
            {
                using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IOrderService>();
                    order.TangOrderProducts = (await service.GetRelativeEntitysAsync<TangOrderProduct, TangOrder>(order.ObjectId))?.ToObservable();
                    order.TangOrderPayments = (await service.GetRelativeEntitysAsync<TangOrderPayment, TangOrder>(order.ObjectId))?.ToObservable();
                    order.TangOrderActivity = (await service.GetRelativeEntitysAsync<TangOrderActivity, TangOrder>(order.ObjectId))?.ToObservable();
                }
            }
        }

    }
}
