using JdCat.CatClient.Common;
using JdCat.CatClient.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public static async Task<string> HttpRequestAsync(string url, StringContent postData = null, string method = "GET")
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
        public static async Task<JsonData<T>> HttpRequestAsync<T>(string url, StringContent postData = null, string method = "GET") where T : class, new()
        {
            var result = await HttpRequestAsync(url, postData, method);
            return JsonConvert.DeserializeObject<JsonData<T>>(result);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<T> GetDataAsync<T>(string url, StringContent postData = null, string method = "GET") where T : class, new()
        {
            var result = await HttpRequestAsync(url, postData, method);
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// 商户登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static async Task<JsonData<Business>> LoginAsync(string code, string pwd)
        {
            var url = $"{ApiUrl}/Client/Login?code={code}&pwd={pwd}";
            return await HttpRequestAsync<Business>(url);
        }

        /// <summary>
        /// 获取商户
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static async Task<JsonData<Business>> GetBusinessAsync(int id)
        {
            var url = $"{ApiUrl}/Client/GetBusiness/{id}";
            return await HttpRequestAsync<Business>(url);
        }

        /// <summary>
        /// 获取商户订单列表
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="paging">分页参数</param>
        /// <returns></returns>
        public async static Task<List<Order>> GetOrdersAsync(Business business, PagingQuery paging)
        {
            var url = $"{ApiUrl}/Client/GetOrders/{business.Id}?PageIndex={paging.PageIndex}&PageSize={paging.PageSize}&CreateTime={DateTime.Now:yyyy-MM-dd}";
            var result = await HttpRequestAsync(url);
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
        public async static Task<Order> GetOrderDetailAsync(int id)
        {
            var url = $"{ApiUrl}/Client/GetOrderDetail/{id}";
            var result = await HttpRequestAsync<Order>(url);
            return result.Data;
        }

        /// <summary>
        /// 获取商户客户端打印机列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<List<Printer>> GetPrintersAsync(int id)
        {
            var url = $"{ApiUrl}/Client/GetPrinters/{id}";
            var result = await HttpRequestAsync<List<Printer>>(url);
            return result.Data;
        }
        
        /// <summary>
        /// 获取商户菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<List<ProductType>> GetProductsAsync(int id)
        {
            var url = $"{ApiUrl}/Client/GetProducts/{id}";
            var result = await GetDataAsync<List<ProductType>>(url);
            return result;
        }
                
        /// <summary>
        /// 上传客户端数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public async static Task<JsonData<List<T>>> UploadDataAsync<T>(IEnumerable<T> list) where T : ClientBaseEntity, new()
        {
            var url = $"{ApiUrl}/Client/Upload{typeof(T).Name}";
            var body = new StringContent(JsonConvert.SerializeObject(list));
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await HttpRequestAsync<List<T>>(url, body, "POST");
            result.Data?.ForEach(a => {
                var entity = list.FirstOrDefault(b => a.ObjectId == b.ObjectId);
                if (entity == null) return;
                entity.Id = a.Id;
            });
            return result;
        }
        
        /// <summary>
        /// 使用员工登录
        /// </summary>
        /// <param name="id">员工id</param>
        /// <param name="pwd">员工密码</param>
        /// <returns></returns>
        public async static Task<JsonData<Staff>> LoginStaffAsync(int id, string pwd)
        {
            var url = $"{ApiUrl}/Client/LoginStaff/{id}?pwd={pwd}";
            return await HttpRequestAsync<Staff>(url);
        }

        /// <summary>
        /// 读取远程数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<RemoteDataObject> SynchronousAsync(int id)
        {
            var url = $"{ApiUrl}/Client/Synchronous/{id}";
            var result = await HttpRequestAsync(url);
            return JsonConvert.DeserializeObject<RemoteDataObject>(result);
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
