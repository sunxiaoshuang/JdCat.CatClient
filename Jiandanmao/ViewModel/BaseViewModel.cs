using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Jiandanmao.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public void MessageTips(string message)
        {
            var sampleMessageDialog = new MessageDialog
            {
                Message = { Text = message }
            };

            DialogHost.Show(sampleMessageDialog, "RootDialog");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

    }
}
