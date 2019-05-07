using JdCat.CatClient.Model;

using System.Collections.Generic;

namespace Jiandanmao.Code
{
    public class RemoteDataObject
    {
        public List<Staff> Staffs { get; set; }
        public List<StaffPost> StaffPosts { get; set; }
        public List<PaymentType> Payments { get; set; }
        public List<DeskType> DeskTypes { get; set; }
        public List<Desk> Desks { get; set; }
        public List<SystemMark> Marks { get; set; }
        public List<ClientPrinter> Printers { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public List<Product> Products { get; set; }
        public List<CookProductRelative> CookProductRelatives { get; set; }
        public List<StoreBooth> Booths { get; set; }
        public List<BoothProductRelative> BoothProductRelatives { get; set; }

    }
}
