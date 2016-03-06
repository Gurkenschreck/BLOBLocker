namespace CryptoPool.Entities.Models.Migrations.AT
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class AdminToolConfiguration : DbMigrationsConfiguration<CryptoPool.Entities.Models.AdminTool.AdminToolContext>
    {
        public AdminToolConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CryptoPool.Entities.Models.AdminTool.AdminToolContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
