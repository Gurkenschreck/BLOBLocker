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
                        Definition = c.String(),
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
                        ConfigID = c.Int(nullable: false),
                        AdditionID = c.Int(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AccountAdditions", t => t.AdditionID, cascadeDelete: true)
                .ForeignKey("dbo.CryptoConfigs", t => t.ConfigID, cascadeDelete: true)
                .Index(t => t.ConfigID)
                .Index(t => t.AdditionID);
            
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
                "dbo.Contacts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
                        AccountAddition_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.AccountID, cascadeDelete: true)
                .ForeignKey("dbo.AccountAdditions", t => t.AccountAddition_ID)
                .Index(t => t.AccountID)
                .Index(t => t.AccountAddition_ID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        CreatedOn = c.DateTime(),
                        IsVisible = c.Boolean(nullable: false),
                        AccountAddition_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AccountAdditions", t => t.AccountAddition_ID)
                .Index(t => t.AccountAddition_ID);
            
            CreateTable(
                "dbo.CryptoConfigs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PublicKey = c.String(),
                        PrivateKey = c.Binary(),
                        PublicKeySignature = c.String(),
                        IV = c.Binary(),
                        RSAKeySize = c.Int(nullable: false),
                        Key = c.Binary(),
                        KeySize = c.Int(nullable: false),
                        IterationCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PoolShares",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PoolID = c.Int(nullable: false),
                        SharedWithID = c.Int(nullable: false),
                        SharedKey = c.Binary(),
                        SharedOn = c.DateTime(),
                        VisibleFrom = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pools", t => t.PoolID)
                .ForeignKey("dbo.Accounts", t => t.SharedWithID)
                .Index(t => t.PoolID)
                .Index(t => t.SharedWithID);
            
            CreateTable(
                "dbo.Pools",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ConfigID = c.Int(nullable: false),
                        Salt = c.Binary(),
                        OwnerID = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CryptoConfigs", t => t.ConfigID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.OwnerID)
                .Index(t => t.ConfigID)
                .Index(t => t.OwnerID);
            
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
            DropForeignKey("dbo.PoolShares", "SharedWithID", "dbo.Accounts");
            DropForeignKey("dbo.PoolShares", "PoolID", "dbo.Pools");
            DropForeignKey("dbo.Pools", "OwnerID", "dbo.Accounts");
            DropForeignKey("dbo.Pools", "ConfigID", "dbo.CryptoConfigs");
            DropForeignKey("dbo.Accounts", "ConfigID", "dbo.CryptoConfigs");
            DropForeignKey("dbo.Accounts", "AdditionID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Notifications", "AccountAddition_ID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Contacts", "AccountAddition_ID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Contacts", "AccountID", "dbo.Accounts");
            DropIndex("dbo.AccountRoleLinks", new[] { "Role_ID" });
            DropIndex("dbo.AccountRoleLinks", new[] { "Account_ID" });
            DropIndex("dbo.Pools", new[] { "OwnerID" });
            DropIndex("dbo.Pools", new[] { "ConfigID" });
            DropIndex("dbo.PoolShares", new[] { "SharedWithID" });
            DropIndex("dbo.PoolShares", new[] { "PoolID" });
            DropIndex("dbo.Notifications", new[] { "AccountAddition_ID" });
            DropIndex("dbo.Contacts", new[] { "AccountAddition_ID" });
            DropIndex("dbo.Contacts", new[] { "AccountID" });
            DropIndex("dbo.Accounts", new[] { "AdditionID" });
            DropIndex("dbo.Accounts", new[] { "ConfigID" });
            DropTable("dbo.Configurations");
            DropTable("dbo.AccountRoleLinks");
            DropTable("dbo.Pools");
            DropTable("dbo.PoolShares");
            DropTable("dbo.CryptoConfigs");
            DropTable("dbo.Notifications");
            DropTable("dbo.Contacts");
            DropTable("dbo.AccountAdditions");
            DropTable("dbo.Accounts");
            DropTable("dbo.AccountRoles");
        }
    }
}
