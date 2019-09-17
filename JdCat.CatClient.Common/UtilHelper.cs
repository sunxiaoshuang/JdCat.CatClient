using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Common
{
    public static class UtilHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                var data = memoryStream.ToArray();
                return data;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string MD5(string input, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
             var md5 = new MD5CryptoServiceProvider();
            var bytResult = md5.ComputeHash(encode.GetBytes(input));
            var strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult.ToLower();
        }

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

        ///// <summary>
        ///// 信息提示
        ///// </summary>
        ///// <param name="msg"></param>
        //public async static void MessageTip(string msg, string dialog = "RootDialog")
        //{
        //    await DialogHost.Show(new MessageDialog { Message = { Text = msg } }, dialog);
        //}

        /// <summary>
        /// 创建订单号
        /// </summary>
        /// <param name="id">指定订单的商户id</param>
        /// <returns></returns>
        public static string CreateOrderCode(int id)
        {
            var sign = id.ToString().PadLeft(5, '0');
            var code = DateTime.Now.ToString("yyyyMMddHHmmss");
            code += $"{sign}{GetRandom(5)}";
            return code;
        }

        /// <summary>
        /// 获取指定位数的随机码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetRandom(int num)
        {
            var random = new Random();
            var code = string.Empty;
            for (int i = 0; i < num; i++)
            {
                code += random.Next(0, 9);
            }
            return code;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="text">错误内容</param>
        public static void Log(string text)
        {
            var logfile = $"{DateTime.Now:yyyy-MM-dd}.txt";
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var filepath = Path.Combine(dirPath, logfile);
            var content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {text}";
            var stream = File.AppendText(filepath);
            stream.WriteLine(content);
            stream.Close();
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="buffer">错误内容</param>
        public static void ErrorLog(List<byte[]> buffer)
        {
            var text = string.Empty;
            foreach (var item in buffer)
            {
                text += item.ToStr();
            }
            Log(text);
        }



    }
}
