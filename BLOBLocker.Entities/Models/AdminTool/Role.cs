using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.AdminTool
{
    public class Role
    {
        [Key]
        public int ID { get; set; }
        public string Definition { get; set; }
    }
}