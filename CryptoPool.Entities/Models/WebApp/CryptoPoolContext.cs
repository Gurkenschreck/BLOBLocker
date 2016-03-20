using CryptoPool.Entities.Models.Models.WebApp;
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

        public DbSet<MemoryPool> MemoryPools { get; set; }
        public DbSet<AssignedMemory> AssignedMemory { get; set; }

        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<Contact> ContactLinks { get; set; }
        public DbSet<Message> Messages { get; set; }

        public CryptoPoolContext()
#if DEBUG
            :base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CryptoPool.Entities.Models.WebApp.CryptoPoolContext;Integrated Security=True")
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
                .WithMany(t => t.Participants)
                .HasForeignKey(a => a.PoolID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PoolShare>()
                .HasRequired(p => p.SharedWith)
                .WithMany(t => t.PoolShares)
                .HasForeignKey(a => a.SharedWithID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AssignedMemory>()
                .HasRequired(p => p.MemoryPool)
                .WithMany(t => t.AssignedMemory)
                .HasForeignKey(q => q.MemoryPoolID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Account>()
                .HasRequired(p => p.MemoryPool)
                .WithRequiredPrincipal(t => t.Owner)
                .WillCascadeOnDelete(false);


        }
    }
}