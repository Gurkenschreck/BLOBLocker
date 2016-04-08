using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class PoolOverviewViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PUID { get; set; }
        public int OverallAssignedPoolSpace { get; set; }
        public ICollection<PoolShare> Participants { get; set; }
        public PoolShare CurrentPoolShare { get; set; }

        public bool IsChatEnabled { get; set; }
        public bool IsFileStorageEnabled { get; set; }
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
            Participants = corPool.Participants;
            PUID = corPool.UniqueIdentifier;
        }
    }
}