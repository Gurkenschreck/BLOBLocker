using BLOBLocker.Code.Membership;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class PoolConfigModel
    {
        public bool IsOwner { get; set; }
        public Pool Pool { get; set; }
        public Account Account { get; set; }
        public PoolShare PoolShare { get; set; }
        public AssignedMemory BasicMemory { get; set; }
        public AssignedMemory AdditionalMemory { get; set; }
        public RightsEditViewModel RightsEditViewModel { get; set; }
        public TitleDescriptionViewModel TitleDescriptionViewModel { get; set; }
        public ManageModulesViewModel ManageModulesViewModel { get; set; }

        public void Populate(Pool pool, Account curAcc)
        {
            Pool = pool;
            Account = curAcc;
            IsOwner = pool.OwnerID == curAcc.ID;
            PoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == pool.UniqueIdentifier);

            RightsEditViewModel = new RightsEditViewModel
            {
                PoolUID = pool.UniqueIdentifier,
                Rights = PoolRightHelper.GetRights(pool.DefaultRights)
            };

            TitleDescriptionViewModel = new TitleDescriptionViewModel
            {
                PUID = pool.UniqueIdentifier,
                Title = pool.Title,
                Description = pool.Description
            };

            ManageModulesViewModel = new ManageModulesViewModel
            {
                PUID = pool.UniqueIdentifier,
                EnableChat = pool.ChatEnabled,
                EnableFileStorage = pool.FileStorageEnabled,
                EnableLinkRepository = pool.LinkRepositoryEnabled
            };
        }
    }
}