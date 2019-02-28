using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Helper
{
    /// <summary>
    /// 通用操作类
    /// </summary>
    public static class UtilHelper
    {
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> Request(string url, string postData = null, string method = "GET")
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
                    var data = new StringContent(postData);
                    data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    res = await client.PostAsync(url, data);
                }
                res.EnsureSuccessStatusCode();
                return await res.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// 将对象转化为querystring
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetProperties<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (var item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    if (value == null)
                    {
                        continue;
                    }
                    tStr += $"{name}={value}&";
                }
            }
            tStr = tStr.Substring(0, tStr.Length - 1);
            return tStr;
        }

        /// <summary>
        /// 将字符串转化为二进制数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] TextToByte(string text)
        {
            return Encoding.Default.GetBytes(text);
        }

        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="msg"></param>
        public async static void MessageTip(string msg)
        {
            await DialogHost.Show(new MessageDialog { Message = { Text = msg } }, "RootDialog");
        }

    }
}
