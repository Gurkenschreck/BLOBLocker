namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class BLWAConfiguration : DbMigrationsConfiguration<BLOBLocker.Entities.Models.WebApp.BLWAContext>
    {
        public BLWAConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BLOBLocker.Entities.Models.WebApp.BLWAContext context)
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
