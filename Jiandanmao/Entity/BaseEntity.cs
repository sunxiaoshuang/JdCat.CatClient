using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Entity
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
    }
}
