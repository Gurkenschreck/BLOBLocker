using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class SpreadBaseContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAddition> Additions { get; set; }
        public DbSet<CryptoConfig> CryptoConfigurations { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Pool> Pools { get; set; }
        public DbSet<PoolShare> PoolShares { get; set; }

        public DbSet<Configuration> SystemConfigurations { get; set; }

        public SpreadBaseContext()
#if DEBUG
            : base(ConfigurationManager.ConnectionStrings["SpreadBaseEntitiesDev"].ConnectionString)
#endif
#if !DEBUG
            : base(ConfigurationManager.ConnectionStrings["SpreadBaseEntities"].ConnectionString)            
#endif
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasRequired(p => p.Config)
                .WithRequiredPrincipal();
        }
    }
}