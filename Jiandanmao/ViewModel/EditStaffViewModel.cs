using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.ViewModel
{
    public class EditStaffViewModel : BaseViewModel
    {
        #region 界面声明
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);


        #endregion

        #region 属性声明
        private EditStaff ThisControler;
        /// <summary>
        /// 是否是新增
        /// </summary>
        public bool IsNew { get => Staff == null || !Staff.IsExist(); }
        /// <summary>
        /// 是否点击确定
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 员工对象
        /// </summary>
        public Staff Staff { get; set; }


        private string _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get => _name; set => this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }

        private string _alise;
        /// <summary>
        /// 登录名
        /// </summary>
        public string Alise { get => _alise; set => this.MutateVerbose(ref _alise, value, RaisePropertyChanged()); }

        private Gender _gender;
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get => _gender; set => this.MutateVerbose(ref _gender, value, RaisePropertyChanged()); }

        private DateTime _birthday;
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get => _birthday; set => this.MutateVerbose(ref _birthday, value, RaisePropertyChanged()); }

        private string _cardId;
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardId { get => _cardId; set => this.MutateVerbose(ref _cardId, value, RaisePropertyChanged()); }

        #endregion

        #region 界面绑定方法

        public void Loaded(object o)
        {
            ThisControler = (EditStaff)o;
        }
        public override void Submit(object o)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorTip("请输入员工名称");
                return;
            }
            if (string.IsNullOrEmpty(Alise))
            {
                ErrorTip("请输入员工登录名");
                return;
            }
            if (IsNew)
            {
                if (string.IsNullOrEmpty(this.ThisControler.txtPwd.Password))
                {
                    ErrorTip("请输入登录密码");
                    return;
                }
                if (this.ThisControler.txtPwd.Password.Length < 5)
                {
                    ErrorTip("密码必须至少6位");
                    return;
                }
                if (ThisControler.txtPwd.Password != ThisControler.txtPwd2.Password)
                {
                    ErrorTip("两次输入密码不一致");
                    return;
                }
                Staff = new Staff
                {
                    Alise = Alise,
                    Password = UtilHelper.MD5(ThisControler.txtPwd.Password),
                    BusinessId = ApplicationObject.App.Business.ID
            };
            }
            Staff.Name = Name;
            Staff.Gender = Gender;
            Staff.Birthday = Convert.ToDateTime(ThisControler.date.Text);
            Staff.CardId = CardId;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IStaffService>();
                if (IsNew)
                {
                    service.SaveStaff(Staff);
                }
                else
                {
                    service.Update(Staff);
                }
            }
            IsSubmit = true;
            base.Submit(o);
        }
        #endregion

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="msg"></param>
        private void ErrorTip(string msg)
        {
            MessageTips(msg, "EditStaffHost");
            //ThisControler.EditStaffSnackbar.IsActive = true;
            //ThisControler.EditStaffSnackbar.Message = new SnackbarMessage { Content = msg };
        }

        public IEnumerable<GenderItem> GenderSource
        {
            get
            {
                yield return new GenderItem { Name = "男", Value = Gender.Boy };
                yield return new GenderItem { Name = "女", Value = Gender.Girl };
            }
        }

        public class GenderItem
        {
            public string Name { get; set; }
            public Gender Value { get; set; }
        }

    }
}
