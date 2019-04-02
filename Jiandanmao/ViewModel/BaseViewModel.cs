using Jiandanmao.Code;
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
using System.Windows.Input;
using System.Windows.Threading;

namespace Jiandanmao.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected Dispatcher Mainthread = Dispatcher.CurrentDispatcher;
        protected ISnackbarMessageQueue SnackbarMessageQueue;

        public ICommand SubmitCommand => new AnotherCommandImplementation(Submit);
        private object _submitParameter { get; set; }
        public object SubmitParameter
        {
            get
            {
                return _submitParameter;
            }
            set
            {
                _submitParameter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SubmitParameter"));
            }
        }

        /// <summary>
        /// 弹出框提醒
        /// </summary>
        /// <param name="message"></param>
        public void MessageTips(string message, string dialog = null)
        {
            var sampleMessageDialog = new MessageDialog
            {
                Message = { Text = message }
            };

            DialogHost.Show(sampleMessageDialog, dialog??"RootDialog");
        }
        /// <summary>
        /// 消息提醒
        /// </summary>
        /// <param name="message"></param>
        public void SnackbarTips(string message)
        {
            SnackbarMessageQueue?.Enqueue(message);
        }
        protected bool IsConfirm;
        public async Task Confirm(string message, string dialog = null)
        {
            var sampleMessageDialog = new ConfirmDialog
            {
                Message = { Text = message },
                DataContext = this
            };
            IsConfirm = false;
            await DialogHost.Show(sampleMessageDialog, dialog??"RootDialog");
        }
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

        public virtual void Submit(object o)
        {
            IsConfirm = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

    }
}
