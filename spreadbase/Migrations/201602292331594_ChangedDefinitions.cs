namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedDefinitions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountRoles", "Definition", c => c.String());
            DropColumn("dbo.AccountRoles", "RoleName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountRoles", "RoleName", c => c.String());
            DropColumn("dbo.AccountRoles", "Definition");
        }
    }
}
