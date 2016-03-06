namespace CryptoPool.Entities.Models.Migrations.AT
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ATInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Alias = c.String(),
                        Email = c.String(),
                        PasswordHash = c.Binary(),
                        LastLogin = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RoleLinks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Account_ID = c.Int(),
                        Role_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.Account_ID)
                .ForeignKey("dbo.Roles", t => t.Role_ID)
                .Index(t => t.Account_ID)
                .Index(t => t.Role_ID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Definition = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleLinks", "Role_ID", "dbo.Roles");
            DropForeignKey("dbo.RoleLinks", "Account_ID", "dbo.Accounts");
            DropIndex("dbo.RoleLinks", new[] { "Role_ID" });
            DropIndex("dbo.RoleLinks", new[] { "Account_ID" });
            DropTable("dbo.Roles");
            DropTable("dbo.RoleLinks");
            DropTable("dbo.Accounts");
        }
    }
}
