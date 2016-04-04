namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WARenamePoolDescToTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pools", "Title", c => c.String());
            DropColumn("dbo.Pools", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pools", "Description", c => c.String());
            DropColumn("dbo.Pools", "Title");
        }
    }
}
