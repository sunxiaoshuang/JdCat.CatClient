using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Jiandanmao.Code;
using Jiandanmao.Entity;
using Jiandanmao.Extension;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// 餐桌设定
    /// </summary>
    public class DeskViewModel : BaseViewModel
    {
        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand AddTypeCommand => new AnotherCommandImplementation(AddType);
        public ICommand AddDeskCommand => new AnotherCommandImplementation(AddDesk);
        public ICommand UpdateTypeCommand => new AnotherCommandImplementation(UpdateType);
        public ICommand DeleteTypeCommand => new AnotherCommandImplementation(DeleteType);

        #endregion



        #region 界面绑定方法

        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;
        public ObservableCollection<DeskType> Types { get; set; } = ApplicationObject.App.DeskTypes;
        private ObservableCollection<Desk> _desks;
        public ObservableCollection<Desk> Desks {
            get { return _desks; }
            set
            {
                this.MutateVerbose(ref _desks, value, RaisePropertyChanged());
            }
        }

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
                        args.Session.Close(false);
                        if (Types == null || Types.Count == 0) return;
                        var type = Types.FirstOrDefault(a => a.IsCheck);
                        if(type == null)
                        {
                            type = Types[0];
                            type.IsCheck = true;
                        }
                        if (type.Desks == null || type.Desks.Count == 0) return;
                        Desks = type.Desks;
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }


        public async void AddType(object o)
        {
            var max = 1;
            if (ApplicationObject.App.DeskTypes.Count > 0)
            {
                max = ApplicationObject.App.DeskTypes.Max(a => a.Sort) + 1;
            }
            var dialog = new AddDeskType(new DeskType { BusinessId = ApplicationObject.App.Business.ID, Sort = max });
            await DialogHost.Show(dialog, "RootDialog");
        }
        public async void AddDesk(object o)
        {
            var type = Types.FirstOrDefault(a => a.IsCheck);
            if (type == null)
            {
                MessageTips("请先选择餐桌区域！");
                return;
            }
            var dialog = new AddDesk(new Desk { BusinessId = ApplicationObject.App.Business.ID, DeskTypeId = type.Id, Quantity = 2 });
            await DialogHost.Show(dialog, "RootDialog");
        }
        public async void UpdateType(object o)
        {
            var type = Types.FirstOrDefault(a => a.IsCheck);
            var dialog = new AddDeskType(type);
            await DialogHost.Show(dialog, "RootDialog");
        }
        public async void DeleteType(object o)
        {
            var type = Types.FirstOrDefault(a => a.IsCheck);
            if (type == null) return;
            if(type.DeskQuantity > 0)
            {
                MessageTips("不可以删除存在餐桌的区域！");
                return;
            }
            await ShowLoadingDialog(Task.Run(async () => {
                var result = await Request.DeleteDeskType(type);
                if (result.Success)
                {
                    await Mainthread.BeginInvoke((Action)delegate ()
                    {
                        Types.Remove(type);
                    });
                }
            }));
        }
        #endregion
    }
}
