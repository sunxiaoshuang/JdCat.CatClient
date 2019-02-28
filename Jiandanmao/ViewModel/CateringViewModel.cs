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

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// OrderList订单列表
    /// </summary>
    public class CateringViewModel : BaseViewModel
    {

        public object ThisContorler;
        public Transitioner Transitioner { get; set; }
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        #region 餐桌命令

        public ICommand SubmitNumberCommand => new AnotherCommandImplementation(SubmitNumber);
        public ICommand CancleCommand => new AnotherCommandImplementation(Cancle);
        public ICommand SelectTypeCommand => new AnotherCommandImplementation(SelectType);
        public ICommand AllDeskCommand => new AnotherCommandImplementation(AllDesk);
        public ICommand SearchCommand => new AnotherCommandImplementation(Search);

        /// <summary>
        /// 餐桌类型
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; set; } = ApplicationObject.App.DeskTypes;
        private ObservableCollection<Desk> _desks;
        /// <summary>
        /// 餐桌
        /// </summary>
        public ObservableCollection<Desk> Desks
        {
            get { return _desks; }
            set
            {
                this.MutateVerbose(ref _desks, value, RaisePropertyChanged());
            }
        }

        private Desk _desk;
        /// <summary>
        /// 选择的餐桌
        /// </summary>
        public Desk Desk
        {
            get { return _desk; }
            set
            {
                this.MutateVerbose(ref _desk, value, RaisePropertyChanged());
            }
        }

        private int _peopleNumber = 2;
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleNumber
        {
            get
            {
                return _peopleNumber;
            }
            set
            {
                this.MutateVerbose(ref _peopleNumber, value, RaisePropertyChanged());
            }
        }
        #endregion

        #region 订单页声明

        public ICommand ProductSearchCommand => new AnotherCommandImplementation(ProductSearch);
        public ICommand ClearProductKeyCommand => new AnotherCommandImplementation(ClearProductKey);



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
                        if (DeskTypes == null || DeskTypes.Count == 0) return;
                        DeskTypes.ForEach(a => a.IsCheck = false);
                        Desks = ApplicationObject.App.Desks;
                    });
                };

                new Thread(start).Start(); // 启动线程

            });

        }

        private void SubmitNumber(object o)
        {
            Transitioner.SelectedIndex = 1;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void Cancle(object o)
        {
            Transitioner.SelectedIndex = 0;
        }

        private void SelectType(object o)
        {
            var type = (DeskType)o;
            DeskTypes.ForEach(a => a.IsCheck = false);
            type.IsCheck = true;
            Desks = type.Desks;
        }

        private void AllDesk(object o)
        {
            DeskTypes.ForEach(a => a.IsCheck = false);
            Desks = ApplicationObject.App.Desks;
        }

        private void Search(object o)
        {
            var key = ((TextBox)o).Text?.Trim().ToLower();
            var type = DeskTypes.FirstOrDefault(a => a.IsCheck);
            ObservableCollection<Desk> desks;
            if(type == null)
            {
                desks = ApplicationObject.App.Desks;
            }
            else
            {
                desks = type.Desks;
            }
            if (key == null)
            {
                Desks = desks;
            }
            else
            {
                Desks = desks.Where(a => a.Name.ToLower().Contains(key)).ToObservable();
            }
        }

        private void ProductSearch(object o)
        {
            var txt = (TextBox)o;
            var key = txt.Text?.Trim();

        }
        private void ClearProductKey(object o)
        {
            var txt = (TextBox)o;
            txt.Text = string.Empty;
            
        }

        #endregion
    }
}
