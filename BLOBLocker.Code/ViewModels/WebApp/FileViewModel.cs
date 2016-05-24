using BLOBLocker.Code.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class FileViewModel
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
