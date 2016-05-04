using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class MemoryPoolHandler
    {
        public class MemoryPoolProperties
        {
            public int BasicMemory { get; set; }
            public int AdditionalMemory { get; set; }
        }

        public MemoryPool SetupNew(Account owner, MemoryPoolProperties props)
        {
            MemoryPool memPool = new MemoryPool();
            memPool.Owner = owner;
            memPool.BasicSpace = props.BasicMemory;
            memPool.AdditionalSpace = props.AdditionalMemory;
            return memPool;
        }
    }
}
