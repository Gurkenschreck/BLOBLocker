using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.WebApp.Models
{
    public class MemoryViewModel
    {
        public string PoolUniqueIdentifier { get; set; }
        public ICollection<AssignedMemory> AssignedMemory { get; set; }
        public int TotalPoolMemory { get; set; }
        public int FreeBasicMemory { get; set; }
        public int FreeAdditionalMemory { get; set; }

        [Display(Name="Additional memory")]
        [Range(0, int.MaxValue, ErrorMessage="Invalid range.")]
        public int AdditionalMemoryToAdd { get; set; }
        [Display(Name="Basic memory")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int BasicMemoryToAdd { get; set; }
    }
}