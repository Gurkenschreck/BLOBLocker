using Cipha.Security.Cryptography.Hash;
using CryptoPool.Entities.Models.AdminTool;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CryptoPool.AdminTool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            CreateAdminIfNotExistent();
        }

        void CreateAdminIfNotExistent()
        {
            using (AdminToolContext atcont = new AdminToolContext())
            {
                var sysadm = atcont.Accounts.FirstOrDefault(p => p.Alias == "Sysadm");

                if (sysadm == null)
                {
                    sysadm = new Entities.Models.AdminTool.Account();
                    sysadm.Alias = "Sysadm";
                    using (var hasher = new Hasher<SHA512Cng>())
                    {
                        sysadm.PasswordHash = hasher.Hash("DefaultChangeMe");
                    }
                    sysadm.Roles = new List<RoleLink>();
                    var adminRole = atcont.Roles.FirstOrDefault(p => p.Definition == "Administrator");
                    var moderatorRole = atcont.Roles.FirstOrDefault(p => p.Definition == "Moderator");
                    var translatorRole = atcont.Roles.FirstOrDefault(p => p.Definition == "Translator");
                    if (adminRole == null)
                    {
                        adminRole = new Role();
                        adminRole.Definition = "Administrator";
                        atcont.Roles.Add(adminRole);
                    }
                    if (moderatorRole == null)
                    {
                        adminRole = new Role();
                        adminRole.Definition = "Moderator";
                        atcont.Roles.Add(adminRole);
                    }
                    if (translatorRole == null)
                    {
                        adminRole = new Role();
                        adminRole.Definition = "Translator";
                        atcont.Roles.Add(adminRole);
                    }
                    sysadm.Roles.Add(new RoleLink
                    {
                        Account = sysadm,
                        Role = adminRole
                    });
                    atcont.Accounts.Add(sysadm);
                    atcont.SaveChanges();
                }
            }
        }
    }
}
