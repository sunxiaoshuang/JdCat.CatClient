using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JdCatUpgrade
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAsync(0);
            await Task.Delay(500);
            var curDir = Environment.CurrentDirectory;
            var dir = System.IO.Path.Combine(curDir, "upgrade");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var host = ConfigurationManager.AppSettings["BackStageWebSite"];                                    // 升级文件主机地址
            // 当前软件版本号
            var version = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(curDir, "Jiandanmao.exe")).ProductVersion;

            await UpdateAsync(10);
            await Task.Delay(500);
            // 下载升级文件
            var zipPath = System.IO.Path.Combine(dir, "upgrade.zip");
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync($"{host}/upgrade/upgrade.zip");
                var buffer = await res.Content.ReadAsByteArrayAsync();
                using (var file = File.Open(zipPath, FileMode.OpenOrCreate))
                {
                    await file.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            await UpdateAsync(20);
            await Task.Delay(500);
            // 解压
            using (var zip = new FileStream(zipPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zip, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var name = entry.Name;
                        using (var stream = entry.Open())
                        {
                            var buffer = new List<byte>();
                            int b;
                            while ((b = stream.ReadByte()) != -1)
                            {
                                buffer.Add((byte)b);
                            }
                            var arr = buffer.ToArray();
                            using (var output = new FileStream($"{System.IO.Path.Combine(dir, name)}", FileMode.OpenOrCreate))
                            {
                                await output.WriteAsync(arr, 0, arr.Length);
                            }
                        }
                    }
                }
            }
            await UpdateAsync(50);
            await Task.Delay(500);
            // 删除压缩文件
            File.Delete(zipPath);
            await UpdateAsync(60);
            await Task.Delay(500);
            // 新软件版本
            var newVersion = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(dir, "Jiandanmao.exe")).ProductVersion;
            // 比较两个版本
            var oldArr = version.Split('.').Select(a => int.Parse(a)).ToList();
            var newArr = newVersion.Split('.').Select(a => int.Parse(a)).ToList();
            if (!(oldArr[0] < newArr[0] || oldArr[1] < newArr[1] || oldArr[2] < newArr[2]))
            {
                // 无需升级
                MessageBox.Show("已经是最新版本，无需升级！");
                Finish();
                return;
            }
            await UpdateAsync(70);
            await Task.Delay(500);
            // 覆盖新文件
            var filePaths = Directory.GetFiles(dir);
            foreach (var path in filePaths)
            {
                File.Copy(path, System.IO.Path.Combine(curDir, System.IO.Path.GetFileName(path)), true);
            }
            await UpdateAsync(100);
            await Task.Delay(500);
            Finish();
        }

        private void Finish()
        {
            Directory.Delete(System.IO.Path.Combine(Environment.CurrentDirectory, "upgrade"), true);
            Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, "Jiandanmao.exe"));
            this.Close();
        }
        private async Task UpdateAsync(double n)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate ()
            {
                num.Text = $"{n}%";
            });
        }
    }
}
