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
            Status = TranslationStatus.New;
            LiveTranslation = "nt";
        }
        [Key]
        public int ID { get; set; }
        [ForeignKey("BaseResource")]
        public int BaseResourceKey { get; set; }
        public virtual StringResource BaseResource { get; set; }
        public string UICulture { get; set; }
        public string Translation { get; set; }
        public string LiveTranslation { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        public TranslationStatus Status { get; set; }
    }
}
