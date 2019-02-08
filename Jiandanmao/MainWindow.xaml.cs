using Jiandanmao.Pages;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jiandanmao
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
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

            DataContext = new MainWindowViewModel();

            //Init();
            //CheckUpdate();
        }



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

        public void SetAccessToken(string accessToken)
        {

            //AccessToken = accessToken;
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
    }
}
