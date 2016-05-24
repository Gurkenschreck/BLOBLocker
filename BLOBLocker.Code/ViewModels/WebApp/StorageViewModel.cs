using BLOBLocker.Code.Data;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class StorageViewModel
    {
        public string PUID { get; set; }
        public IEnumerable<StoredFile> Files { get; set; }

        public double SizeInMP
        {
            get
            {
                double size = 0;
                foreach (var file in Files)
                {
                    size += file.FileSize;
                }
                    // byte / kbyte / mbyte
                return Math.Round((size / 1024) / 1024, 2);
            }
        }
    }
}
