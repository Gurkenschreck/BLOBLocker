using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class VirtualFile
    {
        [LocalizedDisplayName("File.Name")]
        public string FileName { get; set; }
        [LocalizedDisplayName("File.Extension")]
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        [LocalizedDisplayName("File.MimeType")]
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        [LocalizedDisplayName("File.IPv4Address")]
        public string IPv4Address { get; set; }
        [LocalizedDisplayName("File.Description")]
        public string Description { get; set; }
        [LocalizedDisplayName("File.MD5Checksum")]
        public string MD5Checksum { get; set; }
        [LocalizedDisplayName("File.SHA1Checksum")]
        public string SHA1Checksum { get; set; }
        [LocalizedDisplayName("File.Owner")]
        public string Owner { get; set; }
        [LocalizedDisplayName("File.UploadedOn")]
        public DateTime UploadedOn { get; set; }
        [LocalizedDisplayName("File.StoringMode")]
        public StoringMode StoringMode { get; set; }

        [LocalizedDisplayName("File.IsVisible")]
        public bool IsVisible { get; set; }

        [LocalizedDisplayName("File.FileSizeInByte")]
        public double FileSizeInByte 
        { 
            get; 
            set; 
        }

        [LocalizedDisplayName("File.FileSizeInKByte")]
        public double FileSizeInKB
        {
            get
            {
                if (FileSizeInByte != default(double))
                {
                    return Math.Round(FileSizeInByte / 1024, 2);
                }
                return default(double);
            }
        }

        [LocalizedDisplayName("File.FileSizeInMB")]
        public double FileSizeInMB
        {
            get
            {
                if (FileSizeInByte != default(double))
                {
                    return Math.Round(FileSizeInKB / 1024, 2);
                }
                return default(double);
            }
        }

        public VirtualFile()
        {
            IsVisible = true;
            UploadedOn = DateTime.Now;
        }
    }
}
