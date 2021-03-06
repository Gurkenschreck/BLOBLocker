﻿using BLOBLocker.Entities.Models.AdminTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BLOBLocker.Code.Membership
{
    public class AdminToolRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return "BLOBLocker AdminTool";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            using(BLATContext context = new BLATContext())
            {
                context.Roles.Add(new Role
                {
                    Definition = roleName
                });
                context.SaveChanges();
            }
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
            using (var context = new BLATContext())
            {
                var acc = context.Accounts.FirstOrDefault(p => p.Alias == username);

                string[] roles = acc.Roles.Select(p => p.Role.Definition).ToArray();
                return roles;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using(var context = new BLATContext())
            {
                Role role = context.Roles.FirstOrDefault(p => p.Definition == roleName);
                ICollection<Account> accs = (from acc in context.Accounts
                                            where acc.Roles.Any(p => p.Role == role)
                                            select acc).ToList();
                string[] accNames = (from acc in accs
                                    select acc.Alias).ToArray();
                return accNames;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using(var context = new BLATContext())
            {
                Account acc = context.Accounts.FirstOrDefault(p => p.Alias == username);
                if(acc != null)
                {
                    foreach(RoleLink link in acc.Roles)
                    {
                        if (link.Role.Definition == roleName)
                            return true;
                    }
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
            using(var context = new BLATContext())
            {
                Role role = context.Roles.FirstOrDefault(p => p.Definition == roleName);
                if (role != null)
                    return true;
            }
            return false;
        }
    }
}