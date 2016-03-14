namespace CryptoPool.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssignedIsEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssignedMemories", "IsEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssignedMemories", "IsEnabled");
        }
    }
}
