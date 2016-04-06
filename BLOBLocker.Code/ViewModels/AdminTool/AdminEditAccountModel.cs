using BLOBLocker.Entities.Models.AdminTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class AdminEditAccountModel
    {
        public AdminEditAccountModel()
        {

        }
        public AdminEditAccountModel(Account acc)
        {
            Alias = acc.Alias;
            IsActive = acc.IsActive;
            Email = acc.Email;
            ID = acc.ID;
            IsAdmin = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Administrator");
            IsModerator = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Moderator");
            IsTranslator = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Translator");
        }
        public void ApplyChanges(Account acc, ICollection<Role> roles, BLATContext context)
        {
            if (acc.ID != ID)
                throw new NotSupportedException("this model does not correspond to this account");
            if(!string.IsNullOrWhiteSpace(Alias))
                acc.Alias = Alias;
            if(!string.IsNullOrWhiteSpace(Email))
                acc.Email = Email;
            acc.IsActive = IsActive;
            // derive password
            if (!string.IsNullOrWhiteSpace(Password))
            {
                using (var deriver = new Rfc2898DeriveBytes(Password, acc.Salt, 21423))
                {
                    acc.DerivedPassword = deriver.GetBytes(acc.Salt.Length);
                }
            }
            // set roles
            bool isAdm = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Administrator");
            bool isMod = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Moderator");
            bool isTra = acc.Roles.Select(p => p.Role.Definition).Any(p => p == "Translator");
            if(isAdm != IsAdmin)
            {
                if (isAdm)
                {
                    var adminRoleLink = acc.Roles.FirstOrDefault(p => p.Role.Definition == "Administrator");
                    context.RoleLinks.Remove(adminRoleLink);
                }
                else
                    acc.Roles.Add(new RoleLink
                    {
                        Account = acc,
                        Role = roles.FirstOrDefault(p => p.Definition == "Administrator")
                    });
            }
            if (isMod != IsModerator)
            {
                if (isMod)
                {
                    var modRoleLink = acc.Roles.FirstOrDefault(p => p.Role.Definition == "Moderator");
                    context.RoleLinks.Remove(modRoleLink);
                }
                else
                    acc.Roles.Add(new RoleLink
                    {
                        Account = acc,
                        Role = roles.FirstOrDefault(p => p.Definition == "Moderator")
                    });
            }
            if (isTra != IsTranslator)
            {
                if (isTra)
                {
                    var traRoleLink = acc.Roles.FirstOrDefault(p => p.Role.Definition == "Translator");
                    context.RoleLinks.Remove(traRoleLink);
                }
                else
                    acc.Roles.Add(new RoleLink
                    {
                        Account = acc,
                        Role = roles.FirstOrDefault(p => p.Definition == "Translator")
                    });
            }
        }
        public int ID { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public bool IsTranslator { get; set; }

    }
}