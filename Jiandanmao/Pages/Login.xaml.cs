using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Jiandanmao.Helper;
using Jiandanmao.Uc;
using System.Configuration;
using Jiandanmao.Code;
using Newtonsoft.Json;
using Jiandanmao.Model;

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
            ReadLastUser(out string name, out string password);
            NameTextBox.Text = name;
            PasswordBox.Password = password;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => {
                PasswordBox.Focus();
                PasswordBox.SelectAll();
            }));
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string pw = PasswordBox.Password?.Trim();
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
                    var result = await Request.Login(name, pw);
                    await Mainthread.BeginInvoke((Action)delegate ()
                    {
                        args.Session.Close(false);
                        if (result.Success)
                        {
                            var business = result.Data;
                            ApplicationObject.App.Business = business;
                            ApplicationObject.App.Init();
                            SaveLoginUser(name, pw);
                            LoginSuccess = true;
                            Close();
                        }
                        else
                        {
                            MessageTips(result.Msg);
                        }
                    });
                }

                new Thread(start).Start();
            });

        }

        private void LoginWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key==Key.Enter)
            //{
            //    this.Login_click(sender,e);
            //}
        }

        private void PasswordBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
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
        public void SaveLoginUser(string username, string pwd)
        {
            var user = new Tuple<string, string>(username, pwd);
            var userStr = JsonConvert.SerializeObject(user);
            System.IO.File.WriteAllText(System.IO.Path.Combine(InfoDir, "user.json"), userStr);
        }
    }
}
