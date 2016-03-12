using CryptoPool.Entities.Models.Models.WebApp;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoPool.WebApp.Models
{
    public class PoolConfigModel
    {
        public bool IsOwner { get; set; }
        public Pool Pool { get; set; }
        public Account Account { get; set; }
        public AssignedMemory BasicMemory { get; set; }
        public AssignedMemory AdditionalMemory { get; set; }
    }
}