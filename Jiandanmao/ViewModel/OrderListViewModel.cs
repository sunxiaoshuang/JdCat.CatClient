﻿
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

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class OrderListViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand PrePageCommand => new AnotherCommandImplementation(PrePage);
        public ICommand NextPageCommand => new AnotherCommandImplementation(NextPage);
        public ICommand RefreshCommand => new AnotherCommandImplementation(Refresh);
        public ICommand CatCommand => new AnotherCommandImplementation(Cat);
        public ICommand PrintAllCommand => new AnotherCommandImplementation(a => Print(a, 0));
        public ICommand PrintFrontCommand => new AnotherCommandImplementation(a => Print(a, 1));
        public ICommand PrintBackgroundCommand => new AnotherCommandImplementation(a => Print(a, 2));

        private ObservableCollection<Order> _items = new ObservableCollection<Order>();
        public ObservableCollection<Order> Items { get => _items; }
        private PagingQuery _paging = new PagingQuery { PageSize = 20, PageIndex = 1 };
        private PagingQuery PagingQuery { get => _paging; }
        private bool _preEnable;
        public bool PreEnable
        {
            get { return _preEnable; }
            set
            {
                this.MutateVerbose(ref _preEnable, value, RaisePropertyChanged());
            }
        }
        private bool _nextEnable;
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
                ThreadStart start = delegate ()
                {
                    Mainthread.BeginInvoke((Action)delegate ()// 异步更新界面
                    {
                        //Thread.Sleep(1000);
                        args.Session.Close(false);
                        LoadOrders();

                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }

        private async void LoadOrders()
        {
            Items.Clear();
            var list = await Request.GetOrdersAsync(ApplicationObject.App.Business, PagingQuery);
            if (list == null || list.Count == 0) return;
            list.ForEach(a => Items.Add(a));
            PreEnable = PagingQuery.CanPre;
            NextEnable = PagingQuery.CanNext;
        }

        private void PrePage(object obj)
        {
            PagingQuery.PageIndex--;
            LoadOrders();
        }

        private void NextPage(object obj)
        {
            PagingQuery.PageIndex++;
            LoadOrders();
        }

        private void Refresh(object obj)
        {
            PagingQuery.PageIndex = 1;
            LoadOrders();
        }

        private async void Cat(object obj)
        {
            var order = await GetOrderDetailAsync(obj as Order);
            var orderInfo = new OrderInfo(order);
            await DialogHost.Show(orderInfo, "RootDialog");
        }

        private async void Print(object obj, int type)
        {
            var order = await GetOrderDetailAsync(obj as Order);
            if (order.Products == null || order.Products.Count == 0) return;
            ApplicationObject.Print(order, type);
        }

        #endregion

        private async Task<Order> GetOrderDetailAsync(Order order)
        {
            if (!order.IsDetail)
            {
                var newOrder = await Request.GetOrderDetailAsync(order.Id);
                newOrder.IsDetail = true;
                Items.Replace(order, newOrder);
                return newOrder;
            }
            return order;
        }
    }
}
