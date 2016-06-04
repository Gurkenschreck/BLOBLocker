namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAInitial : DbMigration
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
                        Salt = c.Binary(),
                        ConfigID = c.Int(nullable: false),
                        AdditionID = c.Int(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AccountAdditions", t => t.AdditionID, cascadeDelete: true)
                .ForeignKey("dbo.CryptoConfigurations", t => t.ConfigID, cascadeDelete: true)
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
                "dbo.CryptoConfigurations",
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
                "dbo.MemoryPools",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        BasicSpace = c.Int(nullable: false),
                        AdditionalSpace = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.AssignedMemories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MemoryPoolID = c.Int(nullable: false),
                        DateAssigned = c.DateTime(),
                        Space = c.Int(nullable: false),
                        IsBasic = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        Pool_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.MemoryPools", t => t.MemoryPoolID)
                .ForeignKey("dbo.Pools", t => t.Pool_ID)
                .Index(t => t.MemoryPoolID)
                .Index(t => t.Pool_ID);
            
            CreateTable(
                "dbo.Pools",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UniqueIdentifier = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        ConfigID = c.Int(nullable: false),
                        Salt = c.Binary(),
                        OwnerID = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        ChatEnabled = c.Boolean(nullable: false),
                        FileStorageEnabled = c.Boolean(nullable: false),
                        LinkRepositoryEnabled = c.Boolean(nullable: false),
                        DefaultRights = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CryptoConfigurations", t => t.ConfigID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.OwnerID)
                .Index(t => t.ConfigID)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "dbo.StoredFiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OwnerID = c.Int(nullable: false),
                        OriginalFileName = c.String(),
                        StoredFileName = c.String(),
                        FileExtension = c.String(),
                        MimeType = c.String(),
                        FileSize = c.Int(nullable: false),
                        FileSignature = c.String(),
                        Description = c.String(),
                        UploadedOn = c.DateTime(),
                        IPv4Address = c.String(),
                        MD5Checksum = c.String(),
                        SHA1Checksum = c.String(),
                        IsVisible = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Encrypted = c.Boolean(nullable: false),
                        Pool_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.OwnerID, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.Pool_ID)
                .Index(t => t.OwnerID)
                .Index(t => t.Pool_ID);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        TextSignature = c.String(),
                        PoolID = c.Int(nullable: false),
                        Sent = c.DateTime(),
                        IsVisible = c.Boolean(nullable: false),
                        Sender_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pools", t => t.PoolID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.Sender_ID)
                .Index(t => t.PoolID)
                .Index(t => t.Sender_ID);
            
            CreateTable(
                "dbo.PoolShares",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PoolID = c.Int(nullable: false),
                        SharedWithID = c.Int(nullable: false),
                        PoolKey = c.Binary(),
                        SharedOn = c.DateTime(),
                        ShowSince = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        Rights = c.Int(nullable: false),
                        Config_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CryptoConfigurations", t => t.Config_ID)
                .ForeignKey("dbo.Pools", t => t.PoolID)
                .ForeignKey("dbo.Accounts", t => t.SharedWithID)
                .Index(t => t.PoolID)
                .Index(t => t.SharedWithID)
                .Index(t => t.Config_ID);
            
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
                "dbo.StringResources",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                        Type = c.Byte(nullable: false),
                        Base = c.String(),
                        Comment = c.String(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LocalizedStrings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BaseResourceKey = c.Int(nullable: false),
                        UICulture = c.String(),
                        Translation = c.String(),
                        LiveTranslation = c.String(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Status = c.Byte(nullable: false),
                        StringResource_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StringResources", t => t.BaseResourceKey, cascadeDelete: true)
                .ForeignKey("dbo.StringResources", t => t.StringResource_ID)
                .Index(t => t.BaseResourceKey)
                .Index(t => t.StringResource_ID);
            
            CreateTable(
                "dbo.SystemConfigurations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedStrings", "StringResource_ID", "dbo.StringResources");
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            DropForeignKey("dbo.AccountRoleLinks", "Role_ID", "dbo.AccountRoles");
            DropForeignKey("dbo.AccountRoleLinks", "Account_ID", "dbo.Accounts");
            DropForeignKey("dbo.PoolShares", "SharedWithID", "dbo.Accounts");
            DropForeignKey("dbo.PoolShares", "PoolID", "dbo.Pools");
            DropForeignKey("dbo.PoolShares", "Config_ID", "dbo.CryptoConfigurations");
            DropForeignKey("dbo.Pools", "OwnerID", "dbo.Accounts");
            DropForeignKey("dbo.Messages", "Sender_ID", "dbo.Accounts");
            DropForeignKey("dbo.Messages", "PoolID", "dbo.Pools");
            DropForeignKey("dbo.StoredFiles", "Pool_ID", "dbo.Pools");
            DropForeignKey("dbo.StoredFiles", "OwnerID", "dbo.Accounts");
            DropForeignKey("dbo.Pools", "ConfigID", "dbo.CryptoConfigurations");
            DropForeignKey("dbo.AssignedMemories", "Pool_ID", "dbo.Pools");
            DropForeignKey("dbo.MemoryPools", "ID", "dbo.Accounts");
            DropForeignKey("dbo.AssignedMemories", "MemoryPoolID", "dbo.MemoryPools");
            DropForeignKey("dbo.Accounts", "ConfigID", "dbo.CryptoConfigurations");
            DropForeignKey("dbo.Accounts", "AdditionID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Notifications", "AccountAddition_ID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Contacts", "AccountAddition_ID", "dbo.AccountAdditions");
            DropForeignKey("dbo.Contacts", "AccountID", "dbo.Accounts");
            DropIndex("dbo.LocalizedStrings", new[] { "StringResource_ID" });
            DropIndex("dbo.LocalizedStrings", new[] { "BaseResourceKey" });
            DropIndex("dbo.AccountRoleLinks", new[] { "Role_ID" });
            DropIndex("dbo.AccountRoleLinks", new[] { "Account_ID" });
            DropIndex("dbo.PoolShares", new[] { "Config_ID" });
            DropIndex("dbo.PoolShares", new[] { "SharedWithID" });
            DropIndex("dbo.PoolShares", new[] { "PoolID" });
            DropIndex("dbo.Messages", new[] { "Sender_ID" });
            DropIndex("dbo.Messages", new[] { "PoolID" });
            DropIndex("dbo.StoredFiles", new[] { "Pool_ID" });
            DropIndex("dbo.StoredFiles", new[] { "OwnerID" });
            DropIndex("dbo.Pools", new[] { "OwnerID" });
            DropIndex("dbo.Pools", new[] { "ConfigID" });
            DropIndex("dbo.AssignedMemories", new[] { "Pool_ID" });
            DropIndex("dbo.AssignedMemories", new[] { "MemoryPoolID" });
            DropIndex("dbo.MemoryPools", new[] { "ID" });
            DropIndex("dbo.Notifications", new[] { "AccountAddition_ID" });
            DropIndex("dbo.Contacts", new[] { "AccountAddition_ID" });
            DropIndex("dbo.Contacts", new[] { "AccountID" });
            DropIndex("dbo.Accounts", new[] { "AdditionID" });
            DropIndex("dbo.Accounts", new[] { "ConfigID" });
            DropTable("dbo.SystemConfigurations");
            DropTable("dbo.LocalizedStrings");
            DropTable("dbo.StringResources");
            DropTable("dbo.AccountRoleLinks");
            DropTable("dbo.PoolShares");
            DropTable("dbo.Messages");
            DropTable("dbo.StoredFiles");
            DropTable("dbo.Pools");
            DropTable("dbo.AssignedMemories");
            DropTable("dbo.MemoryPools");
            DropTable("dbo.CryptoConfigurations");
            DropTable("dbo.Notifications");
            DropTable("dbo.Contacts");
            DropTable("dbo.AccountAdditions");
            DropTable("dbo.Accounts");
            DropTable("dbo.AccountRoles");
        }
    }
}
