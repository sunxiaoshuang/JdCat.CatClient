using Jiandanmao.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Jiandanmao.Code
{
    public static class Request
    {
        /// <summary>
        /// 请求服务地址
        /// </summary>
        public static string ApiUrl { get; set; }
        static Request()
        {
            ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> HttpRequest(string url, StringContent postData = null, string method = "GET")
        {
            method = method.ToUpper();
            using (var client = new HttpClient())
            {
                HttpResponseMessage res;
                if (method == "GET")
                {
                    res = await client.GetAsync(url);
                }
                else
                {
                    if (postData != null)
                    {
                        postData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }
                    res = await client.PostAsync(url, postData);
                }
                res.EnsureSuccessStatusCode();
                return await res.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<JsonData<T>> HttpRequest<T>(string url, StringContent postData = null, string method = "GET") where T : class, new()
        {
            var result = await HttpRequest(url, postData, method);
            return JsonConvert.DeserializeObject<JsonData<T>>(result);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<T> GetData<T>(string url, StringContent postData = null, string method = "GET") where T : class,new()
        {
            var result = await HttpRequest(url, postData, method);
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// 商户登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static async Task<JsonData<Business>> Login(string code, string pwd)
        {
            var url = $"{ApiUrl}/Client/Login?code={code}&pwd={pwd}";
            return await HttpRequest<Business>(url);
        }

        /// <summary>
        /// 获取商户订单列表
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="paging">分页参数</param>
        /// <returns></returns>
        public async static Task<List<Order>> GetOrders(Business business, PagingQuery paging)
        {
            var url = $"{ApiUrl}/Client/GetOrders/{business.ID}?PageIndex={paging.PageIndex}&PageSize={paging.PageSize}&CreateTime={DateTime.Now:yyyy-MM-dd}";
            var result = await HttpRequest(url);
            var jObj = JObject.Parse(result);
            var list = JsonConvert.DeserializeObject<List<Order>>(jObj["data"]["list"].ToString());
            paging.RecordCount = int.Parse(jObj["data"]["rows"].ToString());
            return list;
        }

        /// <summary>
        /// 获取订单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<Order> GetOrderDetail(int id)
        {
            var url = $"{ApiUrl}/Client/GetOrderDetail/{id}";
            var result = await HttpRequest<Order>(url);
            return result.Data;
        }
        
        /// <summary>
        /// 获取商户客户端打印机列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<List<Printer>> GetPrinters(int id)
        {
            var url = $"{ApiUrl}/Client/GetPrinters/{id}";
            var result = await HttpRequest<List<Printer>>(url);
            return result.Data;
        }

        /// <summary>
        /// 保存客户端打印机设置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="printers"></param>
        /// <returns></returns>
        public async static Task<JsonData<List<Printer>>> SavePrinters(int id, IEnumerable<Printer> printers)
        {
            var url = $"{ApiUrl}/Client/SavePrinters/{id}";
            foreach (var item in printers)
            {
                item.CopyFoodsToIds();
                if (!int.TryParse(item.Id, out int num))
                {
                    item.Id = "0";
                }
            }
            var content = JsonConvert.SerializeObject(printers);
            var postData = new StringContent(content);
            var result = await HttpRequest<List<Printer>>(url, postData, "POST");
            return result;
        }

        /// <summary>
        /// 保存客户端打印机设置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="printer"></param>
        /// <returns></returns>
        public async static Task<JsonData<Printer>> SavePrinter(int id, Printer printer)
        {
            var url = $"{ApiUrl}/Client/SavePrinter/{id}";
            printer.CopyFoodsToIds();
            if (!int.TryParse(printer.Id, out int num))
            {
                printer.Id = "0";
            }
            var content = JsonConvert.SerializeObject(printer);
            var postData = new StringContent(content);
            var result = await HttpRequest<Printer>(url, postData, "POST");
            return result;
        }

        /// <summary>
        /// 删除客户端打印机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<JsonData> DeletePrinter(int id)
        {
            var url = $"{ApiUrl}/Client/DeletePrinter/{id}";
            var result = await HttpRequest(url);
            return JsonConvert.DeserializeObject<JsonData>(result);
        }

        /// <summary>
        /// 获取商户菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<List<ProductType>> GetProducts(int id)
        {
            var url = $"{ApiUrl}/Client/GetProducts/{id}";
            var result = await GetData<List<ProductType>>(url);
            return result;
        }

        /// <summary>
        /// 更新打印机关联的菜品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async static Task<JsonData> PutPrinterProducts(int id, string ids)
        {
            var url = $"{ApiUrl}/Client/PutPrinterProducts/{id}?ids={ids}";
            var result = await GetData<JsonData>(url);
            return result;
        }

        /// <summary>
        /// 登录成功后，获取初始化数据
        /// </summary>
        /// <returns></returns>
        public async static Task<RemoteDataObject> GetInitData(int id)
        {
            var url = $"{ApiUrl}/Client/InitClient/{id}";
            return await GetData<RemoteDataObject>(url);
        }
        
        /// <summary>
        /// 保存餐桌区域
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async static Task<JsonData<DeskType>> SaveDeskType(DeskType type)
        {
            var url = $"{ApiUrl}/Client/SaveDeskType";
            var content = JsonConvert.SerializeObject(type);
            return await HttpRequest<DeskType>(url, new StringContent(content), "POST");
        }

        /// <summary>
        /// 保存餐桌区域
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async static Task<JsonData<DeskType>> UpdateDeskType(DeskType type)
        {
            var url = $"{ApiUrl}/Client/UpdateDeskType";
            var content = JsonConvert.SerializeObject(type);
            return await HttpRequest<DeskType>(url, new StringContent(content), "POST");
        }

        /// <summary>
        /// 删除餐桌区域
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async static Task<JsonData> DeleteDeskType(DeskType type)
        {
            var url = $"{ApiUrl}/Client/DeleteDeskType/{type.Id}";
            var result = await HttpRequest(url);
            return JsonConvert.DeserializeObject<JsonData>(result);
        }

        /// <summary>
        /// 保存餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        public async static Task<JsonData<Desk>> SaveDesk(Desk desk)
        {
            var url = $"{ApiUrl}/Client/SaveDesk";
            var content = JsonConvert.SerializeObject(desk);
            return await HttpRequest<Desk>(url, new StringContent(content), "POST");
        }

        /// <summary>
        /// 修改餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        public async static Task<JsonData<Desk>> UpdateDesk(Desk desk)
        {
            var url = $"{ApiUrl}/Client/UpdateDesk";
            var content = JsonConvert.SerializeObject(desk);
            return await HttpRequest<Desk>(url, new StringContent(content), "POST");
        }

        /// <summary>
        /// 删除餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        public async static Task<JsonData> DeleteDesk(Desk desk)
        {
            var url = $"{ApiUrl}/Client/DeleteDesk/{desk.Id}";
            var result = await HttpRequest(url);
            return JsonConvert.DeserializeObject<JsonData>(result);
        }

        #region 备注
        ///// <summary>
        ///// 根据订单编号获取订单
        ///// </summary>
        ///// <param name="code"></param>
        ///// <returns></returns>
        //public async static Task<Order> GetOrder(string code)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var res = await client.GetAsync(ApiUrl + $"/order/singleByCode?code={code}");
        //            var content = await res.Content.ReadAsStringAsync();
        //            var order = JsonConvert.DeserializeObject<Order>(content);
        //            return order;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
        ///// <summary>
        ///// 获取商品类型列表
        ///// </summary>
        ///// <param name="business"></param>
        ///// <returns></returns>
        //public async static Task<List<ProductType>> GetTypes(Business business)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var res = await client.GetAsync(ApiUrl + $"/product/types/{business.ID}");
        //        var content = await res.Content.ReadAsStringAsync();
        //        var list = JsonConvert.DeserializeObject<List<ProductType>>(content);
        //        return list;
        //    }
        //}

        ///// <summary>
        ///// 自动接单
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //public async static Task<JsonData> Recevice(Order order)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var res = await client.GetAsync(ApiUrl + $"/order/recevice/{order.ID}");
        //        var content = await res.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<JsonData>(content);
        //    }
        //}

        ///// <summary>
        ///// 打印订单
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //public static bool Print(Order order)
        //{
        //    Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    IPAddress ipAddress = IPAddress.Parse("192.168.0.87");
        //    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 9100);

        //    mySocket.Connect(ipEndPoint);
        //    mySocket.Send(PrinterCmdUtils.AlignCenter());
        //    mySocket.Send(PrinterCmdUtils.FontSizeSetBig(3));
        //    mySocket.Send(TextToByte("#3"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.AlignLeft());
        //    mySocket.Send(PrinterCmdUtils.FontSizeSetBig(1));
        //    mySocket.Send(TextToByte("商家小票"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.SplitLine("-"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.AlignCenter());
        //    mySocket.Send(PrinterCmdUtils.FontSizeSetBig(2));
        //    mySocket.Send(TextToByte("简单猫科技"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.AlignLeft());
        //    mySocket.Send(PrinterCmdUtils.FontSizeSetBig(1));
        //    mySocket.Send(TextToByte($"下单时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(TextToByte("订单编号：201807283486948349"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(TextToByte("--------------------购买商品--------------------"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(TextToByte("劲爆花甲"));
        //    mySocket.Send(PrinterCmdUtils.AlignCenter());
        //    mySocket.Send(TextToByte("* 1"));
        //    mySocket.Send(PrinterCmdUtils.AlignRight());
        //    mySocket.Send(TextToByte("12"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.AlignLeft());
        //    mySocket.Send(TextToByte("麻辣虾球"));
        //    mySocket.Send(PrinterCmdUtils.AlignCenter());
        //    mySocket.Send(TextToByte("* 2"));
        //    mySocket.Send(PrinterCmdUtils.AlignRight());
        //    mySocket.Send(TextToByte("36"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(TextToByte("----------------------其他----------------------"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.AlignLeft());
        //    mySocket.Send(TextToByte("配送费"));
        //    mySocket.Send(PrinterCmdUtils.AlignRight());
        //    mySocket.Send(TextToByte("5"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(TextToByte("------------------------------------------------"));
        //    mySocket.Send(PrinterCmdUtils.NextLine());
        //    mySocket.Send(PrinterCmdUtils.FeedPaperCutAll());
        //    mySocket.Close();
        //    return true;
        //}
        #endregion
    }
}
