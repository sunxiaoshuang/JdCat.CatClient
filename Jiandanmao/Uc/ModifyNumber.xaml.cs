using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Jiandanmao.Uc
{
    /// <summary>
    /// ModifyNumber.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyNumber : UserControl
    {
        public int Number { get; set; }

        public bool IsSubmit { get; set; }
        public ModifyNumber()
        {
            InitializeComponent();
            var main = Dispatcher;
            Task.Run(() => {
                Task.Delay(500);
                main.BeginInvoke((Action)delegate() {
                    txtNumber.Text = Number.ToString();
                    txtNumber.Focus();
                    txtNumber.SelectAll();
                });
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Number = int.Parse(txtNumber.Text);
            IsSubmit = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void TxtNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = (TextBox)sender;
            var text = txt.Text.Trim();
            if (!double.TryParse(text, out double distance))
            {
                var reg = Regex.Match(text, @"\d+");
                txt.Text = reg.Value;
                txt.SelectionStart = int.MaxValue;
                return;
            }
        }
    }
}
