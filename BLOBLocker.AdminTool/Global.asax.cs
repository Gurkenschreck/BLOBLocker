using Cipha.Security.Cryptography.Hash;
using BLOBLocker.Entities.Models.AdminTool;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Cipha.Security.Cryptography;
using BLOBLocker.Code;

namespace BLOBLocker.AdminTool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Add(new BlobLockerViewEngine());
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            CreateAdminIfNotExistent();
        }

        void CreateAdminIfNotExistent()
        {
            using (BLATContext atcont = new BLATContext())
            {
                var sysadm = atcont.Accounts.FirstOrDefault(p => p.Alias == "Sysadm");
                if (sysadm == null)
                {
                    sysadm = new Entities.Models.AdminTool.Account();
                    sysadm.Alias = "Sysadm";
                    sysadm.IsActive = true;
                    sysadm.Salt = Utilities.GenerateBytes(32);
                    using(var deriver = new Rfc2898DeriveBytes("changeme,4,4", sysadm.Salt, 21423))
                    {
                        sysadm.DerivedPassword = deriver.GetBytes(sysadm.Salt.Length);
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
                    sysadm.Roles.Add(new RoleLink
                    {
                        Account = sysadm,
                        Role = moderatorRole
                    });
                    sysadm.Roles.Add(new RoleLink
                    {
                        Account = sysadm,
                        Role = translatorRole
                    });

                    atcont.Accounts.Add(sysadm);
                    atcont.SaveChanges();
                }
            }
        }
    }
}
