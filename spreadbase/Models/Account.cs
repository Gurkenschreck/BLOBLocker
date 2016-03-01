using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
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
            ResourceType= typeof(Resources.Models.Models))]
        public string Alias { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password",
            ResourceType = typeof(Resources.Models.Models))]
        public string Password { get; set; }
        [Display(Name = "Account_Salt",
            ResourceType = typeof(Resources.Models.Models))]
        public byte[] Salt { get; set; }

        public virtual CryptoConfig Config { get; set; }
        public virtual AccountAddition Addition { get; set; }
        [Display(Name = "Account_Roles",
            ResourceType = typeof(Resources.Models.Models))]
        public virtual ICollection<AccountRoleLink> Roles { get; set; }
        [Display(Name = "Account_IsEnabled",
            ResourceType = typeof(Resources.Models.Models))]
        public bool IsEnabled { get; set; }
    }
}