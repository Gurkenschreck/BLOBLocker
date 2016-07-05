using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class StoredFile
    {
        [Key]
        public int ID { get; set; }

        public int OwnerID { get; set; }
        [ForeignKey("OwnerID")]
        public virtual Account Owner { get; set; }
        public virtual Pool Pool { get; set; }

        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
        public int FileSize { get; set; }
        public string FileSignature { get; set; }
        public string Description { get; set; }
        public DateTime? UploadedOn { get; set; }

        public string IPv4Address { get; set; }
        public string MD5Checksum { get; set; }
        public string SHA1Checksum { get; set; }

        public bool IsVisible { get; set; }
        public bool IsDeleted { get; set; }

        public StoringMode StoringMode { get; set; }

        public StoredFile()
        {
            StoringMode = WebApp.StoringMode.CompressedAndEncrypted;
            UploadedOn = DateTime.Now;
            IsVisible = true;
            IsDeleted = false;
        }
    }
}
