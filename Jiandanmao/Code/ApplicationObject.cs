
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
using Autofac;
using System;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using JdCat.CatClient.Common;
using System.Threading;

namespace Jiandanmao.Code
{
    public class ApplicationObject : DependencyObject, INotifyPropertyChanged
    {
        public static ApplicationObject App;
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

        /// <summary>
        /// 本地打印机
        /// </summary>
        public ObservableCollection<Printer> Printers { get; } = new ObservableCollection<Printer>();
        /// <summary>
        /// 餐桌类别
        /// </summary>
        public ObservableCollection<DeskType> DeskTypes { get; } = new ObservableCollection<DeskType>();
        /// <summary>
        /// 所有餐桌
        /// </summary>
        public ObservableCollection<Desk> Desks { get; } = new ObservableCollection<Desk>();


        /// <summary>
        /// 菜单类别与菜品
        /// </summary>
        public ObservableCollection<ProductType> Types { get; } = new ObservableCollection<ProductType>();
        /// <summary>
        /// 所有菜品
        /// </summary>
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

        /// <summary>
        /// 登录的员工
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
        public void Init()
        {
            // 加载客户端数据
            LoadClientData();

            //// 加载打印机数据
            //await LoadPrinter();

            //// 初始化餐厅信息
            //await InitCatering();

            // 数据库初始化
            using (var scope = DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                service.InitDatabase(Business.Id);
            }
        }

        #region 本地数据操作

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
                // 默认不是主收银台，且接收外卖订单
                ClientData = new ClientData { IsHost = false, IsReceive = true };
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

        #endregion

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        public static void Print<T>(T order, int type = 0, PrintOption option = null) where T : class
        {
            var printers = App.Printers.Where(a => a.Device.State == 1);
            if (type != 0)
            {
                printers = printers.Where(a => a.Device.Type == type);
            }
            foreach (var printer in printers)
            {
                for (int i = 0; i < printer.Device.Quantity; i++)
                {
                    if (order is Order entity)
                    {
                        if ((printer.Device.Scope & ActionScope.Takeout) > 0)
                            printer.Print(entity);
                    }
                    else if (order is TangOrder tang)
                    {
                        if ((printer.Device.Scope & ActionScope.Store) > 0)
                            printer.Print(tang, option);
                    }
                }
            }
        }

        public async Task<JsonData> SyncDataAsync()
        {
            var result = new JsonData();
            try
            {
                var data = await Request.SynchronousAsync(App.Business.Id);
                using (var scope = DataBase.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IUtilService>();
                    await service.SaveStaffAsync(data.Staffs);
                    await service.SaveRemoteDataAsync(data.StaffPosts);
                    await service.SaveRemoteDataAsync(data.Payments);
                    await service.SaveRemoteDataAsync(data.DeskTypes);
                    await service.SaveRemoteDataAsync(data.Desks);
                    await service.SaveRemoteDataAsync(data.Marks);
                    await service.SaveRemoteDataAsync(data.Printers);
                    await service.SaveRemoteDataAsync(data.ProductTypes);
                    await service.SaveRemoteDataAsync(data.Products);
                    await service.SaveRemoteDataAsync(data.CookProductRelatives);
                    await service.SaveRemoteDataAsync(data.Booths);
                    await service.SaveRemoteDataAsync(data.BoothProductRelatives);
                }
                result.Success = true;
                result.Msg = "同步成功";
            }
            catch (Exception)
            {
                result.Msg = "同步失败，加载已缓存的数据";
            }
            await ReloadAsync();

            return result;
        }

        /// <summary>
        /// 重新加载内存数据
        /// </summary>
        /// <returns></returns>
        public async Task ReloadAsync()
        {
            using (var scope = DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                var orderService = scope.Resolve<IOrderService>();
                Types.Clear();
                Products.Clear();
                DeskTypes.Clear();
                Desks.Clear();
                // 产品列表
                (await service.GetProductTypeAsync())?.ForEach(a =>
                {
                    Types.Add(a);
                    a.Products?.ForEach(b => Products.Add(b));
                });
                // 产品库存
                (await service.GetProductStocksAsync())?.ForEach(a =>
                {
                    var product = Products.FirstOrDefault(b => b.Id == a.ProductId);
                    if (product == null) return;
                    product.Stock = a.Stock;
                });
                // 桌台
                (await service.GetDeskTypesAsync())?.ForEach(a =>
                {
                    DeskTypes.Add(a);
                    a.Desks?.ForEach(b => Desks.Add(b));
                });
                // 未完成订单
                orderService.GetUnfinishOrder()?.ForEach(order =>
                {
                    var desk = Desks.FirstOrDefault(a => a.Id == order.DeskId);
                    if (desk == null) return;
                    desk.Order = order;
                });
                service.PubSubscribe("SystemMessage", "ResetDeskStatus");
                var clientPrinters = await service.GetAllAsync<ClientPrinter>();
                // 将删除的打印机关闭
                var delPrinter = new List<Printer>();
                Printers.ForEach(a =>
                {
                    var printer = clientPrinters?.FirstOrDefault(b => a.Device.Id == b.Id);
                    if (printer != null) return;
                    a.Close();
                    delPrinter.Add(a);
                });
                delPrinter.ForEach(a => Printers.Remove(a));
                clientPrinters?.ForEach(item =>
                {
                    var printer = Printers.FirstOrDefault(a => a.Device.Id == item.Id);
                    if (printer == null)
                    {
                        printer = new Printer { Device = item };
                        Printers.Add(printer);
                    }
                    else
                    {
                        printer.Device = item;
                    }
                    if (printer.Device.State == 1)
                    {
                        printer.Open();
                    }
                    else
                    {
                        printer.Close();
                    }
                });

            }
        }

        private static object uploadLock = new object();
        /// <summary>
        /// 上传本地数据到服务器
        /// </summary>
        public static async Task UploadDataAsync()
        {
            await UploadOrderAsync();
            DeleteExpireOrder();
        }

        private async static Task UploadOrderAsync()
        {
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var list = service.GetAll<TangOrder>(EntityStatus.All)
                    ?.Where(a => !a.Sync && (a.OrderStatus & TangOrderStatus.Finish) > 0)
                    .ToList();
                if (list == null || list.Count == 0) return;
                await Request.UploadDataAsync(list);

                var products = new List<TangOrderProduct>();
                foreach (var item in list)
                {
                    var goods = service.GetOrderProduct(item.ObjectId);
                    if (goods == null) continue;
                    products.AddRange(goods);
                }
                await Request.UploadDataAsync(products);

                var payments = await service.GetRelativeEntitysAsync<TangOrderPayment, TangOrder>(list.Select(a => a.ObjectId).ToArray());
                await Request.UploadDataAsync(payments);


                service.SyncFinish(list);
                service.SyncFinish(products);
                service.SyncFinish(payments);
            }
        }
        /// <summary>
        /// 删除超过30天的订单
        /// </summary>
        private static void DeleteExpireOrder()
        {
            var paging = new PagingQuery { PageIndex = 1, PageSize = 50 };
            using (var scope = App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var count = service.Length<TangOrder>();
                var pageCount = Math.Ceiling(Convert.ToDouble(count) / Convert.ToDouble(paging.PageSize));
                var now = DateTime.Now;
                for (int i = 0; i < pageCount; i++)
                {
                    paging.PageIndex += i;
                    var orders = service.GetRange<TangOrder>(paging, EntityStatus.All);
                    if (orders == null) break;
                    orders.ForEach(order =>
                    {
                        if (order.CreateTime.Value.AddDays(30) > now) return;
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
