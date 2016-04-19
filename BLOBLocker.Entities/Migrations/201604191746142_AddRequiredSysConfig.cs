namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequiredSysConfig : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SystemConfigurations", "Key", c => c.String(nullable: false));
            AlterColumn("dbo.SystemConfigurations", "Value", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SystemConfigurations", "Value", c => c.String());
            AlterColumn("dbo.SystemConfigurations", "Key", c => c.String());
        }
    }
}
