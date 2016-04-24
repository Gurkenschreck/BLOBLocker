namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAChangeResourceKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            DropIndex("dbo.LocalizedStrings", new[] { "BaseResourceKey" });
            DropPrimaryKey("dbo.StringResources");
            AddColumn("dbo.StringResources", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.StringResources", "Key", c => c.String(nullable: false));
            AlterColumn("dbo.LocalizedStrings", "BaseResourceKey", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.StringResources", "ID");
            CreateIndex("dbo.LocalizedStrings", "BaseResourceKey");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            DropIndex("dbo.LocalizedStrings", new[] { "BaseResourceKey" });
            DropPrimaryKey("dbo.StringResources");
            AlterColumn("dbo.LocalizedStrings", "BaseResourceKey", c => c.String(maxLength: 128));
            AlterColumn("dbo.StringResources", "Key", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.StringResources", "ID");
            AddPrimaryKey("dbo.StringResources", "Key");
            CreateIndex("dbo.LocalizedStrings", "BaseResourceKey");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "Key");
        }
    }
}
