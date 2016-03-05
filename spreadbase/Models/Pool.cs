using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class Pool
    {
        public Pool()
        {
            CreatedOn = DateTime.Now;
            IsActive = true;
            Participents = new List<PoolShare>();
        }
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        [ForeignKey("Config")]
        public int ConfigID { get; set; }
        public virtual CryptoConfig Config { get; set; }

        public byte[] Salt { get; set; }

        [ForeignKey("Owner")]
        public int OwnerID { get; set; }
        public virtual Account Owner { get; set; }
        public virtual ICollection<PoolShare> Participents { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}