using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class PoolOverviewViewModel
    {
        [LocalizedDisplayName("Pool.Title")]
        public string Title { get; set; }
        [LocalizedDisplayName("Pool.Description")]
        public string Description { get; set; }
        [LocalizedDisplayName("Pool.PUID")]
        public string PUID { get; set; }
        [LocalizedDisplayName("Pool.TotalAssignedMemory")]
        public int OverallAssignedPoolSpace { get; set; }
        [LocalizedDisplayName("Pool.Participants")]
        public ICollection<PoolShare> Participants { get; set; }
        public PoolShare CurrentPoolShare { get; set; }

        [LocalizedDisplayName("Pool.IsChatEnabled")]
        public bool IsChatEnabled { get; set; }
        [LocalizedDisplayName("Pool.IsFileStorageEnabled")]
        public bool IsFileStorageEnabled { get; set; }
        [LocalizedDisplayName("Pool.IsLinkRepoEnabled")]
        public bool IsLinkRepoEnabled { get; set; }

        public void Populate(Pool corPool)
        {
            Title = corPool.Title;
            OverallAssignedPoolSpace = (corPool.AssignedMemory.Count != 0) ? corPool.AssignedMemory
                                                                                    .Where(p => p.IsEnabled)
                                                                                    .Select(p => p.Space)
                                                                                    .Sum() : 0;
            IsChatEnabled = corPool.ChatEnabled;
            IsFileStorageEnabled = corPool.FileStorageEnabled;
            IsLinkRepoEnabled = corPool.LinkRepositoryEnabled;
            Participants = corPool.Participants.Where(p => p.IsActive).ToList();
            PUID = corPool.UniqueIdentifier;
        }
    }
}