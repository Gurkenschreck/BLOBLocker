using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPool.Entities.Models.Models.WebApp
{
    public class MemoryPool
    {
        public MemoryPool()
        {
            AssignedMemory = new List<AssignedMemory>();
        }
        [Key]
        public int ID { get; set; }
        public virtual Account Owner { get; set; }
        public int BasicSpace { get; set; }
        public int AdditionalSpace { get; set; }
        public virtual ICollection<AssignedMemory> AssignedMemory { get; set; }
    }
}
