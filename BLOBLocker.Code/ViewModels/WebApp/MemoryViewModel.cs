using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class MemoryViewModel
    {
        [Required]
        [MinLength(5),MaxLength(20)]
        public string PoolUniqueIdentifier { get; set; }

        public MemoryOverviewViewModel MemoryOverviewModel { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        [LocalizedDisplayName("MemoryPool.TotalPoolMemory")]
        public int TotalPoolMemory { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        [LocalizedDisplayName("MemoryPool.FreeBasicMemory")]
        public int FreeBasicMemory { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        [LocalizedDisplayName("MemoryPool.FreeAdditionalMemory")]
        public int FreeAdditionalMemory { get; set; }

        [Range(0, int.MaxValue, ErrorMessage="Invalid range.")]
        [LocalizedDisplayName("MemoryPool.AdditionalToAdd")]
        public int AdditionalMemoryToAdd { get; set; }
        [LocalizedDisplayName("MemoryPool.BasicToAdd")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int BasicMemoryToAdd { get; set; }

        public MemoryViewModel()
        {
            MemoryOverviewModel = new MemoryOverviewViewModel();
        }
    }
}