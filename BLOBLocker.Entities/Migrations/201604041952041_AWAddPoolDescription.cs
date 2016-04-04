namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AWAddPoolDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pools", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pools", "Description");
        }
    }
}
