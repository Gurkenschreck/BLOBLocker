using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public sealed class AccountRepository : IRepository<Account>
    {
        BLWAContext context = new BLWAContext();

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

        public void Add(Account entity)
        {
            context.Accounts.Add(entity);
            context.SaveChanges();
        }

        public void AddOrUpdate(Account entity)
        {
            if (Exists(entity))
            {
                context.Entry<Account>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Accounts.Add(entity);
            }
            context.SaveChanges();
        }

        public void Delete(Account entity)
        {
            context.Accounts.Remove(entity);
            context.SaveChanges();
        }

        public int Count()
        {
            return context.Accounts.Count();
        }

        public bool Exists(Account entity)
        {
            return context.Accounts.FirstOrDefault(p => p.ID == entity.ID) != null;
        }

        public Account Get(int id)
        {
            return Get(p => p.ID == id);
        }
        public Account GetByKey(string alias)
        {
            return Get(p => p.Alias == alias);
        }

        public Account Get(Func<Account, bool> predicate)
        {
            return context.Accounts.FirstOrDefault(predicate);
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
