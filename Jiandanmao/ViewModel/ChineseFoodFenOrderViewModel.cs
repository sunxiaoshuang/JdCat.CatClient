using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Input;

namespace Jiandanmao.ViewModel
{
    public class ChineseFoodFenOrderViewModel : BaseViewModel
    {
        public ChineseFoodFenOrderViewModel(IEnumerable<Desk> desks, TangOrder order, TangOrderProduct product)
        {
            Desks = desks.Where(a => a.Order != null && a.Id != order.DeskId).ToObservable();
            Good = product;
        }

        public ICommand ConfirmCommand => new AnotherCommandImplementation(ConfirmChange);

        #region 属性

        private Desk _desk;
        /// <summary>
        /// 选择的餐台
        /// </summary>
        public Desk Desk { get => _desk; set => this.MutateVerbose(ref _desk, value, RaisePropertyChanged()); }

        public TangOrderProduct Good { get; set; }

        private ObservableCollection<Desk> _desks;
        /// <summary>
        /// 可分单的餐台
        /// </summary>
        public ObservableCollection<Desk> Desks { get => _desks; set => this.MutateVerbose(ref _desks, value, RaisePropertyChanged()); }
        #endregion

        #region 界面绑定方法

        public async void ConfirmChange(object o)
        {
            if(Desk == null)
            {
                MessageTips("请选择餐台！", "FenOrderDialog");
                return;
            }
            await Confirm($"确定将商品[{Good.Name}]分单到[{Desk.Name}]吗？", "FenOrderDialog");
            if (!IsConfirm) return;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        

        #endregion

    }
}