using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SpreadBase.App_Code
{
    public class SBRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();

        }

        public override string[] GetRolesForUser(string username)
        {
            using(SpreadBaseContext context = new SpreadBaseContext())
            {
                var acc = context.Accounts.FirstOrDefault(p => p.Alias == username);
                if(acc == null)
                {
                    return null;
                }
                else
                {
                    return acc.Roles.Select(x => x.Role.RoleName).ToArray();
                }
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();

        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using(var context = new SpreadBaseContext())
            {
                var acc = context.Accounts.FirstOrDefault(x => x.Alias == username);

                foreach(AccountRoleLink role in acc.Roles)
                {
                    if (role.Role.RoleName == roleName)
                        return true;
                }
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            using(var context = new SpreadBaseContext())
            {
                var role = context.AccountRoles.FirstOrDefault(r => r.RoleName == roleName);
                if (role != null)
                    return true;
            }
            return false;
        }
    }
}