using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.WebApp
{
    public class CryptoPoolContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAddition> Additions { get; set; }
        public DbSet<CryptoConfiguration> CryptoConfigurations { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Pool> Pools { get; set; }
        public DbSet<PoolShare> PoolShares { get; set; }

        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<Contact> ContactLinks { get; set; }

        public CryptoPoolContext()
#if DEBUG
            :base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CryptoPool.Entities.Models.CryptoPoolContext;Integrated Security=True")
#endif
#if !DEBUG
            :base(ConfigurationManager.ConnectionStrings["CryptoPoolEntities"].ConnectionString)
#endif
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pool>()
                .HasRequired(p => p.Owner)
                .WithMany(t => t.Pools)
                .HasForeignKey(a => a.OwnerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PoolShare>()
                .HasRequired(p => p.Pool)
                .WithMany(t => t.Participents)
                .HasForeignKey(a => a.PoolID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PoolShare>()
                .HasRequired(p => p.SharedWith)
                .WithMany(t => t.ForeignPools)
                .HasForeignKey(a => a.SharedWithID)
                .WillCascadeOnDelete(false);
        }
    }
}