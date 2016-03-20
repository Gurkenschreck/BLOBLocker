using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BLOBLocker.Code.Membership
{
    public class CPRoleProvider : RoleProvider
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
            using(CryptoPoolContext context = new CryptoPoolContext())
            {
                var acc = context.Accounts.FirstOrDefault(p => p.Alias == username);
                if(acc == null)
                {
                    return null;
                }
                else
                {
                    return acc.Roles.Select(x => x.Role.Definition).ToArray();
                }
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();

        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using(var context = new CryptoPoolContext())
            {
                var acc = context.Accounts.FirstOrDefault(x => x.Alias == username);

                foreach(AccountRoleLink role in acc.Roles)
                {
                    if (role.Role.Definition == roleName)
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
            using(var context = new CryptoPoolContext())
            {
                var role = context.AccountRoles.FirstOrDefault(r => r.Definition == roleName);
                if (role != null)
                    return true;
            }
            return false;
        }
    }
}