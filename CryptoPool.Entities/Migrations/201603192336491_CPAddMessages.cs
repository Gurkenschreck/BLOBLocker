namespace CryptoPool.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CPAddMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        PoolID = c.Int(nullable: false),
                        Sent = c.DateTime(),
                        IsVisible = c.Boolean(nullable: false),
                        Sender_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pools", t => t.PoolID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.Sender_ID)
                .Index(t => t.PoolID)
                .Index(t => t.Sender_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Sender_ID", "dbo.Accounts");
            DropForeignKey("dbo.Messages", "PoolID", "dbo.Pools");
            DropIndex("dbo.Messages", new[] { "Sender_ID" });
            DropIndex("dbo.Messages", new[] { "PoolID" });
            DropTable("dbo.Messages");
        }
    }
}
