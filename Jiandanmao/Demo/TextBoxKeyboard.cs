using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Jiandanmao.Demo
{
    public class TextBoxKeyboard : TextBox
    {
        private static bool focus;
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            focus = true;
            var ps = Process.GetProcesses().Where(a => a.ProcessName.Contains("keyboard"));
            if (ps.Count() == 0)
            {
                var position = this.PointToScreen(new Point(0, 0));
                Process.Start("keyboard.exe", $"{position.X + 100} {position.Y + 50}");
            }
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            focus = false;
            Task.Run(() =>
            {
                Thread.Sleep(200);
                if (focus) return;
                var ps = Process.GetProcesses().Where(a => a.ProcessName.Contains("keyboard"));
                if (ps.Count() > 0)
                {
                    foreach (var item in ps)
                    {
                        item.Kill();
                    }
                }
            });
            base.OnLostFocus(e);
        }
    }
}
