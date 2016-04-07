namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AWAddPoolRights : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pools", "DefaultRights", c => c.Int(nullable: false));
            AddColumn("dbo.PoolShares", "Rights", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PoolShares", "Rights");
            DropColumn("dbo.Pools", "DefaultRights");
        }
    }
}
