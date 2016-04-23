namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WARenamed : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.LocalizedStrings", name: "TranslationKey", newName: "BaseResourceKey");
            RenameIndex(table: "dbo.LocalizedStrings", name: "IX_TranslationKey", newName: "IX_BaseResourceKey");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.LocalizedStrings", name: "IX_BaseResourceKey", newName: "IX_TranslationKey");
            RenameColumn(table: "dbo.LocalizedStrings", name: "BaseResourceKey", newName: "TranslationKey");
        }
    }
}
