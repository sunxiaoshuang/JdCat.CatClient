using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JdCat.CatClient.Common
{
    public static class UtilExtend
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nameInstend"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value, bool nameInstend = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstend == true)
            {
                return name;
            }
            return attribute?.Description;
        }

        /// <summary>
        /// 循环
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return list;
        }

        /// <summary>
        /// 返回可通知列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> list)
        {
            if (list == null) return null;
            var result = new ObservableCollection<T>();
            foreach (var item in list)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 替换集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public static Collection<T> Replace<T>(this Collection<T> list, T item1, T item2)
        {
            var index = list.IndexOf(item1);
            list[index] = item2;
            return list;
        }

        /// <summary>
        /// 字符串转化为二进制数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByte(this string str)
        {
            return Encoding.GetEncoding("gbk").GetBytes(str);
        }

        /// <summary>
        /// 将时间转化为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime time)
        {
            var ts = time - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        /// <summary>
        /// 对象转化为JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 字符串转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
