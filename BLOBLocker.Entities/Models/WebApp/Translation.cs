using BLOBLocker.Entities.Models.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class Translation
    {
        public Translation()
        {
            Base = "nt";
            Translations = new List<LocalizedString>();
        }

        [Key]
        [Required]
        public string Key { get; set; }
        public TranslationType Type { get; set; }
        public string Base { get; set; }
        public virtual ICollection<LocalizedString> Translations { get; set; }
        public string Comment { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
