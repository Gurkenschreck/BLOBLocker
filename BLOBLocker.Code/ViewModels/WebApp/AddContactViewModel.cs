using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class AddContactViewModel
    {
        [LocalizedDisplayName("Account.Alias")]
        [Required]
        public string AddAlias { get; set; }
    }
}
