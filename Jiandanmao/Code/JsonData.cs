using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Code
{
    public class JsonData
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Msg { get; set; }
    }
    public class JsonData<T> where T : class, new()
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Msg { get; set; }
    }
}
