﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class AssignedMemory
    {
        public AssignedMemory()
        {
            DateAssigned = DateTime.Now;
            IsBasic = true;
            IsEnabled = true;
        }
        [Key]
        public int ID { get; set; }
        [ForeignKey("MemoryPool")]
        public int MemoryPoolID { get; set; }
        public virtual MemoryPool MemoryPool { get; set; }
        public Nullable<DateTime> DateAssigned { get; set; }
        public int Space { get; set; }
        public bool IsBasic { get; set; }
        public bool IsEnabled { get; set; } // Create enum with assignment state .Assigned, .WithdrawQueue, .Withdrawed or similar
    }
}
