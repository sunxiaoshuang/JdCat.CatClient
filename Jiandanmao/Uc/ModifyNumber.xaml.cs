using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                Thread.Sleep(500);
                main.BeginInvoke((Action)delegate() {
                    txtNumber.Text = Number.ToString();
                    txtNumber.Focus();
                    txtNumber.SelectAll();
                });
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void TxtNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Submit();
        }

        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            e.Handled = re.IsMatch(e.Text);
        }

        private void Submit()
        {
            Number = int.Parse(txtNumber.Text);
            IsSubmit = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
