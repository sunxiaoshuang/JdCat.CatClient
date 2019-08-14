using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;

using Jiandanmao.Enum;
using Jiandanmao.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiandanmao.Code
{
    public class Printer : IDisposable
    {
        private System.Windows.Controls.ListBox ctl = new System.Windows.Controls.ListBox();            // 确保打印时线程安全程连接
        private bool[] isStop = new bool[] { false };                                                   // 是否停止打印任务
        private List<object> _printQueue = new List<object>();

        private static Dictionary<string, object> dicLock = new Dictionary<string, object>();// 打印锁，同一个ip同一时间只能接受一个打印任务

        public ClientPrinter Device { get; set; }

        /// <summary>
        /// 打印商品中数量占用的纸张长度
        /// </summary>
        private const int QuantityLen = 6;
        /// <summary>
        /// 打印商品中价格占用的商品长度
        /// </summary>
        private const int PriceLen = 8;

        /// <summary>
        /// 规格对应的字符长度
        /// </summary>
        public int FormatLen
        {
            get
            {
                return Device.Format == 80 ? 48 : 32;
            }
        }

        /// <summary>
        /// 打印商品列表中名称的占用纸张长度
        /// </summary>
        public int NameLen
        {
            get
            {
                return FormatLen - QuantityLen - PriceLen;
            }
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public void Print(object order)
        {
            _printQueue.Add(order);
        }

        /// <summary>
        /// 打印二进制文本
        /// </summary>
        /// <param name="buffer"></param>
        public void Print(List<byte[]> buffers)
        {
            lock (GetLock(Device.IP))
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(Device.IP), Device.Port));
                foreach (var buffer in buffers)
                {
                    socket.Send(buffer);
                }
                socket.Close();
            }
        }

        /// <summary>
        /// 打印堂食订单
        /// </summary>
        /// <param name="order"></param>
        public void Print(TangOrder order, PrintOption option)
        {
            lock (GetLock(Device.IP))
            {
                try
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(IPAddress.Parse(Device.IP), Device.Port));
                    PrintOrder(order, socket, option);
                    socket.Close();
                }
                catch (SocketException ex)
                {
                    LogHelper.AddLog($"堂食订单打印异常：{ex.Message}");
                    throw new Exception($"打印机[{Device.Name}]连接异常");
                }
                Thread.Sleep(10);
            }
        }

        private Task PrintTask { get; set; }

        /// <summary>
        /// 开始打印任务
        /// </summary>
        public void Open()
        {
            isStop[0] = false;
            if (PrintTask != null && PrintTask.Status == TaskStatus.Running) return;
            PrintTask = Task.Run(() =>
            {
                while (true)
                {
                    if (isStop[0]) break;      // 是否停止任务
                    if (_printQueue.Count > 0)
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        try
                        {
                            lock (GetLock(Device.IP))
                            {
                                socket.Connect(new IPEndPoint(IPAddress.Parse(Device.IP), Device.Port));
                                while (true)
                                {
                                    var order = _printQueue.FirstOrDefault();
                                    if (order == null) break;
                                    //if (order.Products == null || order.Products.Count == 0) break;
                                    ctl.Dispatcher.Invoke(() =>
                                    {
                                        PrintOrder(order, socket);
                                    });
                                    _printQueue.Remove(order);
                                    Thread.Sleep(200);              // 每次打印一单，停顿200毫秒
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Log\\Error");
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }
                            var filepath = Path.Combine(dirPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                            var content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 打印出错，打印机[{Device.Name}]连接错误，原因：{ex}";
                            ctl.Dispatcher.Invoke(() =>
                            {
                                var stream = File.AppendText(filepath);
                                stream.WriteLine(content);
                                stream.Close();
                            });

                            // 打印失败后，线程等待2秒再开始执行打印任务
                            Thread.Sleep(2000);
                        }
                        finally
                        {
                            socket.Close();
                        }
                    }
                    else
                    {
                        // 每0.2秒轮询一次，查看是否有打印任务
                        Thread.Sleep(200);
                    }

                }
            });
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public object GetLock(string ip)
        {
            if (!dicLock.ContainsKey(ip))
            {
                dicLock.Add(ip, new object());
            }
            return dicLock[ip];
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void PrintOrder(object order, Socket socket)
        {
            if (Device.Type == 1)
            {
                if (order is Order entity)
                {
                    ReceptionPrint(entity, socket);
                }
                else if (order is ThirdOrder third)
                {
                    ReceptionPrint(third, socket);
                }
            }
            else if (Device.Type == 2)
            {
                if (order is Order entity)
                {
                    Backstage(entity, socket);
                }
                else if (order is ThirdOrder third)
                {
                    Backstage(third, socket);
                }
            }
        }

        /// <summary>
        /// 打印订单（堂食）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void PrintOrder(TangOrder order, Socket socket, PrintOption option)
        {
            if (Device.Type == 1)
            {
                ReceptionPrint(order, socket, option);
            }
            else if (Device.Type == 2)
            {
                Backstage(order, socket, option);
            }
        }
        #region 前台打印

        /// <summary>
        /// 前台打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void ReceptionPrint(Order order, Socket socket)
        {
            var bufferArr = new List<byte[]>();
            // 打印当日序号
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(3));
            bufferArr.Add(TextToByte("简单猫 #" + order.Identifier));
            //bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            //bufferArr.Add(TextToByte("简单猫"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印小票类别
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(TextToByte("前台小票"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("-", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 备注
            if (!string.IsNullOrEmpty(order.Remark))
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte($"备注：{order.Remark}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 开票信息
            if (!string.IsNullOrEmpty(order.InvoiceTax) && !string.IsNullOrEmpty(order.InvoiceName))
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte($"开票信息：{order.InvoiceName}，{order.InvoiceTax}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 商户名称
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(ApplicationObject.App.Business.Name));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 下单时间
            bufferArr.Add(TextToByte($"下单时间：{order.PayTime:yyyy-MM-dd HH:mm:ss}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 订单编号
            bufferArr.Add(TextToByte($"订单编号：{order.OrderCode}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 商品分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "购买商品", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印商品
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            foreach (var product in order.Products)
            {
                var buffer = ProductLine(product, 2);
                buffer.ForEach(a =>
                {
                    bufferArr.Add(a);
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                });
                if (product.Tag1 != null && product.Tag1.Count > 0)
                {
                    bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
                    // 套餐打印明细
                    product.Tag1.ForEach(a =>
                    {
                        bufferArr.Add($"  - {a.Name}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                    });
                    bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                }
            }
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "其他", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 包装费
            if (order.PackagePrice.HasValue)
            {
                bufferArr.Add(PrintLineLeftRight("包装费", double.Parse(order.PackagePrice + "") + ""));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 配送费
            bufferArr.Add(PrintLineLeftRight("配送费", double.Parse(order.Freight + "") + ""));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 满减活动打印
            if (order.SaleFullReduce != null)
            {
                bufferArr.Add(PrintLineLeftRight(order.SaleFullReduce.Name, "-￥" + double.Parse(order.SaleFullReduce.ReduceMoney + "") + ""));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 优惠券打印
            if (order.SaleCouponUser != null)
            {
                bufferArr.Add(PrintLineLeftRight(order.SaleCouponUser.Name, "-￥" + order.SaleCouponUser.Value + ""));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 订单金额
            bufferArr.Add(PrinterCmdUtils.AlignRight());
            bufferArr.Add(TextToByte("实付："));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.Price + "元"));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("*", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 地址
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.ReceiverAddress));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.Phone));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.ReceiverName));
            bufferArr.Add(PrinterCmdUtils.NextLine());

            // 切割
            bufferArr.Add(PrinterCmdUtils.FeedPaperCutAll());

            // 打印
            bufferArr.ForEach(a => socket.Send(a));
        }

        /// <summary>
        /// 前台打印（堂食订单）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void ReceptionPrint(TangOrder order, Socket socket, PrintOption option)
        {
            var bufferArr = new List<byte[]>
            {
                // 打印当日序号
                PrinterCmdUtils.AlignCenter(),
                PrinterCmdUtils.FontSizeSetBig(2),
                TextToByte(ApplicationObject.App.Business.Name),
                PrinterCmdUtils.NextLine(),
                TextToByte($"当日流水：{order.Identifier}"),
                PrinterCmdUtils.NextLine(),
                TextToByte(option.Title??"结账单"),
                PrinterCmdUtils.NextLine()
            };
            // 餐桌
            if (order.DeskId != null)
            {
                bufferArr.Add(PrinterCmdUtils.AlignLeft());
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight($"餐桌：{order.DeskName}", $"人数：{order.PeopleNumber}", fontSize: 2));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            // 订单编号
            bufferArr.Add(TextToByte($"订单编号：{order.Code}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 下单
            bufferArr.Add(TextToByte($"下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 服务员
            bufferArr.Add(TextToByte($"收银员：{order.StaffName}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 备注
            if (!string.IsNullOrEmpty(order.Remark))
            {
                bufferArr.Add(TextToByte($"备注：{order.Remark}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 商品分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "购买商品", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印商品
            foreach (var product in option.Products)
            {
                var buffer = ProductLine(product);
                buffer.ForEach(a =>
                {
                    bufferArr.Add(a);
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                });
            }
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("-", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 金额
            bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("餐位费：", order.MealFee.ToString()));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("原价：", order.OriginalAmount.ToString()));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("优惠：", (order.OriginalAmount - order.Amount + order.PreferentialAmount).ToString()));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("实收：", order.ActualAmount.ToString()));
            bufferArr.Add(PrinterCmdUtils.NextLine());

            //if (!string.IsNullOrEmpty(order.PaymentTypeName))
            //{
            //    bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("付款方式：", order.PaymentTypeName));
            //    bufferArr.Add(PrinterCmdUtils.NextLine());
            //}
            if (order.TangOrderPayments != null && order.TangOrderPayments.Count > 0)
            {
                if (order.TangOrderPayments.Count == 1)
                {
                    var first = order.TangOrderPayments.First();
                    bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("付款方式：", first.Name));
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                }
                else
                {
                    var first = order.TangOrderPayments.First();
                    bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("付款方式：", $"{first.Name}:{first.Amount}"));
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                    for (int i = 1; i < order.TangOrderPayments.Count; i++)
                    {
                        bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("", $"{order.TangOrderPayments[i].Name}:{order.TangOrderPayments[i].Amount}"));
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                    }
                }
            }
            if (!string.IsNullOrEmpty(order.PaymentRemark))
            {
                bufferArr.Add(PrinterCmdUtils.PrintLineLeftRight("支付备注：", order.PaymentRemark));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "其他", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印时间
            bufferArr.Add(TextToByte($"打印时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(ApplicationObject.App.Business.Name));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte($"电话：{ApplicationObject.App.Business.Mobile}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte($"地址：{ApplicationObject.App.Business.Address}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(TextToByte($"谢谢您的惠顾"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.NextLine());

            // 切割
            bufferArr.Add(PrinterCmdUtils.FeedPaperCutAll());

            // 打印
            bufferArr.ForEach(a => socket.Send(a));
        }

        /// <summary>
        /// 前台打印（第三方订单）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void ReceptionPrint(ThirdOrder order, Socket socket)
        {
            var bufferArr = new List<byte[]>();
            var sign = order.OrderSource == 0 ? "美团" : "饿了么";
            // 打印当日序号
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(3));
            bufferArr.Add(TextToByte(sign + " #" + order.DaySeq));
            //bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            //bufferArr.Add(TextToByte(sign));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            if (order.PrintTimes > 0)
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte("（补打）"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 打印小票类别
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(TextToByte("前台小票"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("-", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 备注
            if (!string.IsNullOrEmpty(order.Caution))
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte($"备注：{order.Caution}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 开票信息
            if (!string.IsNullOrEmpty(order.InvoiceTitle) && !string.IsNullOrEmpty(order.TaxpayerId))
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte($"开票信息：{order.InvoiceTitle}，{order.TaxpayerId}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 商户名称
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(ApplicationObject.App.Business.Name));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 下单时间
            bufferArr.Add(TextToByte($"下单时间：{order.Ctime:yyyy-MM-dd HH:mm:ss}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 订单编号
            bufferArr.Add(TextToByte($"订单编号：{order.OrderId}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 商品分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "购买商品", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印商品
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            foreach (var product in order.ThirdOrderProducts)
            {
                if (order.OrderSource == 1 && product.Name == "餐盒") continue;
                var buffer = ProductLine(product, 2);
                buffer.ForEach(a =>
                {
                    bufferArr.Add(a);
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                });
                if (product.Tag1 != null && product.Tag1.Count > 0)
                {
                    bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
                    // 套餐打印明细
                    product.Tag1.ForEach(a =>
                    {
                        bufferArr.Add($"  - {a.Name}".ToByte());
                        bufferArr.Add(PrinterCmdUtils.NextLine());
                    });
                    bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                }
            }
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "其他", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 包装费
            bufferArr.Add(PrintLineLeftRight("包装费", order.PackageFee + ""));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 配送费
            bufferArr.Add(PrintLineLeftRight("配送费", order.ShippingFee + ""));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 满减活动打印
            if (order.ThirdOrderActivities != null && order.ThirdOrderActivities.Count > 0)
            {
                foreach (var item in order.ThirdOrderActivities)
                {
                    bufferArr.Add(PrintLineLeftRight(item.Remark, "-￥" + item.ReduceFee));
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                }
            }
            // 订单金额
            bufferArr.Add(PrinterCmdUtils.AlignRight());
            bufferArr.Add(TextToByte("实付："));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.Amount + "元"));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("*", Device.Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 地址
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.RecipientAddress));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.RecipientPhone));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.RecipientName));
            bufferArr.Add(PrinterCmdUtils.NextLine());

            // 切割
            bufferArr.Add(PrinterCmdUtils.FeedPaperCutAll());

            // 打印
            bufferArr.ForEach(a => socket.Send(a));
        }

        #endregion


        /// <summary>
        /// 后台打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void Backstage<T>(T order, Socket socket, PrintOption option = null) where T : class
        {
            var printSign = string.Empty;
            if (order is TangOrder)
            {
                printSign = "Tang";
            }
            else if (order is ThirdOrder)
            {
                printSign = "Third";
            }
            var enumName = System.Enum.GetName(typeof(PrinterMode), Device.Mode);
            var type = System.Type.GetType($"Jiandanmao.Code.{printSign}{enumName}Print");
            if (order is Order)
            {
                ((BackstagePrint)Activator.CreateInstance(type, order, this, socket)).Print();
            }
            else if (order is TangOrder)
            {
                ((TangBackstagePrint)Activator.CreateInstance(type, order, this, socket, option)).Print();
            }
            else
            {
                ((ThirdBackstagePrint)Activator.CreateInstance(type, order, this, socket)).Print();
            }
        }

        /// <summary>
        /// 关闭打印任务
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 重新开始任务
        /// </summary>
        public void Restart()
        {
            this.Close();
            this.Open();
        }

        public void Dispose()
        {
            // 释放打印机资源
            if (PrintTask == null) return;
            isStop[0] = true;
            while (true)
            {
                if (PrintTask.Status == TaskStatus.RanToCompletion)
                {
                    PrintTask.Dispose();
                    PrintTask = null;
                    _printQueue.RemoveAll(a => true);
                    break;
                }
            }
        }

        /// <summary>
        /// 将内容转化为字节数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private byte[] TextToByte(string text)
        {
            return Encoding.GetEncoding("gbk").GetBytes(text ?? " ");
        }

        private int maxRightLen = 10;           // 打印商品时，商品数量与商品价格最多占10个字符位
        /// <summary>
        /// 打印订单商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<byte[]> ProductLine(OrderProduct product, int fontSize = 1)
        {
            var left = product.Name;
            if (!string.IsNullOrEmpty(product.Description))
            {
                left += "(" + product.Description + ")";
            }
            var middle = "*" + Convert.ToDouble(product.Quantity);
            var right = Convert.ToDouble(product.Price) + "";
            var place = string.Empty;
            for (int i = 0; i < maxRightLen - middle.Length - right.Length; i++)
            {
                place += " ";
            }
            right = middle + place + right;

            var buffer = PrinterCmdUtils.PrintLineLeftRight(left, right, fontSize: fontSize);
            return new List<byte[]> { buffer };
        }

        /// <summary>
        /// 打印订单商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<byte[]> ProductLine(TangOrderProduct product, int fontSize = 1)
        {
            var left = product.Name;
            if (!string.IsNullOrEmpty(product.Description))
            {
                left += "(" + product.Description + ")";
            }
            var middle = "*" + Convert.ToDouble(product.Quantity);
            var right = Convert.ToDouble(product.Amount) + "";
            var place = string.Empty;
            for (int i = 0; i < maxRightLen - middle.Length - right.Length; i++)
            {
                place += " ";
            }
            right = middle + place + right;

            var buffer = PrinterCmdUtils.PrintLineLeftRight(left, right, fontSize: fontSize);
            return new List<byte[]> { buffer };
        }

        /// <summary>
        /// 打印订单商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<byte[]> ProductLine(ThirdOrderProduct product, int fontSize = 1)
        {
            var left = product.Name;
            if (!string.IsNullOrEmpty(product.GetDesc()))
            {
                left += "(" + product.GetDesc() + ")";
            }
            var middle = "*" + Convert.ToDouble(product.Quantity);
            var right = Convert.ToDouble(product.Price * product.Quantity) + "";
            var place = string.Empty;
            for (int i = 0; i < maxRightLen - middle.Length - right.Length; i++)
            {
                place += " ";
            }
            right = middle + place + right;

            var buffer = PrinterCmdUtils.PrintLineLeftRight(left, right, fontSize: fontSize);

            return new List<byte[]> { buffer };
        }

        /// <summary>
        /// 打印左右对齐的行
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private byte[] PrintLineLeftRight(string left, string right, int fontSize = 1)
        {
            var zhLeft = PrinterCmdUtils.CalcZhQuantity(left);          // 左边文本的中文字符长度
            var enLeft = left.Length - zhLeft;          // 左边文本的其他字符长度
            var zhRight = PrinterCmdUtils.CalcZhQuantity(right);        // 右边文本的中文字符长度
            var enRight = right.Length - zhRight;       // 右边文本的其他字符长度
            var len = FormatLen - ((zhLeft * 2 + enLeft + zhRight * 2 + enRight) * fontSize);            // 缺少的字符长度
            if (len > 0)
            {
                for (int i = 0; i < len / fontSize; i++)
                {
                    left += " ";
                }
            }
            else
            {
                var times = 1;
                while (true)
                {
                    if (FormatLen * times + len > 0)
                    {
                        break;
                    }
                    times++;
                }
                for (int i = 0; i < (FormatLen * times + len) / fontSize; i++)
                {
                    left += " ";
                }
            }
            return TextToByte(left + right);
        }

    }
}
