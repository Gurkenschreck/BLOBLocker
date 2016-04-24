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
    public class StringResource
    {
        public StringResource()
        {
            Base = "nt";
            LocalizedStrings = new List<LocalizedString>();
        }

        [Key]
        public int ID { get; set; }
        [Required]
        public string Key { get; set; }
        public TranslationType Type { get; set; }
        public string Base { get; set; }
        public virtual ICollection<LocalizedString> LocalizedStrings { get; set; }
        public string Comment { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
