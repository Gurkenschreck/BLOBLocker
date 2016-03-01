namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNotificationOwner : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "Owner_ID", "dbo.Accounts");
            DropIndex("dbo.Notifications", new[] { "Owner_ID" });
            DropColumn("dbo.Notifications", "Owner_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "Owner_ID", c => c.Int());
            CreateIndex("dbo.Notifications", "Owner_ID");
            AddForeignKey("dbo.Notifications", "Owner_ID", "dbo.Accounts", "ID");
        }
    }
}
