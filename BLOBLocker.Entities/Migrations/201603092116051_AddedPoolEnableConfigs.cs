namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPoolEnableConfigs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pools", "ChatEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Pools", "FileStorageEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Pools", "LinkRepositoryEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pools", "LinkRepositoryEnabled");
            DropColumn("dbo.Pools", "FileStorageEnabled");
            DropColumn("dbo.Pools", "ChatEnabled");
        }
    }
}
