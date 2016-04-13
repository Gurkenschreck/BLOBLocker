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
        [Display(Name = "Additional memory")]
        [Required]
        [MinLength(5),MaxLength(20)]
        public string PoolUniqueIdentifier { get; set; }
        public ICollection<AssignedMemory> AssignedMemory { get; set; }
        [Display(Name = "Total Pool Memory")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int TotalPoolMemory { get; set; }
        [Display(Name = "Free Basic Memory")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int FreeBasicMemory { get; set; }
        [Display(Name = "Free Additional Memory")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int FreeAdditionalMemory { get; set; }

        [Display(Name="Additional memory")]
        [Range(0, int.MaxValue, ErrorMessage="Invalid range.")]
        public int AdditionalMemoryToAdd { get; set; }
        [Display(Name="Basic memory")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid range.")]
        public int BasicMemoryToAdd { get; set; }
    }
}