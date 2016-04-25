using BLOBLocker.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class AccountRole
    {
        public int ID { get; set; }
        public string Definition { get; set; }
    }
}