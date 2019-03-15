using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jiandanmao.Demo
{
    /// <summary>
    /// Ctrl1.xaml 的交互逻辑
    /// </summary>
    public partial class Ctrl1 : UserControl
    {
        public Ctrl1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
            string path32 = @"C:\Program Files (x86)\Common Files\Microsoft Shared\Ink\TabTip32.exe";
            if (File.Exists(path))
            {
                Process.Start(path);
            }
            else if (File.Exists(path32))
            {
                Process.Start(path32);
            }
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private const Int32 WM_SYSCOMMAND = 274;
        private const UInt32 SC_CLOSE = 61536;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            IntPtr TouchhWnd = new IntPtr(0);
            TouchhWnd = FindWindow("IPTip_Main_Window", null);
            if (TouchhWnd == IntPtr.Zero)
                return;
            PostMessage(TouchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }
        MediaPlayer player = new MediaPlayer();
        int num = 1;
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (num > 4) num = 0;
            player.Open(new Uri($"Assets/Video/{num++}.mp3", UriKind.Relative));
            player.SpeedRatio = 1.0;
            player.Volume = 20;
            player.Balance = 20;
            player.Play();
            player.MediaEnded += (a, b) =>
            {
                player.Close();
            };
        }
    }
}
