namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WARenamedBaseResourceKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            AddColumn("dbo.LocalizedStrings", "StringResource_ID", c => c.Int());
            CreateIndex("dbo.LocalizedStrings", "StringResource_ID");
            AddForeignKey("dbo.LocalizedStrings", "StringResource_ID", "dbo.StringResources", "ID");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            DropForeignKey("dbo.LocalizedStrings", "StringResource_ID", "dbo.StringResources");
            DropIndex("dbo.LocalizedStrings", new[] { "StringResource_ID" });
            DropColumn("dbo.LocalizedStrings", "StringResource_ID");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "ID", cascadeDelete: true);
        }
    }
}
