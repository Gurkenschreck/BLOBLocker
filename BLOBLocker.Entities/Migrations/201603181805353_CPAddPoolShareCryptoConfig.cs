namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CPAddPoolShareCryptoConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PoolShares", "ShowSince", c => c.DateTime());
            AddColumn("dbo.PoolShares", "Config_ID", c => c.Int());
            CreateIndex("dbo.PoolShares", "Config_ID");
            AddForeignKey("dbo.PoolShares", "Config_ID", "dbo.CryptoConfigurations", "ID");
            DropColumn("dbo.PoolShares", "SharedKey");
            DropColumn("dbo.PoolShares", "VisibleFrom");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PoolShares", "VisibleFrom", c => c.DateTime());
            AddColumn("dbo.PoolShares", "SharedKey", c => c.Binary());
            DropForeignKey("dbo.PoolShares", "Config_ID", "dbo.CryptoConfigurations");
            DropIndex("dbo.PoolShares", new[] { "Config_ID" });
            DropColumn("dbo.PoolShares", "Config_ID");
            DropColumn("dbo.PoolShares", "ShowSince");
        }
    }
}
