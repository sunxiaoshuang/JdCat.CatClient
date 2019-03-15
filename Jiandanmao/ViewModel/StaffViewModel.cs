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
using Jiandanmao.DataBase;
using Autofac;
using Jiandanmao.Enum;
using Jiandanmao.Helper;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class StaffViewModel : BaseViewModel
    {
        public StaffViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            SnackbarMessageQueue = snackbarMessageQueue ?? throw new ArgumentNullException(nameof(snackbarMessageQueue));
        }

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);

        public ICommand CreateCommand => new AnotherCommandImplementation(Create);

        public ICommand EditCommand => new AnotherCommandImplementation(Edit);

        public ICommand DeleteCommand => new AnotherCommandImplementation(Delete);


        #region 属性定义

        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get
            {
                return _staffs;
            }
            set
            {
                this.MutateVerbose(ref _staffs, value, RaisePropertyChanged());
            }
        }

        public Staff SelectedStaff { get; set; }

        #endregion


        #region 界面绑定方法

        public void Loaded(object o)
        {
            Init(o);
        }

        public override void Submit(object o)
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IStaffService>();
                service.Remove(SelectedStaff);
                LoadStaff();
            }
            base.Submit(o);
        }

        private void Init(object o)
        {
            var loadingDialog = new LoadingDialog();

            var result = DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                ThreadStart start = delegate ()
                {
                    Mainthread.BeginInvoke((Action)delegate ()
                    {
                        args.Session.Close(false);
                        LoadStaff();
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }
        /// <summary>
        /// 加载所有员工
        /// </summary>
        private void LoadStaff()
        {
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IStaffService>();
                var list = service.GetAll();
                if (list == null) return;
                Staffs = list.Where(a => a.BusinessId == ApplicationObject.App.Business.ID).ToObservable();
            }
        }

        private async void Create(object o)
        {
            var vm = new EditStaffViewModel { Birthday = DateTime.Now };
            var uc = new EditStaff() { DataContext = vm };
            await DialogHost.Show(uc, "RootDialog");
            if (vm.IsSubmit)
            {
                LoadStaff();
            }
        }

        private async void Edit(object o)
        {
            var entity = (Staff)o;
            var vm = new EditStaffViewModel { Staff = entity, Alise = entity.Alise, Name = entity.Name, Birthday = entity.Birthday.Value, CardId = entity.CardId, Gender = entity.Gender };
            var uc = new EditStaff() { DataContext = vm };
            await DialogHost.Show(uc, "RootDialog");
            if (vm.IsSubmit)
            {
                LoadStaff();
            }
        }

        private async void Delete(object o)
        {
            SelectedStaff = (Staff)o;
            await Confirm("确定删除吗？");
        }

        #endregion
    }
}
