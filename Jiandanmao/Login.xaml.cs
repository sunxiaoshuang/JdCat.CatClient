using MaterialDesignThemes.Wpf;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using Newtonsoft.Json;
using Autofac;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Common;
using System.Threading.Tasks;
using JdCat.CatClient.Model.Enum;

namespace Jiandanmao.Pages
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login
    {
        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        private double AnimationTime = 0.7;

        public bool LoginSuccess { get; set; }

        public string InfoDir { get; set; }

        public Login()
        {
            InitializeComponent();
            this.DataContext = ApplicationObject.App.Config;
            InfoDir = System.IO.Path.Combine(Environment.CurrentDirectory, "Info");
            if (!System.IO.Directory.Exists(InfoDir))
            {
                System.IO.Directory.CreateDirectory(InfoDir);
            }
            var logDir = System.IO.Path.Combine(Environment.CurrentDirectory, "Log");
            if (!System.IO.Directory.Exists(logDir))
            {
                System.IO.Directory.CreateDirectory(logDir);
            }
            ReadLastUser(out string name, out string password);
            NameTextBox.Text = name;
            PasswordBox.Password = password;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                PasswordBox.Focus();
                PasswordBox.SelectAll();
            }));
        }

        private string name;
        private string pw;
        private void Login_click(object sender, RoutedEventArgs e)
        {
            name = NameTextBox.Text?.Trim();
            pw = PasswordBox.Password?.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pw)) return;

            ShowLoadingDialog(name, pw);

        }

        //private void SignIn_click(object sender, RoutedEventArgs e)
        //{
        //    GridOperation.Fade_Out_Grid(LoginGrid, AnimationTime);
        //    GridOperation.Fade_Int_Grid(SignInGrid, AnimationTime);
        //}

        public void MessageTips(string message)
        {
            var sampleMessageDialog = new MessageDialog
            {
                Message = { Text = message }
            };

            DialogHost.Show(sampleMessageDialog, "LoginDialog");
        }

        private async void ShowLoadingDialog(string name, string pw)
        {
            var loadingDialog = new LoadingDialog();

            await DialogHost.Show(loadingDialog, "LoginDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                async void start()
                {
                    /* 登录流程
                       1. 首先检测是否员工登录
                       2. 如果登录成功，则直接读取远程商户数据，进入收银员系统
                       3. 如果登录不成功，则选择商户登录
                       4. 如果仍然登录失败，则提示登录失败
                       5. 如果登录成功，则进入管理员系统
                     */
                    await Mainthread.BeginInvoke((Action)async delegate ()
                    {
                        //Staff staff = null;
                        //Business business = null;
                        //using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                        //{
                        //    var service = scope.Resolve<IStaffService>();
                        //    staff = service.GetStaffByAlise(name);
                        //}
                        //if (staff != null && staff.Password == UtilHelper.MD5(pw))
                        //{
                        //    business = (await Request.GetBusinessAsync(staff.BusinessId)).Data;
                        //    ApplicationObject.App.Staff = staff;
                        //    ApplicationObject.App.IsAdmin = false;
                        //}
                        //else
                        //{
                        //    var result = await Request.LoginAsync(name, pw);
                        //    if (result.Success)
                        //    {
                        //        business = result.Data;
                        //        ApplicationObject.App.IsAdmin = true;
                        //    }
                        //    else
                        //    {
                        //        args.Session.Close();
                        //        MessageTips(result.Msg);
                        //        return;
                        //    }
                        //}
                        //args.Session.Close();
                        //ApplicationObject.App.Business = business;
                        //SaveLoginUser(name, pw);
                        //await ApplicationObject.App.Init();
                        //LoginSuccess = true;
                        //Close();



                        /* 登录流程
                         * 1. 员工登录：
                         *      a. 根据登录名，读取本地数据库中的员工信息
                         *      b. 如果存在该员工，则使用该员工的id、密码登录远程服务器
                         *      c. 如果登录成功，则进入系统；如果不成功，则进入下一步商户登录
                         *      d. 如果远程登录出错，则验证已取得的员工密码是否匹配，如果匹配成功则进入系统，如果不成功则进入下一步商户登录
                           2. 商户登录：
                                a. 使用帐号密码，以商户的身份登录服务器
                                b. 如果登录成功，则进入系统；如果不成功，则提示用户登录名或密码错误
                                c. 如果远程登录出错，则使用登录名读取本地数据库中的商户信息
                                d. 如果存在商户，则验证密码，密码验证成功进入系统，验证失败提示用户登录名或密码错误
                         */
                        using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                        {
                            var service = scope.Resolve<IUtilService>();
                            var staff = await service.GetStaffByAliseAsync(name.Trim().ToLower());
                            if (staff != null)
                            {
                                try
                                {
                                    var result = await Request.LoginStaffAsync(staff.Id, pw);
                                    if (result.Success)
                                    {
                                        EnterSystem(result.Data);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (staff.Password == UtilHelper.MD5(pw))
                                    {
                                        EnterSystem(staff);
                                        return;
                                    }
                                }
                            }
                            try
                            {
                                var result = await Request.LoginAsync(name, pw);
                                if (result.Success)
                                {
                                    EnterSystem(null, result.Data);
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                var business = await service.GetEntityByCodeAsync<Business>(name.Trim().ToLower());
                                if (business != null && business.Password == UtilHelper.MD5(pw))
                                {
                                    EnterSystem(null, business);
                                    return;
                                }
                            }

                            args.Session.Close();
                            MessageTips("帐号或密码错误");
                        }
                    });
                }

                new Thread(start).Start();
            });
        }

        private async void EnterSystem(Staff staff, Business business = null)
        {
            if (business == null)
            {
                using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IUtilService>();
                    business = await service.GetAsync<Business>(staff.BusinessId.ToString());
                }
            }
            else
            {
                ApplicationObject.App.IsAdmin = true;
            }
            ApplicationObject.App.Business = business;
            ApplicationObject.App.Staff = staff;
            await SaveLoginUserAsync(name, pw);
            ApplicationObject.App.Init();
            LoginSuccess = true;
            Close();
        }

        private void LoginWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Login_click(sender, e);
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ApplicationObject.App.Config.BackStageWebSite);
        }

        //private void BackToLogin_OnClick(object sender, RoutedEventArgs e)
        //{
        //    GridOperation.Fade_Out_Grid(SignInGrid, AnimationTime);
        //    GridOperation.Fade_Int_Grid(LoginGrid, AnimationTime);
        //}

        ///// <summary>
        ///// 注册新账号
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SignInNew_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (SignInPassWord.Password != SignInConfimPassWord.Password)
        //    {
        //        MessageTips("密码不一致！");
        //        return;
        //    }

        //    string name = SignInUserName.Text;
        //    string pw = SignInPassWord.Password;

        //    var loadingDialog = new LoadingDialog();

        //    var result = DialogHost.Show(loadingDialog, "LoginDialog", delegate (object senders, DialogOpenedEventArgs args)
        //    {

        //        ThreadStart start = delegate ()
        //        {
        //            string url = $"https://api.bobdong.cn/time_manager/user/register?name={name}&pw={pw}";

        //            //var ReturnDatastr = NetHelper.HttpCall(url, null, HttpEnum.Get);

        //            //var ReturnDataObject = JsonHelper.Deserialize<ReturnData<User>>(ReturnDatastr);

        //            Mainthread.BeginInvoke((Action)delegate ()// 异步更新界面
        //            {

        //                args.Session.Close(false);
        //                //if (ReturnDataObject.code != 0)
        //                //{
        //                //    MessageTips(ReturnDataObject.message);
        //                //}
        //                //else
        //                //{
        //                //    MainStaticData.AccessToken = ReturnDataObject.data.access_token;
        //                //    Close();
        //                //}
        //                // 线程结束后的操作
        //            });

        //        };

        //        new Thread(start).Start(); // 启动线程

        //    });
        //}

        /// <summary>
        /// 读取最后登录的用户名与密码
        /// </summary>
        public void ReadLastUser(out string username, out string pwd)
        {
            username = pwd = string.Empty;
            var userPath = System.IO.Path.Combine(InfoDir, "user.json");
            if (!System.IO.File.Exists(userPath)) return;
            var user = JsonConvert.DeserializeObject<Tuple<string, string>>(System.IO.File.ReadAllText(userPath));
            username = user.Item1;
            pwd = user.Item2;
        }

        /// <summary>
        /// 保存登录的用户
        /// </summary>
        public async Task SaveLoginUserAsync(string username, string pwd)
        {
            var user = new Tuple<string, string>(username, pwd);
            var userStr = JsonConvert.SerializeObject(user);
            System.IO.File.WriteAllText(System.IO.Path.Combine(InfoDir, "user.json"), userStr);
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                await scope.Resolve<IUtilService>().SetLoginBusinessAsync(ApplicationObject.App.Business);
            }
        }
    }
}
