using Jiandanmao.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
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
        /// 商户登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static async Task<JsonData> Login(string code, string pwd)
        {
            var url = $"{ApiUrl}/Business/Login?username={code}&pwd={pwd}";
            var content = await HttpRequest(url);
            var data = JsonConvert.DeserializeObject<JsonData>(content);
            return data;
        }

        /// <summary>
        /// 获取商户订单列表
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="paging">分页参数</param>
        /// <returns></returns>
        public async static Task<List<Order>> GetOrders(Business business, PagingQuery paging)
        {
            using (var client = new HttpClient())
            {
                var param = new StringContent(JsonConvert.SerializeObject(paging));
                param.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.PostAsync(ApiUrl + $"/order/getOrderFromClient/0?businessId={ApplicationObject.App.Business.ID}&createTime={DateTime.Now:yyyy-MM-dd}", param);
                var content = await res.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(content);
                var list = JsonConvert.DeserializeObject<List<Order>>(jObj["data"]["list"].ToString());
                paging.RecordCount = int.Parse(jObj["data"]["rows"].ToString());
                double a1 = paging.RecordCount, a2 = paging.PageSize;
                paging.PageCount = (int)Math.Ceiling(a1 / a2);
                return list;
            }
        }
        /// <summary>
        /// 根据订单编号获取东单
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async static Task<Order> GetOrder(string code)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync(ApiUrl + $"/order/singleByCode?code={code}");
                    var content = await res.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<Order>(content);
                    return order;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取商品类型列表
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        public async static Task<List<ProductType>> GetTypes(Business business)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(ApiUrl + $"/product/types/{business.ID}");
                var content = await res.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ProductType>>(content);
                return list;
            }
        }

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

        /// <summary>
        /// 将字符串转化为二进制数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static byte[] TextToByte(string text)
        {
            return Encoding.Default.GetBytes(text);
        }
    }
}
