using System;
using System.Collections.Generic;
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
        }

        public int ID { get; set; }
        public virtual Pool Pool { get; set; }
        public virtual Account SharedWith { get; set; }
        public byte[] Key { get; set; }
        public Nullable<DateTime> SharedOn { get; set; }
        public Nullable<DateTime> VisibleFrom { get; set; }
        public bool IsActive { get; set; }
    }
}