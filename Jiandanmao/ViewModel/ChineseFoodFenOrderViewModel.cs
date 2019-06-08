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
        public ChineseFoodFenOrderViewModel(IEnumerable<Desk> desks)
        {
            DeskChanges = desks.Where(a => a.Order != null).ToObservable();
            DeskTargets = desks.Where(a => a.Order == null).ToObservable();
        }

        public ICommand ConfirmCommand => new AnotherCommandImplementation(ConfirmChange);

        #region 属性

        private Desk _deskChange;
        /// <summary>
        /// 更换餐台
        /// </summary>
        public Desk DeskChange { get => _deskChange; set => this.MutateVerbose(ref _deskChange, value, RaisePropertyChanged()); }

        private Desk _deskTarget;
        /// <summary>
        /// 目标餐台
        /// </summary>
        public Desk DeskTarget { get => _deskTarget; set => this.MutateVerbose(ref _deskTarget, value, RaisePropertyChanged()); }

        private ObservableCollection<Desk> _deskChanges;
        /// <summary>
        /// 可选择的更换餐台
        /// </summary>
        public ObservableCollection<Desk> DeskChanges { get => _deskChanges; set => this.MutateVerbose(ref _deskChanges, value, RaisePropertyChanged()); }

        private ObservableCollection<Desk> _deskTargets;
        /// <summary>
        /// 可选择的目标餐台
        /// </summary>
        public ObservableCollection<Desk> DeskTargets { get => _deskTargets; set => this.MutateVerbose(ref _deskTargets, value, RaisePropertyChanged()); }
        #endregion

        #region 界面绑定方法

        public async void ConfirmChange(object o)
        {
            if(DeskTarget == null || DeskTarget == null)
            {
                MessageTips("请选择餐台信息！", "ChangeDeskDialog");
                return;
            }
            await Confirm($"确定将餐台[{DeskChange.Name}]转到[{DeskTarget.Name}]吗？", "ChangeDeskDialog");
            if (!IsConfirm) return;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        

        #endregion

    }
}