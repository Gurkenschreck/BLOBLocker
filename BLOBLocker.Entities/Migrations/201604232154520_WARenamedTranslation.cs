namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WARenamedTranslation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LocalizedStrings", newName: "StringResources");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.StringResources", newName: "LocalizedStrings");
        }
    }
}
