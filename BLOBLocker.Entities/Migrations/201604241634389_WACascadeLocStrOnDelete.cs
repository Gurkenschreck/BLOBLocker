namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WACascadeLocStrOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources");
            AddForeignKey("dbo.LocalizedStrings", "BaseResourceKey", "dbo.StringResources", "ID");
        }
    }
}
