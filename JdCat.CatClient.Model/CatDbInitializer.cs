using SQLite.CodeFirst;
using System.Data.Entity;

namespace JdCat.CatClient.Model
{
    public class CatDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<CatDbContext>
    {
        //public CatDbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder, typeof(CustomHistory)) { }
        public CatDbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder, typeof(string)) { }

        protected override void Seed(CatDbContext context)
        {

        }
    }
}
