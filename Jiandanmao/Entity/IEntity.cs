using System;

namespace Jiandanmao.Entity
{
    public interface IEntity
    {
        int Id { get; set; }
        string ObjectId { get; set; }
        DateTime CreateTime { get; set; }
        bool IsSync { get; set; }
    }
}
