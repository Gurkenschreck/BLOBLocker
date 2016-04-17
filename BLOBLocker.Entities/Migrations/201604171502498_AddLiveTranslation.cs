namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLiveTranslation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalizedStrings", "LiveTranslation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalizedStrings", "LiveTranslation");
        }
    }
}
