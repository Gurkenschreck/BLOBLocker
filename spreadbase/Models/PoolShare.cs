using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class PoolShare
    {
        public PoolShare()
        {
            IsActive = true;
            SharedOn = DateTime.Now;
            VisibleFrom = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }
        [ForeignKey("Pool")]
        public int PoolID { get; set; }
        public virtual Pool Pool { get; set; }
        [ForeignKey("SharedWith")]
        public int SharedWithID { get; set; }
        public virtual Account SharedWith { get; set; }
        public byte[] SharedKey { get; set; }
        public Nullable<DateTime> SharedOn { get; set; }
        public Nullable<DateTime> VisibleFrom { get; set; }
        public bool IsActive { get; set; }
    }
}