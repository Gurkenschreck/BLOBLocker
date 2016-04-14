namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTranslationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Translations", "Type", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Translations", "Type");
        }
    }
}
