using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.Models.WebApp
{
    public class LocalizedString
    {
        public LocalizedString()
        {
            Translation = "nt";
        }
        [Key]
        public int ID { get; set; }
        [ForeignKey("TranslationOf")]
        public string TranslationKey { get; set; }
        public virtual Translation TranslationOf { get; set; }
        public string UICulture { get; set; }
        public string Translation { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
