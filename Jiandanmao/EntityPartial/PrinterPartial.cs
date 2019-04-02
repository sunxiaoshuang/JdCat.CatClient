using JdCat.CatClient.Common;
using Jiandanmao.Extension;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.Code
{
    public partial class Printer
    {
        [JsonIgnore]
        public bool IsNew { get => Convert.ToInt32(Id) > 0; }
        [JsonIgnore]
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        [JsonIgnore]
        public ICommand ModifyCommand => new AnotherCommandImplementation(Modify);
        [JsonIgnore]
        public ICommand DeleteCommand => new AnotherCommandImplementation(Delete);
        [JsonIgnore]
        public ICommand SelectProductCommand => new AnotherCommandImplementation(SelectProduct);
        [JsonIgnore]
        public ICommand PrintTestCommand => new AnotherCommandImplementation(PrintTest);


        private async void Save(object o)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(IP) || Port == 0)
            {
                return;
            }
            var result = await Request.SavePrinter(ApplicationObject.App.Business.ID, this);
            Printer printer;
            if (int.Parse(Id) > 0)
            {
                printer = ApplicationObject.App.Printers.FirstOrDefault(a => a.Id == Id);
                printer.Name = result.Data.Name;
                printer.IP = result.Data.IP;
                printer.Port = result.Data.Port;
                printer.Type = result.Data.Type;
                printer.Mode = result.Data.Mode;
                printer.Format = result.Data.Format;
                printer.State = result.Data.State;
                printer.Quantity = result.Data.Quantity;
            }
            else
            {
                printer = result.Data;
                printer.Foods = new ObservableCollection<int>();
                ApplicationObject.App.Printers.Add(printer);
            }
            if (printer.State == 1)
            {
                printer.Restart();
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private async void Modify(object o)
        {
            await DialogHost.Show(new AddPrinter((Printer)o), "RootDialog");
        }
        private async void Delete(object o)
        {
            if (MessageBox.Show($"确定删除打印机【{Name}】吗", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            var result = await Request.DeletePrinter(Convert.ToInt32(Id));
            if (!result.Success)
            {
                MessageBox.Show("删除失败，请重试！");
                return;
            }
            var printer = ApplicationObject.App.Printers.FirstOrDefault(a => a.Id == Id);
            if (printer != null)
            {
                printer.Dispose();
                ApplicationObject.App.Printers.Remove(printer);
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private async void SelectProduct(object o)
        {
            var viewModel = new SelectProductViewModel(this);
            var dialog = new SelectProduct { DataContext = viewModel };
            await DialogHost.Show(dialog, "RootDialog");
            if (!viewModel.IsSubmit) return;
            if (viewModel.Types == null || viewModel.Types.Count == 0) return;
            Foods.Clear();
            viewModel.Types.ForEach(type => {
                if (type.Products == null || type.Products.Count == 0) return;
                type.Products.ForEach(product => {
                    if (!product.IsCheck) return;
                    Foods.Add(product.ID);
                });
            });
            FoodIds = JsonConvert.SerializeObject(Foods);
            await Request.PutPrinterProducts(Convert.ToInt32(Id), FoodIds);
        }
        private void PrintTest(object o)
        {
            if (string.IsNullOrEmpty(IP))return;
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));
                socket.Send(TextToByte("打印机测试成功"));
                socket.Send(PrinterCmdUtils.NextLine());
                socket.Send(PrinterCmdUtils.NextLine());
                socket.Send(PrinterCmdUtils.FeedPaperCutAll());
                socket.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
