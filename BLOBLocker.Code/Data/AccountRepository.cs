using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public sealed class AccountRepository : IRepository<Account>
    {
        BLWAContext context;

        public int Count
        {
            get
            {
                return context.Accounts.Count();
            }
        }

        public AccountRepository()
        {
            context = new BLWAContext();
        }

        public AccountRepository(BLWAContext context)
        {
            this.context = context;
        }

        ~AccountRepository()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public int Add(Account entity)
        {
            context.Accounts.Add(entity);
            return context.SaveChanges();
        }

        public Task<int> AddAsync(Account entity)
        {
            context.Accounts.Add(entity);
            return context.SaveChangesAsync();
        }

        public int AddOrUpdate(Account entity)
        {
            if (Exists(entity))
            {
                context.Entry<Account>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Accounts.Add(entity);
            }
            return context.SaveChanges();
        }

        public Task<int> AddOrUpdateAsync(Account entity)
        {
            if (Exists(entity))
            {
                context.Entry<Account>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Accounts.Add(entity);
            }
            return context.SaveChangesAsync();
        }

        public int Delete(Account entity)
        {
            DbSet<Account> dbset = context.Accounts;

            context.Accounts.Remove(entity);
            return context.SaveChanges();
        }

        public Task<int> DeleteAsync(Account entity)
        {
            context.Accounts.Remove(entity);
            return context.SaveChangesAsync();
        }

        public bool Exists(Account entity)
        {
            return context.Accounts.Any(p => p.ID == entity.ID);
        }

        public Account Get(int id)
        {
            return Get(p => p.ID == id);
        }
        
        public Account Get(Func<Account, bool> predicate)
        {
            return context.Accounts.FirstOrDefault(predicate);
        }

        public IEnumerable<Account> GetMultiple(Func<Account, bool> predicate)
        {
            return context.Accounts.Where(predicate);
        }

        public Account GetByKey(string alias)
        {
            return Get(p => p.Alias == alias);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
    }
}
