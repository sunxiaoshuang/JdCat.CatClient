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
    public class MixPaymentViewModel : BaseViewModel
    {
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AddPaymentCommand => new AnotherCommandImplementation(AddPayment);
        public ICommand DeletePaymentCommand => new AnotherCommandImplementation(DeletePayment);
        public ICommand AmountChangedCommand => new AnotherCommandImplementation(AmountChanged);

        #region 界面属性

        private TangOrder _order;
        /// <summary>
        /// 待支付订单
        /// </summary>
        public TangOrder Order { get => _order; set => this.MutateVerbose(ref _order, value, RaisePropertyChanged()); }

        private ObservableCollection<PaymentType> _types;
        /// <summary>
        /// 付款方式列表
        /// </summary>
        public ObservableCollection<PaymentType> Types { get => _types; set => this.MutateVerbose(ref _types, value, RaisePropertyChanged()); }

        private ObservableCollection<TangOrderPayment> _payments;
        /// <summary>
        /// 收款方式
        /// </summary>
        public ObservableCollection<TangOrderPayment> Payments { get => _payments; set => this.MutateVerbose(ref _payments, value, RaisePropertyChanged()); }

        #endregion
        public MixPaymentViewModel(TangOrder order)
        {
            Order = order;
        }

        private async void Loaded(object o)
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                var payments = await service.GetAllAsync<PaymentType>();
                if (payments == null || payments.Count == 0) return;
                Types = payments.ToObservable();
                Payments = new ObservableCollection<TangOrderPayment>
                {
                    new TangOrderPayment{ Name = payments[0].Name, Amount = Order.ActualAmount }
                };
            }
        }

        private void AddPayment(object o)
        {
            if (Types == null || Types.Count == 0) return;
            var amount = Math.Round(Order.ActualAmount - Payments.Sum(a => a.Amount), 2);
            Payments.Add(new TangOrderPayment { Name = Types[0].Name, Amount = amount });
        }

        private void DeletePayment(object o)
        {
            if (Payments.Count == 1) return;
            Payments.Remove((TangOrderPayment)o);
        }

        public void AmountChanged(object o)
        {
            if (Payments.Count != 2) return;
            var payment = (TangOrderPayment)o;
            var other = Payments.First(a => a != payment);
            other.Amount = Math.Round(Order.ActualAmount - payment.Amount, 2);
        }

        public override void Submit(object o)
        {
            var amount = Payments.Sum(a => a.Amount);
            if(amount != Order.ActualAmount)
            {
                MessageTips("支付总金额必须等于应收金额", "MixPayment");
                return;
            }
            if(Payments.GroupBy(a => a.Name).Count() != Payments.Count)
            {
                MessageTips("不能出现两个相同的支付方式", "MixPayment");
                return;
            }
            Payments.ForEach(item => {
                var type = Types.First(a => a.Name == item.Name);
                item.PaymentTypeId = type.Id;
                item.PaymentTypeObjectId = type.ObjectId;
                item.OrderId = Order.Id;
                item.OrderObjectId = Order.ObjectId;
            });
            base.Submit(o);
        }

    }
}
