namespace BLOBLocker.Entities.Models.Migrations.AT
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ATAddPasswordDerived : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "DerivedPassword", c => c.Binary());
            AddColumn("dbo.Accounts", "Salt", c => c.Binary());
            DropColumn("dbo.Accounts", "PasswordHash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "PasswordHash", c => c.Binary());
            DropColumn("dbo.Accounts", "Salt");
            DropColumn("dbo.Accounts", "DerivedPassword");
        }
    }
}
