using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class LoginViewModel
    {
        [Required]
        [LocalizedDisplayName("Account.Alias")]
        public string Alias { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Account.Password")]
        public string Password { get; set; }
    }
}
