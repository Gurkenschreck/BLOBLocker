﻿using CryptoPool.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.WebApp
{
    public class AccountRole
    {
        public int ID { get; set; }
        [Display(Name = "AccountRole_RoleDefinition",
            ResourceType = typeof(Resources.Models))]
        public string Definition { get; set; }
    }

    public class AccountRoleLink
    {
        public int ID { get; set; }
        public virtual Account Account { get; set; }
        public virtual AccountRole Role { get; set; }
    }
}