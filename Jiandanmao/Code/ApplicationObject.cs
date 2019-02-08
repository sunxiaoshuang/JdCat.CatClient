using Jiandanmao.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jiandanmao.Code
{
    public class ApplicationObject : DependencyObject, INotifyPropertyChanged
    {
        public static ApplicationObject App;
        public const string PrinterDir = "Printer";
        static ApplicationObject()
        {
            App = new ApplicationObject();
        }
        protected ApplicationObject()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationConfig Config { get; set; }

        public Business Business
        {
            get { return (Business)GetValue(BusinessProperty); }
            set
            {
                SetValue(BusinessProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Business"));
            }
        }

        public static readonly DependencyProperty BusinessProperty =
            DependencyProperty.Register("Business", typeof(Business), typeof(ApplicationObject));



        public ObservableCollection<Order> Orders
        {
            get { return (ObservableCollection<Order>)GetValue(OrdersProperty); }
            set
            {
                SetValue(OrdersProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Orders"));
            }
        }

        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ObservableCollection<Order>), typeof(ApplicationObject));



        public ObservableCollection<Printer> Printers
        {
            get { return (ObservableCollection<Printer>)GetValue(PrintersProperty); }
            set { SetValue(PrintersProperty, value); }
        }

        public static readonly DependencyProperty PrintersProperty =
            DependencyProperty.Register("Printers", typeof(ObservableCollection<Printer>), typeof(ApplicationObject));

        /// <summary>
        /// 初始化应用数据
        /// </summary>
        public void Init()
        {
            if (Business == null) return;
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), PrinterDir);
            Directory.CreateDirectory(dirPath);
            var filepath = Path.Combine(dirPath, Business.ID + ".json");
            if (!File.Exists(filepath))
            {
                Printers = new ObservableCollection<Printer>();
                return;
            }
            var data = File.ReadAllText(filepath);
            Printers = JsonConvert.DeserializeObject<ObservableCollection<Printer>>(data);
            foreach (var printer in Printers)
            {
                if(printer.Foods == null)
                {
                    printer.Foods = new ObservableCollection<int>();
                }
                if (printer.State == 1)
                {
                    printer.Open();
                }
            }
        }

        /// <summary>
        /// 保存打印机设置
        /// </summary>
        public void Save()
        {
            if (Business == null) return;
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), PrinterDir);
            var filepath = Path.Combine(dirPath, Business.ID + ".json");
            if (Printers == null || Printers.Count == 0) return;
            var data = JsonConvert.SerializeObject(Printers);
            File.WriteAllText(filepath, data, Encoding.UTF8);
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        public static void Print(Order order)
        {
            foreach (var printer in App.Printers.Where(a => a.State == 1))
            {
                for (int i = 0; i < printer.Quantity; i++)
                {
                    printer.Print(order);
                }
            }
        }

    }

    public class ApplicationConfig
    {
        public string ApiUrl { get; set; }
        public string OrderUrl { get; set; }
        public string BackStageWebSite { get; set; }
    }
}
