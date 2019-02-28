
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Entity;

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

        private ObservableCollection<Order> _items = new ObservableCollection<Order>();
        public ObservableCollection<Order> Items => _items;
        private PagingQuery _pagingQuery;
        private PagingQuery PagingQuery => _pagingQuery;
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

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public void Loaded(object o)
        {
            _pagingQuery = new PagingQuery { PageSize = 20, PageIndex = 1 };
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
            _items.Clear();
            var list = await Request.GetOrders(ApplicationObject.App.Business, PagingQuery);
            if (list == null || list.Count == 0) return;
            list.ForEach(a => _items.Add(a));
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

        #endregion
    }
}
