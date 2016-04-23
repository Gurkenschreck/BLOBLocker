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
        void Add(T entity);
        void AddOrUpdate(T entity);
        void Delete(T entity);
        bool Exists(T entity);
        T Get(int id);
        T GetByKey(string key);
        int Count();
    }
}
