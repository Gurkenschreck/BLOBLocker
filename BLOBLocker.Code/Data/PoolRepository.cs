using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class PoolRepository : IRepository<Pool>
    {
        BLWAContext context;

        public PoolRepository()
        {
            context = new BLWAContext();
        }

        public PoolRepository(BLWAContext context)
        {
            this.context = context;
        }

        ~PoolRepository()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public int Add(Pool entity)
        {
            context.Pools.Add(entity);
            return context.SaveChanges();
        }
        public Task<int> AddAsync(Pool entity)
        {
            context.Pools.Add(entity);
            return context.SaveChangesAsync();
        }

        public int AddOrUpdate(Pool entity)
        {
            if (Exists(entity))
            {
                context.Entry<Pool>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Pools.Add(entity);
            }
            return context.SaveChanges();
        }

        public Task<int> AddOrUpdateAsync(Pool entity)
        {
            if (Exists(entity))
            {
                context.Entry<Pool>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                context.Pools.Add(entity);
            }
            return context.SaveChangesAsync();
        }

        public int Delete(Pool entity)
        {
            context.Pools.Remove(entity);
            return context.SaveChanges();
        }

        public Task<int> DeleteAsync(Pool entity)
        {
            context.Pools.Remove(entity);
            return context.SaveChangesAsync();
        }

        public bool Exists(Pool entity)
        {
            return context.Pools.Any(p => p.ID == entity.ID);
        }

        public Pool Get(int id)
        {
            return context.Pools.FirstOrDefault(p => p.ID == id);
        }

        public Pool Get(Func<Pool, bool> predicate)
        {
            return context.Pools.FirstOrDefault(predicate);
        }
        public IEnumerable<Pool> GetMultiple(Func<Pool, bool> predicate)
        {
            return context.Pools.Where(predicate);
        }

        public Pool GetByKey(string puid)
        {
            return context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
        }

        public int Count
        {
            get
            {
                return context.Pools.Count();
            }
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
