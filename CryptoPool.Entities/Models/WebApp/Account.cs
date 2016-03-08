﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.WebApp
{
    public class Account
    {
        public Account()
        {
            IsEnabled = true;
        }
        [Key]
        public int ID { get; set; }
        [Display(Name="Account_Alias",
            ResourceType = typeof(Resources.Models))]
        public string Alias { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password",
            ResourceType = typeof(Resources.Models))]
        public string Password { get; set; }
        [Display(Name = "Account_Salt",
            ResourceType = typeof(Resources.Models))]
        public byte[] Salt { get; set; }

        [ForeignKey("Config")]
        public int ConfigID { get; set; }
        public virtual CryptoConfiguration Config { get; set; }
        [ForeignKey("Addition")]
        public int AdditionID { get; set; }
        public virtual AccountAddition Addition { get; set; }
        [Display(Name = "Account_Roles",
            ResourceType = typeof(Resources.Models))]
        public virtual ICollection<AccountRoleLink> Roles { get; set; }
        [Display(Name="Account_Pools",
            ResourceType= typeof(Resources.Models))]
        public virtual ICollection<Pool> Pools { get; set; }
        [Display(Name = "Account_ForeignPools",
            ResourceType = typeof(Resources.Models))]
        public virtual ICollection<PoolShare> ForeignPools { get; set; }
        [Display(Name = "Account_IsEnabled",
            ResourceType = typeof(Resources.Models))]
        public bool IsEnabled { get; set; }
    }
}