namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsEnabledAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsEnabled");
        }
    }
}
