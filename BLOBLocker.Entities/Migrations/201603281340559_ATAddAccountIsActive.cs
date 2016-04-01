namespace BLOBLocker.Entities.Models.Migrations.AT
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ATAddAccountIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsActive");
        }
    }
}
