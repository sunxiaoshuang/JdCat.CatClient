using Jiandanmao.Extension;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Jiandanmao.Entity;

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


        /// <summary>
        /// 本地打印机
        /// </summary>
        public ObservableCollection<Printer> Printers
        {
            get { return (ObservableCollection<Printer>)GetValue(PrintersProperty); }
            set { SetValue(PrintersProperty, value); }
        }

        public static readonly DependencyProperty PrintersProperty =
            DependencyProperty.Register("Printers", typeof(ObservableCollection<Printer>), typeof(ApplicationObject));

        /// <summary>
        /// 餐桌类别
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; set; }

        public static readonly DependencyProperty DeskTypesProperty =
            DependencyProperty.Register("DeskTypes", typeof(ObservableCollection<DeskType>), typeof(ApplicationObject));

        /// <summary>
        /// 所有餐桌
        /// </summary>
        public ObservableCollection<Desk> Desks
        {
            get
            {
                if (DeskTypes == null) return null;
                var desks = new ObservableCollection<Desk>();
                DeskTypes.ForEach(a => {
                    if (a.Desks == null) return;
                    a.Desks.ForEach(b => desks.Add(b));
                });
                return desks;
            }
        }

        public List<Product> Products { get; set; }

        /// <summary>
        /// 初始化应用数据
        /// </summary>
        public async Task Init()
        {
            // 打印机初始化
            Printers = new ObservableCollection<Printer>();
            if (Business == null) return;

            var printers = await Request.GetPrinters(Business.ID);
            if (printers == null || printers.Count == 0)
            {
                await LoadOldPrinters();
            }
            else
            {
                printers.ForEach(a => Printers.Add(a));
            }

            foreach (var printer in Printers)
            {
                if (printer.Foods == null)
                {
                    printer.Foods = new ObservableCollection<int>();
                }
                if (printer.State == 1)
                {
                    printer.Open();
                }
            }

            // 初始化门店数据
            var initData = await Request.GetInitData(Business.ID);
            DeskTypes = new ObservableCollection<DeskType>();
            initData.Desk.ForEach(a => {
                a.ReloadDeskQuantity();
                DeskTypes.Add(a);
            });
            Products = initData.Products;
        }

        /// <summary>
        /// 加载旧的打印设置
        /// </summary>
        private async Task LoadOldPrinters()
        {
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), PrinterDir);
            if (!Directory.Exists(dirPath)) return;
            var filepath = Path.Combine(dirPath, Business.ID + ".json");
            if (!File.Exists(filepath)) return;
            var data = File.ReadAllText(filepath);
            var printers = JsonConvert.DeserializeObject<List<Printer>>(data);
            var result = await Request.SavePrinters(Business.ID, printers);
            if (!result.Success)
            {
                await DialogHost.Show(new MessageDialog { Message = { Text = result.Msg } }, "RootDialog");
                return;
            }
            result.Data.ForEach(a => Printers.Add(a));
        }

        ///// <summary>
        ///// 保存打印机设置
        ///// </summary>
        //public void Save()
        //{
        //    if (Business == null) return;
        //    var dirPath = Path.Combine(Directory.GetCurrentDirectory(), PrinterDir);
        //    var filepath = Path.Combine(dirPath, Business.ID + ".json");
        //    if (Printers == null || Printers.Count == 0) return;
        //    var data = JsonConvert.SerializeObject(Printers);
        //    File.WriteAllText(filepath, data, Encoding.UTF8);
        //}

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        public static void Print(Order order, int type = 0)
        {
            var printers = App.Printers.Where(a => a.State == 1);
            if (type != 0)
            {
                printers = printers.Where(a => a.Type == type);
            }
            foreach (var printer in printers)
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
