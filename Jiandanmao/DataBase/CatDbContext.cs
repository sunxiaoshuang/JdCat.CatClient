using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.DataBase
{
    public class CatDbContext : DbContext
    {
        public CatDbContext() : base("CatDataBase")
        {
            Configure();
        }

        public CatDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelConfiguration.Configure(modelBuilder);
            var initializer = new CatDbInitializer(modelBuilder);
            Database.SetInitializer(initializer);
        }

        public DbSet<StoreOrder> StoreOrders { get; set; }
        public DbSet<StoreOrderProduct> StoreOrderProducts { get; set; }


    }
}
