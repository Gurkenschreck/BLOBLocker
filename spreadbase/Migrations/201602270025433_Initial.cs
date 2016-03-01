namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountRoles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Alias = c.String(),
                        Password = c.String(),
                        Salt = c.Binary(),
                        Addition_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AccountAdditions", t => t.Addition_ID)
                .Index(t => t.Addition_ID);
            
            CreateTable(
                "dbo.AccountAdditions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContactEmail = c.String(),
                        LastLogin = c.DateTime(),
                        LastFailedLogin = c.DateTime(),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CryptoConfigs",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        PublicKey = c.String(),
                        PrivateKey = c.Binary(),
                        PublicKeySignature = c.String(),
                        IV = c.Binary(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.AccountRoleLinks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Account_ID = c.Int(),
                        Role_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.Account_ID)
                .ForeignKey("dbo.AccountRoles", t => t.Role_ID)
                .Index(t => t.Account_ID)
                .Index(t => t.Role_ID);
            
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountRoleLinks", "Role_ID", "dbo.AccountRoles");
            DropForeignKey("dbo.AccountRoleLinks", "Account_ID", "dbo.Accounts");
            DropForeignKey("dbo.CryptoConfigs", "ID", "dbo.Accounts");
            DropForeignKey("dbo.Accounts", "Addition_ID", "dbo.AccountAdditions");
            DropIndex("dbo.AccountRoleLinks", new[] { "Role_ID" });
            DropIndex("dbo.AccountRoleLinks", new[] { "Account_ID" });
            DropIndex("dbo.CryptoConfigs", new[] { "ID" });
            DropIndex("dbo.Accounts", new[] { "Addition_ID" });
            DropTable("dbo.Configurations");
            DropTable("dbo.AccountRoleLinks");
            DropTable("dbo.CryptoConfigs");
            DropTable("dbo.AccountAdditions");
            DropTable("dbo.Accounts");
            DropTable("dbo.AccountRoles");
        }
    }
}
