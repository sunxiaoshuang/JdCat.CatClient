
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
using Autofac;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using System.Collections.Generic;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.Common;
using Jiandanmao.Enum;

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
        public List<KeyValuePair<string, HostMode>> Modes { get; set; } = new List<KeyValuePair<string, HostMode>> { new KeyValuePair<string, HostMode>("正餐模式", HostMode.Chinese), new KeyValuePair<string, HostMode>("快餐模式", HostMode.Fast) };

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

                        }
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }


        public void Save(object o)
        {
            ApplicationObject.App.ClientData = this.ClientData;
            ApplicationObject.App.SaveClientData();
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                service.SetMealFee(MealFee);
            }
            SnackbarTips("保存成功，您需要重新启动客户端程序，才能使修改的配置生效！");
        }
        #endregion
    }
}
