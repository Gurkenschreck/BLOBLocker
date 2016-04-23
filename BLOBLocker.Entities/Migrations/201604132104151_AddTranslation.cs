namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalizedStrings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Base = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.LocalizedStrings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TranslationKey = c.String(maxLength: 128),
                        Culture = c.String(),
                        Translation = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.LocalizedStrings", t => t.TranslationKey)
                .Index(t => t.TranslationKey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.LocalizedStrings");
            DropIndex("dbo.LocalizedStrings", new[] { "BaseResourceKey" });
            DropTable("dbo.LocalizedStrings");
            DropTable("dbo.LocalizedStrings");
        }
    }
}
