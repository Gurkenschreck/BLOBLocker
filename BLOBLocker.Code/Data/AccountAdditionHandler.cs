using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class AccountAdditionHandler
    {
        public class AccountAdditionProperties
        {
            public string Email { get; set; }
            
        }

        public AccountAddition SetupNew(AccountAdditionProperties props)
        {
            AccountAddition addition = new AccountAddition();
            addition.ContactEmail = props.Email;
            return addition;
        }
    }
}
