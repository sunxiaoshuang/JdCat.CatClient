using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Pages;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Jiandanmao
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        Dispatcher Mainthread = Dispatcher.CurrentDispatcher;
        public MainWindow()
        {
            // 初始化登录窗口
            Login loginDialog = new Login();

            loginDialog.ShowDialog();
            if (!loginDialog.LoginSuccess)
            {
                Close();
                return;
            }
            InitializeComponent();

            DataContext = new MainWindowViewModel(MainSnackbar.MessageQueue);

            Init();
            this.KeyDown += Window_KeyDown;
            //CheckUpdate();
        }

        private async void Init()
        {
            InitTimer();
            title.Text = ApplicationObject.App.Business.Name;
            if (!ApplicationObject.App.Config.IsCash)       // 不需要收银
            {
                btnSettle.Visibility = Visibility.Collapsed;
                btnChange.Visibility = Visibility.Collapsed;
                btnSync.Visibility = Visibility.Collapsed;
                btnUpload.Visibility = Visibility.Collapsed;
                await ApplicationObject.App.LoadRemotePrinterAsync();
                return;
            }
            InitUploadTimer();
            if (!ApplicationObject.App.IsAdmin && (ApplicationObject.App.Staff.StaffPost.Authority & StaffPostAuth.Manager) == 0)
            {
                btnSettle.Visibility = Visibility.Collapsed;
            }
            if (ApplicationObject.App.Staff == null)
            {
                btnChange.Visibility = Visibility.Collapsed;
            }
            await Task.Run(() =>
            {
                Thread.Sleep(500);
                Sync();
            });

        }

        #region 定时打印外卖订单

        private static DispatcherTimer readDataTimer = new DispatcherTimer();
        private static string orderUrl;
        private static bool isError = false;    // 记录是否出错
        private void InitTimer()
        {
            if (!ApplicationObject.App.ClientData.IsReceive) return;            // 如果不接收外卖订单，则直接退出
            readDataTimer.Tick += new EventHandler(HandleOrder);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 5);          // 5秒取一次
            orderUrl = string.Format(ApplicationObject.App.Config.OrderUrl, ApplicationObject.App.Business.Id);
            readDataTimer.Start();
        }
        private async void HandleOrder(object sender, EventArgs e)
        {
            try
            {
                var result = await Request.HttpRequestAsync(orderUrl);
                var json = JsonConvert.DeserializeObject<JsonData<JObject>>(result);
                // 本地订单
                var local = json.Data["local"];
                if (local.HasValues)
                {
                    var orders = new List<Order>();
                    foreach (string item in local)
                    {
                        orders.Add(JsonConvert.DeserializeObject<Order>(item));
                    }
                    var firstOrder = orders[0];
                    this.Dispatcher.Invoke(() =>
                    {
                        var filename = string.Empty;
                        if (firstOrder.Status == OrderStatus.Payed)
                        {
                            filename = "1.mp3";
                        }
                        else
                        {
                            filename = "2.mp3";
                        }
                        PlayMedia("Assets/Video/" + filename);
                    }, DispatcherPriority.Normal);
                    orders.ForEach(order =>
                    {
                        if (order.Products == null || order.Products.Count == 0) return;
                        ApplicationObject.Print(order);
                    });
                }
                // 第三方订单
                var third = json.Data["third"];
                if (third.HasValues)
                {
                    var orders = new List<ThirdOrder>();
                    foreach (string item in third)
                    {
                        orders.Add(JsonConvert.DeserializeObject<ThirdOrder>(item));
                    }
                    orders.ForEach(order =>
                    {
                        if (order.ThirdOrderProducts == null || order.ThirdOrderProducts.Count == 0) return;
                        ApplicationObject.Print(order);
                    });
                }
            }
            catch (Exception ex)
            {
                if (isError) return;
                //AutoClosingMessageBox.Show("读取新订单错误：" + ex.Message, "提示", 3000);
            }
        }

        MediaPlayer player = new MediaPlayer();
        /// <summary>
        /// 播放提示音
        /// </summary>
        /// <param name="path"></param>
        private void PlayMedia(string path)
        {
            player.Open(new Uri(path, UriKind.Relative));
            player.Volume = 1;
            player.Play();
            player.MediaEnded += (a, b) =>
            {
                player.Close();
            };
        }
        #endregion

        #region 定时上传客户端数据
        private static DispatcherTimer uploadTimer = new DispatcherTimer();
        private void InitUploadTimer()
        {
            if (!ApplicationObject.App.ClientData.IsHost) return;       // 如果不是主收银台，则直接退出
            uploadTimer.Interval = new TimeSpan(0, 0, 10, 0);          // 10分钟上传一次
            uploadTimer.Tick += async (sender, e) =>
            {
                try
                {
                    await ApplicationObject.UploadDataAsync();
                }
                catch (Exception ex)
                {
                    LogHelper.AddLog($"数据同步出错：{ex.Message}");
                }
            };
            uploadTimer.Start();
        }
        #endregion


        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;

        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageTips(((ButtonBase)sender).Content.ToString(), null, null);
        }



        private void ChangeWork_Click(object obj, RoutedEventArgs e)
        {
            DialogHost.Show(new ChangeWork { DataContext = new ChangeWorkViewModel(false) }, "RootDialog");
        }

        private void Settle_Click(object obj, RoutedEventArgs e)
        {
            DialogHost.Show(new ChangeWork { DataContext = new ChangeWorkViewModel(true) }, "RootDialog");
        }

        private void Sync_Click(object obj, RoutedEventArgs e)
        {
            Sync();
        }

        private async void Upload_Click(object obj, RoutedEventArgs e)
        {
            if (!ApplicationObject.App.ClientData.IsHost)
            {
                MessageBox.Show("只有主收银机才能上传数据");
                return;
            }
            await ApplicationObject.UploadDataAsync();
            MainSnackbar.MessageQueue.Enqueue("上传成功");
        }

        /// <summary>
        /// 同步远程数据
        /// </summary>
        private async void Sync()
        {
            await Mainthread.BeginInvoke((Action)async delegate ()
            {
                var dialog = new DialogSync();
                await DialogHost.Show(dialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
                {
                    async void start()
                    {
                        await Mainthread.BeginInvoke((Action)async delegate ()
                        {
                            await Task.Delay(1000);
                            var result = await ApplicationObject.App.SyncDataAsync();
                            MainSnackbar.MessageQueue.Enqueue(result.Msg);

                            args.Session.Close();
                        });
                    }

                    new Thread(start).Start();
                });
            });
        }

        /// <summary>
        /// 设为开机启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStart_Click(object sender, RoutedEventArgs e)
        {
            string starupPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Jiandanmao.exe");


            RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            try
            {
                rgkRun.SetValue("Jiandanmao", starupPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



            //RegistryKey run = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            //loca.Close();
            //run.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 500毫秒后提醒
        /// </summary>
        /// <param name="message"></param>
        public void Tips(string message)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
            }).ContinueWith(t =>
            {
                MainSnackbar.MessageQueue.Enqueue(message);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// 弹出框提示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void MessageTips(string message, object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new MessageDialog
            {
                Message = { Text = message }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");

        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("Update" + ".exe");
            }
            catch (Exception ex)
            {

            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (this.WindowStyle == WindowStyle.None)//全屏
                {
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

    }
}
