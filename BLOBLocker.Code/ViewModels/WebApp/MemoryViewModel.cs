using BLOBLocker.Code.Attributes;
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

        public void Populate(Account curAcc, Pool curPool)
        {
            TotalPoolMemory = curPool.AssignedMemory.Where(p => p.IsEnabled).Select(p => p.Space).Sum();
            MemoryOverviewModel.PUID = curPool.UniqueIdentifier;
            MemoryOverviewModel.Rights = curAcc.PoolShares.First(p => p.PoolID == curPool.ID).Rights;
            MemoryOverviewModel.Memory = curPool.AssignedMemory.Where(p => p.IsEnabled).ToList();
            FreeBasicMemory = curAcc.MemoryPool.BasicSpace - curAcc
                .MemoryPool.AssignedMemory
                .Where(p => p.IsBasic && p.IsEnabled)
                .Select(p => p.Space).Sum();
            FreeAdditionalMemory = curAcc.MemoryPool.AdditionalSpace - curAcc
                .MemoryPool.AssignedMemory
                .Where(p => !p.IsBasic && p.IsEnabled)
                .Select(p => p.Space).Sum();
            PoolUniqueIdentifier = curPool.UniqueIdentifier;
        }
    }
}