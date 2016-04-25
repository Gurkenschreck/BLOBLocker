using BLOBLocker.Entities.Models.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class Account : IDataEntity
    {
        public Account()
        {
            IsEnabled = true;
        }
        [Key]
        public int ID { get; set; }
        public string Alias { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public byte[] Salt { get; set; }

        [ForeignKey("Config")]
        public int ConfigID { get; set; }
        public virtual CryptoConfiguration Config { get; set; }
        [ForeignKey("Addition")]
        public int AdditionID { get; set; }
        public virtual AccountAddition Addition { get; set; }
        public virtual ICollection<AccountRoleLink> Roles { get; set; }
        public virtual ICollection<Pool> Pools { get; set; }
        public virtual ICollection<PoolShare> PoolShares { get; set; }
        public bool IsEnabled { get; set; }

        public virtual MemoryPool MemoryPool { get; set; }
    }
}