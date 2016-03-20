namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CPAddPoolKeyToShare : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PoolShares", "PoolKey", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PoolShares", "PoolKey");
        }
    }
}
