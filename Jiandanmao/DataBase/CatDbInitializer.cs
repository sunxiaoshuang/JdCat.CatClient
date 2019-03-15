using Jiandanmao.Entity;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace Jiandanmao.DataBase
{
    public class CatDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<CatDbContext>
    {
        public CatDbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder, typeof(CustomHistory)) { }

        protected override void Seed(CatDbContext context)
        {

        }
    }
}
