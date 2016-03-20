using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class Contact
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Account")]
        public int AccountID { get; set; }
        public virtual Account Account { get; set; }
    }
}