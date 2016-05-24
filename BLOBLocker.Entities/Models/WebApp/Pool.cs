using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class Pool : IDataEntity
    {
        public Pool()
        {
            CreatedOn = DateTime.Now;
            IsActive = true;
            Participants = new List<PoolShare>();
            AssignedMemory = new List<AssignedMemory>();
            Messages = new List<Message>();
            ChatEnabled = true;
            FileStorageEnabled = true;
            LinkRepositoryEnabled = true;
        }
        [Key]
        public int ID { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [ForeignKey("Config")]
        public int ConfigID { get; set; }
        public virtual CryptoConfiguration Config { get; set; }

        public byte[] Salt { get; set; }

        [ForeignKey("Owner")]
        public int OwnerID { get; set; }
        public virtual Account Owner { get; set; }
        public virtual ICollection<AssignedMemory> AssignedMemory { get; set; }
        public virtual ICollection<PoolShare> Participants { get; set; }
        public virtual ICollection<StoredFile> Files { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public bool ChatEnabled { get; set; }
        public bool FileStorageEnabled { get; set; }
        public bool LinkRepositoryEnabled { get; set; }

        public int DefaultRights { get; set; }
    }
}