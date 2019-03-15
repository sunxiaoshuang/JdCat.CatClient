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
using Autofac;
using Jiandanmao.DataBase;
using System;
using Jiandanmao.Redis;

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

        public ClientData ClientData { get; set; }

        public Autofac.IContainer DataBase { get; set; }

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

        /// <summary>
        /// 菜单类别与菜品
        /// </summary>
        public List<ProductType> Types { get; set; }

        /// <summary>
        /// 初始化应用数据
        /// </summary>
        public async Task Init()
        {
            // 加载客户端数据
            LoadClientData();

            // 加载打印机数据
            await LoadPrinter();

            // 初始化餐厅信息
            await InitCatering();
        }

        /// <summary>
        /// 加载客户端本地数据
        /// </summary>
        public void LoadClientData()
        {
            // 读取客户端数据
            var path = Path.Combine(Environment.CurrentDirectory, "Info", "clientdata.json");
            if (File.Exists(path))
            {
                ClientData = JsonConvert.DeserializeObject<ClientData>(File.ReadAllText(path));
            }
            else
            {
                ClientData = new ClientData { IsHost = true, IsReceive = true };
            }
        }
        /// <summary>
        /// 保存客户端本地数据
        /// </summary>
        public void SaveClientData()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Info", "clientdata.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(ClientData));
        }
        /// <summary>
        /// 加载打印机
        /// </summary>
        public async Task LoadPrinter()
        {
            Printers = new ObservableCollection<Printer>();
            if (Business == null) return;

            var printers = await Request.GetPrinters(Business.ID);
            if (printers == null || printers.Count == 0)
            {
                // 如果远程数据库中不存在打印机列表，则加载本地保存的打印机列表
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
                    printer.Open();         // 开启打印机监听
                }
            }
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
        /// <summary>
        /// 初始化堂食数据
        /// </summary>
        /// <returns></returns>
        private async Task InitCatering()
        {
            var initData = await Request.GetInitData(Business.ID);      // 读取远程数据库数据

            DeskTypes = new ObservableCollection<DeskType>();           // 餐桌
            initData.Desk.ForEach(a => {
                a.ReloadDeskQuantity();
                DeskTypes.Add(a);
            });
            Types = initData.Types;                                     // 菜单
            // 读取数据库本地数据库
            //using (var scope = DataBase.BeginLifetimeScope())
            //{
            //    // 获取正在使用中的订单，并绑定到餐桌上
            //    var service = scope.Resolve<ClientDbService>();
            //    if (service != null)
            //    {
            //        var unFinishOrder = service.GetUsingOrder(Business.ID);
            //        Desks.ForEach(a =>
            //        {
            //            var order = unFinishOrder.FirstOrDefault(b => b.DeskId == a.Id);
            //            if (order == null) return;
            //            a.Order = order;
            //        });
            //    }
            //}
            using (var scope = DataBase.BeginLifetimeScope())
            {
                // 读取堂食
                // todo

            }
        }
        


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
        public string RedisDefaultKey { get; set; }
        public string StaffPrefix { get; set; }
    }
}
