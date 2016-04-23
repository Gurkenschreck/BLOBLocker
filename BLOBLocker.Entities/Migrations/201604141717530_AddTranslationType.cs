namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTranslationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalizedStrings", "Type", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalizedStrings", "Type");
        }
    }
}
