
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Entity;
using Autofac;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using System.Collections.Generic;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.Common;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// SystemSettingViewModel 绑定UserControler -> SystemSetting.xaml Mvvm
    /// </summary>
    public class SystemSettingViewModel : BaseViewModel
    {
        public SystemSettingViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            SnackbarMessageQueue = snackbarMessageQueue;
        }
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        public ICommand AddFlavorCommand => new AnotherCommandImplementation(AddFlavor);
        public ICommand DeleteFlavorCommand => new AnotherCommandImplementation(DeleteFlavor);
        public ICommand AddPaymentTypeCommand => new AnotherCommandImplementation(AddPaymentType);
        public ICommand DeletePaymentTypeCommand => new AnotherCommandImplementation(DeletePaymentType);
        private ClientData _clientData;
        /// <summary>
        /// 客户端本地数据
        /// </summary>
        public ClientData ClientData
        {
            get { return _clientData; }
            set { this.MutateVerbose(ref _clientData, value, RaisePropertyChanged()); }
        }
        private double _mealFee;
        /// <summary>
        /// 餐位费
        /// </summary>
        public double MealFee { get => _mealFee; set => this.MutateVerbose(ref _mealFee, value, RaisePropertyChanged()); }

        private ObservableCollection<string> _flavors;
        /// <summary>
        /// 口味列表
        /// </summary>
        public ObservableCollection<string> Flavors { get => _flavors; set => this.MutateVerbose(ref _flavors, value, RaisePropertyChanged()); }
        private string _flavorText;
        /// <summary>
        /// 口味文本
        /// </summary>
        public string FlavorText { get => _flavorText; set => this.MutateVerbose(ref _flavorText, value, RaisePropertyChanged()); }

        private ObservableCollection<PaymentType> _paymentTypes;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        public ObservableCollection<PaymentType> PaymentTypes { get => _paymentTypes; set => this.MutateVerbose(ref _paymentTypes, value, RaisePropertyChanged()); }
        private string _paymentTypeText;
        /// <summary>
        /// 支付方式文本
        /// </summary>
        public string PaymentTypeText { get => _paymentTypeText; set => this.MutateVerbose(ref _paymentTypeText, value, RaisePropertyChanged()); }

        #endregion

        #region 界面绑定方法


        public void Loaded(object o)
        {
            Init(o);
        }


        /// <summary>
        /// 初始化，读取在线配置信息
        /// </summary>
        private void Init(object o)
        {
            var loadingDialog = new LoadingDialog();

            var result = DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                ThreadStart start = delegate ()
                {
                    Mainthread.BeginInvoke((Action)delegate ()// 异步更新界面
                    {
                        args.Session.Close(false);

                        ClientData = (ClientData)ApplicationObject.App.ClientData.Clone();
                        using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                        {
                            var service = scope.Resolve<IUtilService>();
                            MealFee = service.GetMealFee();
                            Flavors = service.GetFlavors()?.ToObservable() ?? new ObservableCollection<string>();

                            var paymentService = scope.Resolve<IPaymentTypeService>();
                            PaymentTypes = paymentService.GetAll()?.ToObservable() ?? new ObservableCollection<PaymentType>();
                        }
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }
        public void AddFlavor(object o)
        {
            if (string.IsNullOrEmpty(FlavorText)) return;
            Flavors.Add(FlavorText);
            FlavorText = string.Empty;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        public void DeleteFlavor(object o)
        {
            Flavors.Remove(o.ToString());
        }

        private List<PaymentType> _delList;         // 被删除的支付方式
        private List<PaymentType> _addList;         // 新添加的支付方式
        public void AddPaymentType(object o)
        {
            if (string.IsNullOrEmpty(PaymentTypeText)) return;
            var payment = new PaymentType { BusinessId = ApplicationObject.App.Business.ID, Name = PaymentTypeText, Icon = SystemIcon.Other };
            if (_addList == null) _addList = new List<PaymentType>();
            _addList.Add(payment);
            PaymentTypes.Add(payment);
            PaymentTypeText = string.Empty;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        public void DeletePaymentType(object o)
        {
            var payment = (PaymentType)o;
            PaymentTypes.Remove(payment);
            if (_addList != null && _addList.Contains(payment))
            {
                _addList.Remove(payment);
            }
            else
            {
                if (_delList == null) _delList = new List<PaymentType>();
                _delList.Add(payment);
            }
        }



        public void Save(object o)
        {
            ApplicationObject.App.ClientData = this.ClientData;
            ApplicationObject.App.SaveClientData();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                service.SetMealFee(MealFee);
                service.SetFlavors(Flavors);

                var paymentService = scope.Resolve<IPaymentTypeService>();
                if (_addList != null && _addList.Count > 0)
                {
                    _addList.ForEach(a => paymentService.Add(a));
                    _addList = null;
                }
                if (_delList != null && _delList.Count > 0)
                {
                    _delList.ForEach(a => paymentService.Remove(a));
                    _delList = null;
                }
            }
            SnackbarTips("保存成功，您需要重新启动客户端程序，才能使修改的配置生效！");
        }
        #endregion
    }
}
