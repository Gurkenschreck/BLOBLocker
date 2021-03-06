﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class SystemConfiguration
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
