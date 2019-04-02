using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentTypeService : BaseRedisService<PaymentType>, IPaymentTypeService
    {
        public PaymentTypeService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config) : base(connectionMultiplexer, config)
        {
        }

        public void Add(PaymentType type)
        {
            var codeKey = AddKeyPrefix("PaymentTypeCode", typeof(RedisEntity).Name);
            type.Code = $"M{Database.StringIncrement(codeKey).ToString().PadLeft(4, '0')}";
            type.Icon = SystemIcon.Other;
            type.TypeStatus = PaymentTypeStatus.Normal;
            Save(type);
        }

    }
}
