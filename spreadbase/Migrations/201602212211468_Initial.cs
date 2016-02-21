namespace spreadbase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Alias = c.String(),
                        Password = c.String(),
                        Salt = c.Binary(),
                        ContactEmail = c.String(),
                        Type = c.Byte(nullable: false),
                        Config_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CryptoConfigs", t => t.Config_ID)
                .Index(t => t.Config_ID);
            
            CreateTable(
                "dbo.CryptoConfigs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PublicKey = c.String(),
                        PrivateKey = c.String(),
                        PublicKeySignature = c.String(),
                        IV = c.Binary(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "Config_ID", "dbo.CryptoConfigs");
            DropIndex("dbo.Accounts", new[] { "Config_ID" });
            DropTable("dbo.CryptoConfigs");
            DropTable("dbo.Accounts");
        }
    }
}
