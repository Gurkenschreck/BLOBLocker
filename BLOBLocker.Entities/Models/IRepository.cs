using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models
{
    public interface IRepository<T> : IDisposable
        where T : IDataEntity
    {
        int Count { get; }

        int Add(T entity);
        Task<int> AddAsync(T entity);
        int AddOrUpdate(T entity);
        Task<int> AddOrUpdateAsync(T entity);
        int Delete(T entity);
        Task<int> DeleteAsync(T entity);
        bool Exists(T entity);
        T Get(int id);
        T Get(Func<T, bool> predicate);
        IEnumerable<T> GetMultiple(Func<T, bool> predicate);
        T GetByKey(string key);
    }
}
