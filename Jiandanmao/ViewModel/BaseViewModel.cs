using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

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
        protected Dispatcher Mainthread = Dispatcher.CurrentDispatcher;

        public async Task ShowLoadingDialog(Task task)
        {
            var loadingDialog = new LoadingDialog();

            await DialogHost.Show(loadingDialog, "RootDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                async void start()
                {
                    await Mainthread.BeginInvoke((Action)async delegate ()
                    {
                        await task;
                        args.Session.Close(false);
                    });
                }

                new Thread(start).Start();
            });

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
