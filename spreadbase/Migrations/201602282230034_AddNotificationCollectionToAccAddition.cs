namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotificationCollectionToAccAddition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "AccountAddition_ID", c => c.Int());
            CreateIndex("dbo.Notifications", "AccountAddition_ID");
            AddForeignKey("dbo.Notifications", "AccountAddition_ID", "dbo.AccountAdditions", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "AccountAddition_ID", "dbo.AccountAdditions");
            DropIndex("dbo.Notifications", new[] { "AccountAddition_ID" });
            DropColumn("dbo.Notifications", "AccountAddition_ID");
        }
    }
}
