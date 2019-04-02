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
using System;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.Common;

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

        private static readonly DependencyProperty BusinessProperty =
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

        private static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ObservableCollection<Order>), typeof(ApplicationObject));


        /// <summary>
        /// 本地打印机
        /// </summary>
        public ObservableCollection<Printer> Printers
        {
            get { return (ObservableCollection<Printer>)GetValue(PrintersProperty); }
            set { SetValue(PrintersProperty, value); }
        }

        private static readonly DependencyProperty PrintersProperty =
            DependencyProperty.Register("Printers", typeof(ObservableCollection<Printer>), typeof(ApplicationObject));

        /// <summary>
        /// 餐桌类别
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; set; }

        private static readonly DependencyProperty DeskTypesProperty =
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
                DeskTypes.ForEach(a =>
                {
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
        /// 所有菜品
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get
            {
                if (Types == null) return null;
                var products = new ObservableCollection<Product>();
                Types.ForEach(a =>
                {
                    a.Products?.ForEach(b => products.Add(b));
                });
                return products;
            }
        }

        /// <summary>
        /// 登录服务员
        /// </summary>
        public Staff Staff
        {
            get
            {
                return (Staff)GetValue(StaffProperty);
            }
            set
            {
                SetValue(StaffProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Staff"));
            }
        }
        private static readonly DependencyProperty StaffProperty = DependencyProperty.Register("Staff", typeof(Staff), typeof(ApplicationObject));
        /// <summary>
        /// 登录人是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }

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

            // 数据库初始化
            using (var scope = DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                service.InitDatabase(Business.ID);
            }
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
            initData.Desk.ForEach(a =>
            {
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
        /// 打印外卖订单
        /// </summary>
        /// <param name="order"></param>
        public static void Print<T>(T order, int type = 0, PrintOption option = null) where T : class
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
                    if (order is Order entity)
                    {
                        printer.Print(entity);
                    }
                    else if (order is TangOrder tang)
                    {
                        printer.Print(tang, option);
                    }
                }
            }

        }

        /// <summary>
        /// 上传本地数据到服务器
        /// </summary>
        public async static Task UploadData()
        {
            /* 同步流程
             * 1. 上传员工数据
             * 2. 上传支付类型
             * 3. 上传已完成的订单
             * 4. 删除7天前且已经上传的订单
             */
            await UploadStaff();
            await UploadPaymentType();
            await UploadOrder();
            DeleteExpireOrder();
        }

        private async static Task UploadStaff()
        {
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IStaffService>();
                var list = service.GetAll(EntityStatus.All)?.Where(a => !a.Sync).ToList();
                if (list == null || list.Count == 0) return;
                await Request.UploadData(list);
                service.SyncFinish(list);
            }
        }
        private async static Task UploadPaymentType()
        {
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IPaymentTypeService>();
                var list = service.GetAll(EntityStatus.All)?.Where(a => !a.Sync).ToList();
                if (list == null || list.Count == 0) return;
                await Request.UploadData(list);
                service.SyncFinish(list);
            }
        }
        private async static Task UploadOrder()
        {
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var list = service.GetAll(EntityStatus.All)
                    ?.Where(a => !a.Sync && (a.OrderStatus & TangOrderStatus.Finish) > 0)
                    .ToList();
                if (list == null || list.Count == 0) return;
                await Request.UploadData(list);
                var products = new List<TangOrderProduct>();
                foreach (var item in list)
                {
                    var goods = service.GetOrderProduct(item.ObjectId);
                    if (goods == null) continue;
                    products.AddRange(goods);
                }
                await Request.UploadData(products);
                service.SyncFinish(list);
                service.SyncFinish(products);
            }
        }
        private static void DeleteExpireOrder()
        {
            var paging = new JdCat.CatClient.Common.PagingQuery { PageIndex = 1, PageSize = 50 };
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var count = service.Length();
                var pageCount = Math.Ceiling(Convert.ToDouble(count) / Convert.ToDouble(paging.PageSize));
                var now = DateTime.Now;
                for (int i = 0; i < pageCount; i++)
                {
                    paging.PageIndex += i;
                    var orders = service.GetRange(paging, EntityStatus.All);
                    if (orders == null) break;
                    orders.ForEach(order => {
                        if (order.CreateTime.AddDays(7) > now) return;
                        service.DeleteOrder(order);
                    });
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
