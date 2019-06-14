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
using System.Collections.Generic;
using JdCat.CatClient.IService;
using System.Text.RegularExpressions;
using JdCat.CatClient.Common;
using System.Threading.Tasks;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// ChineseFoodDetail 订单商品详情页
    /// </summary>
    public class FastFoodHoogupViewModel : BaseViewModel
    {
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand SelectedChangeCommand => new AnotherCommandImplementation(SelectedChange);
        public ICommand SubmitItemCommand => new AnotherCommandImplementation(SubmitItem);
        public ICommand RemoveCommand => new AnotherCommandImplementation(RemoveAsync);

        #region 界面属性

        private ObservableCollection<TangOrder> _orders;
        /// <summary>
        /// 订单列表
        /// </summary>
        public ObservableCollection<TangOrder> Orders { get => _orders; set => this.MutateVerbose(ref _orders, value, RaisePropertyChanged()); }

        private ObservableCollection<TangOrderProduct> _goods;
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public ObservableCollection<TangOrderProduct> Goods { get => _goods; set => this.MutateVerbose(ref _goods, value, RaisePropertyChanged()); }

        /// <summary>
        /// 选择的订单
        /// </summary>
        public TangOrder SelectedOrder { get; set; }

        #endregion

        private async void Loaded(object o)
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                Orders = (await service.GetHoogupOrdersAsync())?.OrderBy(a => a.CreateTime).ToObservable();
            }
        }

        private void SelectedChange(object o)
        {
            var list = (ListView)o;
            if (list.SelectedIndex == -1) return;
            var order = Orders[list.SelectedIndex];
            Goods = order.TangOrderProducts;
        }

        private void SubmitItem(object o)
        {
            SelectedOrder = (TangOrder)o;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private async void RemoveAsync(object o)
        {
            await Confirm("确定删除订单吗？", "HoogupDialog");
            if (!IsConfirm) return;
            var order = (TangOrder)o;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                await service.RemoveHoogupOrderAsync(order);
            }
            Orders.Remove(order);
        }

    }
}
