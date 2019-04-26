using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.ViewModel
{
    public class ChangeWorkViewModel : BaseViewModel
    {

        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand ConfirmChangeWorkCommand => new AnotherCommandImplementation(ConfirmChangeWork);


        #endregion

        #region 属性
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsManager { get; set; }
        /// <summary>
        /// 显示厨师数据
        /// </summary>
        public Visibility ShowCook { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string StaffName { get; set; }

        private string _payTime;
        /// <summary>
        /// 结账时间
        /// </summary>
        public string PayTime { get => _payTime; set => this.MutateVerbose(ref _payTime, value, RaisePropertyChanged()); }

        private string _startTime;
        /// <summary>
        /// 营业开始时间
        /// </summary>
        public string StartTime { get => _startTime; set => this.MutateVerbose(ref _startTime, value, RaisePropertyChanged()); }

        private string _endTime;
        /// <summary>
        /// 营业结束时间
        /// </summary>
        public string EndTime { get => _endTime; set => this.MutateVerbose(ref _endTime, value, RaisePropertyChanged()); }

        private string _orderIdentity;
        /// <summary>
        /// 单号
        /// </summary>
        public string OrderIdentity { get => _orderIdentity; set => this.MutateVerbose(ref _orderIdentity, value, RaisePropertyChanged()); }

        private int _orderCount;
        /// <summary>
        /// 单数
        /// </summary>
        public int OrderCount { get => _orderCount; set => this.MutateVerbose(ref _orderCount, value, RaisePropertyChanged()); }

        private int _peopleCount;
        /// <summary>
        /// 人数
        /// </summary>
        public int PeopleCount { get => _peopleCount; set => this.MutateVerbose(ref _peopleCount, value, RaisePropertyChanged()); }

        private double _productAmount;
        /// <summary>
        /// 商品（原价）金额
        /// </summary>
        public double ProductAmount { get => _productAmount; set => this.MutateVerbose(ref _productAmount, value, RaisePropertyChanged()); }

        private double _mealFeesAmount;
        /// <summary>
        /// 餐位费
        /// </summary>
        public double MealFeesAmount { get => _mealFeesAmount; set => this.MutateVerbose(ref _mealFeesAmount, value, RaisePropertyChanged()); }

        private double _discountAmount;
        /// <summary>
        /// 折扣
        /// </summary>
        public double DiscountAmount { get => _discountAmount; set => this.MutateVerbose(ref _discountAmount, value, RaisePropertyChanged()); }

        private double _orderAmount;
        /// <summary>
        /// 总单价
        /// </summary>
        public double OrderAmount { get => _orderAmount; set => this.MutateVerbose(ref _orderAmount, value, RaisePropertyChanged()); }

        private double _cashAmount;
        /// <summary>
        /// 收取现金
        /// </summary>
        public double CashAmount { get => _cashAmount; set => this.MutateVerbose(ref _cashAmount, value, RaisePropertyChanged()); }

        private double _giveAmount;
        /// <summary>
        /// 找赎
        /// </summary>
        public double GiveAmount { get => _giveAmount; set => this.MutateVerbose(ref _giveAmount, value, RaisePropertyChanged()); }

        private double _preferentialAmount;
        /// <summary>
        /// 优惠
        /// </summary>
        public double PreferentialAmount { get => _preferentialAmount; set => this.MutateVerbose(ref _preferentialAmount, value, RaisePropertyChanged()); }

        private ObservableCollection<Tuple<string, int, double>> _payments;
        /// <summary>
        /// 收款
        /// </summary>
        public ObservableCollection<Tuple<string, int, double>> Payments { get => _payments; set => this.MutateVerbose(ref _payments, value, RaisePropertyChanged()); }

        private ObservableCollection<Tuple<string, double, double>> _cooks;
        /// <summary>
        /// 厨师
        /// </summary>
        public ObservableCollection<Tuple<string, double, double>> Cooks { get => _cooks; set => this.MutateVerbose(ref _cooks, value, RaisePropertyChanged()); }

        #endregion



        #region 界面绑定方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSettle">是否是结算报表</param>
        public ChangeWorkViewModel(bool isSettle)
        {
            StoreName = ApplicationObject.App.Business.Name;
            StaffName = ApplicationObject.App.Staff?.Name;
            if (isSettle)
            {
                Title = "结算报表";
                IsManager = true;
                ShowCook = Visibility.Visible;
            }
            else
            {
                Title = "交班报表";
                ShowCook = Visibility.Hidden;
            }
        }

        public void Loaded(object o)
        {
            Task.Run(async () =>
            {
                await Mainthread.BeginInvoke((Action)async delegate ()
                {
                    await DialogHost.Show(new LoadingDialog(), "WorkDialog", delegate (object sender, DialogOpenedEventArgs args)
                    {
                        async void start()
                        {
                            await Mainthread.BeginInvoke((Action)async delegate ()
                            {

                                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                                using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
                                {
                                    var service = scope.Resolve<IOrderService>();
                                    var orders = await service.GetAllAsync<TangOrder>();
                                    if (orders == null) { args.Session.Close(); return; }
                                    var query = orders.Where(a => (a.OrderStatus & TangOrderStatus.Finish) > 0 && a.PayTime >= today && a.PayTime < DateTime.Now);
                                    if (!IsManager)
                                    {
                                        query = query.Where(a => a.StaffId == ApplicationObject.App.Staff.Id);
                                    }
                                    orders = query.ToList();
                                    if (!orders.Any()) { args.Session.Close(); return; }
                                    // 订单数据
                                    OrderIdentity = $"{orders[0].Identifier.ToString().PadLeft(4, '0')}-{orders.Last().Identifier.ToString().PadLeft(4, '0')}";
                                    orders = orders.OrderBy(a => a.PayTime).ToList();
                                    StartTime = orders[0].PayTime?.ToString("yyyy-MM-dd HH:mm:ss");
                                    EndTime = orders.Last().PayTime?.ToString("yyyy-MM-dd HH:mm:ss");
                                    PayTime = $"{orders[0].PayTime?.ToString("HH:mm")}-{orders.Last().PayTime?.ToString("HH:mm")}";
                                    OrderCount = orders.Count;
                                    PeopleCount = orders.Sum(a => a.PeopleNumber);
                                    // 营业数据
                                    ProductAmount = Math.Round(orders.Sum(a => a.OriginalAmount - a.MealFee), 2);
                                    MealFeesAmount = Math.Round(orders.Sum(a => a.MealFee), 2);
                                    var products = new List<TangOrderProduct>();
                                    foreach (var order in orders)
                                    {
                                        var product = service.GetOrderProduct(order.ObjectId).ToObservable();
                                        products.AddRange(product);
                                    }
                                    DiscountAmount = Math.Round(products.Where(a => a.ProductStatus != TangOrderProductStatus.Return).Sum(a => (a.OriginalPrice * a.Quantity) - a.Amount), 2);
                                    OrderAmount = Math.Round(orders.Sum(a => a.Amount), 2);
                                    PreferentialAmount = Math.Round(orders.Sum(a => a.PreferentialAmount), 2);
                                    // 收款数据
                                    var payments = await service.GetAllAsync<PaymentType>() ?? new List<PaymentType>();
                                    Payments = payments.Select(payment =>
                                    {
                                        var items = orders.Where(a => a.PaymentTypeId == payment.Id);
                                        if (!items.Any()) return null;
                                        if (payment.Category == PaymentCategory.Money)
                                        {
                                            CashAmount = Math.Round(items.Sum(a => a.ReceivedAmount));
                                            GiveAmount = Math.Round(items.Sum(a => a.GiveAmount));
                                        }
                                        return new Tuple<string, int, double>(payment.Name, items.Count(), Math.Round(items.Sum(a => a.Amount - a.PreferentialAmount), 2));
                                    }).Where(a => a != null).ToObservable();
                                    Payments.Add(new Tuple<string, int, double>("收款汇总", Payments.Sum(a => a.Item2), Math.Round(Payments.Sum(a => a.Item3), 2)));
                                    // 厨师（管理员才打印厨师报表）
                                    if (IsManager)
                                    {
                                        var staffs = await service.GetAllAsync<Staff>() ?? new List<Staff>();
                                        var posts = await service.GetAllAsync<StaffPost>() ?? new List<StaffPost>();
                                        staffs.ForEach(a => a.StaffPost = posts.FirstOrDefault(b => a.StaffPostId == b.Id));
                                        var cooks = staffs.Where(a => a.StaffPost != null && (a.StaffPost.Authority & StaffPostAuth.Cook) > 0)
                                            .Select(a => new Tuple<int, string>(a.Id, a.Name)).ToObservable();
                                        var relatives = await service.GetAllAsync<CookProductRelative>() ?? new List<CookProductRelative>();
                                        Cooks = cooks.Select(cook =>
                                        {
                                            var productIds = relatives.Where(a => a.StaffId == cook.Item1).Select(a => a.ProductId).ToList();
                                            var quantity = Math.Round(products.Where(a => productIds.Contains(a.ProductId)).Sum(a => a.Quantity), 2);
                                            var amount = Math.Round(products.Where(a => productIds.Contains(a.ProductId)).Sum(a => a.Amount), 2);
                                            return new Tuple<string, double, double>(cook.Item2, quantity, amount);
                                        }).ToObservable();
                                        Cooks.Add(new Tuple<string, double, double>("厨师合计", Cooks.Sum(a => a.Item2), Math.Round(Cooks.Sum(a => a.Item3), 2)));
                                    }

                                }

                                args.Session.Close();
                            });
                        }
                        new Thread(start).Start();
                    });
                });

            });
        }

        public async void ConfirmChangeWork(object o)
        {
            await DialogHost.Show(new LoadingDialog(), "WorkDialog", delegate (object sender, DialogOpenedEventArgs args)
            {
                void start()
                {
                    Mainthread.BeginInvoke((Action)delegate ()
                    {
                        var printer = ApplicationObject.App.Printers.FirstOrDefault(a => a.Device.Type == 1);
                        if (printer == null) return;

                        var bufferArr = new List<byte[]>
                        {
                            PrinterCmdUtils.AlignCenter(),
                            PrinterCmdUtils.FontSizeSetBig(2),
                            Title.ToByte(),
                            PrinterCmdUtils.NextLine(),
                            PrinterCmdUtils.FontSizeSetBig(1),
                            PrinterCmdUtils.SplitLine("-", printer.Device.Format),
                            PrinterCmdUtils.AlignLeft()
                        };
                        bufferArr.Add($"门店：{StoreName}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        if (!string.IsNullOrEmpty(StaffName))
                        {
                            bufferArr.Add($"员工：{StaffName}".ToByte());
                            bufferArr.Add(PrinterCmdUtils.NextLine());
                        }
                        bufferArr.Add($"开始时间：{StartTime}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add($"结束时间：{EndTime}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add($"打印时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.SplitLine("-", printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("营业额", OrderAmount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("订单数", OrderCount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("消费人数", PeopleCount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("收取现金", CashAmount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("找赎", GiveAmount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("优惠金额", PreferentialAmount.ToString(), printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.SplitText("-", "收款", printer.Device.Format));
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftMidRight("收款统计", "笔数", "金额"));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        foreach (var payment in Payments)
                        {
                            bufferArr.Add(PrinterCmdUtils.PrintLineLeftMidRight(payment.Item1, payment.Item2.ToString(), payment.Item3.ToString()));
                            bufferArr.Add(PrinterCmdUtils.NextLine());
                        }
                        if (IsManager)
                        {
                            bufferArr.Add(PrinterCmdUtils.SplitText("-", "厨师", printer.Device.Format));
                            bufferArr.Add(PrinterCmdUtils.PrintLineLeftMidRight("厨师", "产出数量", "产出金额"));
                            bufferArr.Add(PrinterCmdUtils.NextLine());
                            foreach (var cook in Cooks)
                            {
                                bufferArr.Add(PrinterCmdUtils.PrintLineLeftMidRight(cook.Item1, cook.Item2.ToString(), cook.Item3.ToString()));
                                bufferArr.Add(PrinterCmdUtils.NextLine());
                            }
                        }
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                        bufferArr.Add(PrinterCmdUtils.FeedPaperCutAll());

                        printer.Print(bufferArr);
                        args.Session.Close();
                    });
                }
                new Thread(start).Start();
            });
            if (!IsManager)
            {
                Process p = new Process();
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Jiandanmao.exe";
                p.StartInfo.UseShellExecute = false;
                p.Start();
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}