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
    public class NewFileViewModel
    {
        [Required]
        public string PUID { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public string Description { get; set; }


        public ICollection<VirtualFile> Parse()
        {
            ICollection<VirtualFile> files = new List<VirtualFile>();

            foreach (var file in Files)
            {
                VirtualFile vf = new VirtualFile();
                vf.Description = Description;
                vf.FileName = file.FileName.Split('.')[0];
                vf.FileExtension = file.FileName.Split('.')[1];
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    vf.Content = binaryReader.ReadBytes((int)file.InputStream.Length);
                }
                vf.MimeType = file.ContentType;
                files.Add(vf);
            }

            return files;
        }
    }
}
