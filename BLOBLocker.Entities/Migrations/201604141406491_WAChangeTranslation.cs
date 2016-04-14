namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAChangeTranslation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Translations", "Comment", c => c.String());
            AddColumn("dbo.Translations", "Version", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.LocalizedStrings", "UICulture", c => c.String());
            AddColumn("dbo.LocalizedStrings", "Version", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.LocalizedStrings", "Status", c => c.Byte(nullable: false));
            DropColumn("dbo.LocalizedStrings", "Culture");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LocalizedStrings", "Culture", c => c.String());
            DropColumn("dbo.LocalizedStrings", "Status");
            DropColumn("dbo.LocalizedStrings", "Version");
            DropColumn("dbo.LocalizedStrings", "UICulture");
            DropColumn("dbo.Translations", "Version");
            DropColumn("dbo.Translations", "Comment");
        }
    }
}
