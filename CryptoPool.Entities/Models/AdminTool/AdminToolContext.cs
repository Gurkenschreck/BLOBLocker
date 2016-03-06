﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.AdminTool
{
    public class AdminToolContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }


        public AdminToolContext()
#if DEBUG
            : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CryptoPool.Entities.Models.AdminTool.AdminToolContext;Integrated Security=True")
#endif
#if !DEBUG
            :base(ConfigurationManager.ConnectionStrings["CryptoPoolEntities"].ConnectionString)
#endif
        {

        }
    }
}