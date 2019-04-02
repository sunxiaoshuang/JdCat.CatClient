using JdCat.CatClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IPaymentTypeService : IBaseService<PaymentType>
    {
        void Add(PaymentType type);
    }
}
