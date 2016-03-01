namespace SpreadBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPools : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pools",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Created = c.DateTime(),
                        PublicKey = c.String(),
                        PrivateKey = c.Binary(),
                        Key = c.Binary(),
                        IV = c.Binary(),
                        Salt = c.Binary(),
                        IsActive = c.Boolean(nullable: false),
                        Owner_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.Owner_ID)
                .Index(t => t.Owner_ID);
            
            CreateTable(
                "dbo.PoolShares",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.Binary(),
                        SharedOn = c.DateTime(),
                        VisibleFrom = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        Pool_ID = c.Int(),
                        SharedWith_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pools", t => t.Pool_ID)
                .ForeignKey("dbo.Accounts", t => t.SharedWith_ID)
                .Index(t => t.Pool_ID)
                .Index(t => t.SharedWith_ID);
            
            AddColumn("dbo.Accounts", "Pool_ID", c => c.Int());
            CreateIndex("dbo.Accounts", "Pool_ID");
            AddForeignKey("dbo.Accounts", "Pool_ID", "dbo.Pools", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PoolShares", "SharedWith_ID", "dbo.Accounts");
            DropForeignKey("dbo.PoolShares", "Pool_ID", "dbo.Pools");
            DropForeignKey("dbo.Accounts", "Pool_ID", "dbo.Pools");
            DropForeignKey("dbo.Pools", "Owner_ID", "dbo.Accounts");
            DropIndex("dbo.PoolShares", new[] { "SharedWith_ID" });
            DropIndex("dbo.PoolShares", new[] { "Pool_ID" });
            DropIndex("dbo.Pools", new[] { "Owner_ID" });
            DropIndex("dbo.Accounts", new[] { "Pool_ID" });
            DropColumn("dbo.Accounts", "Pool_ID");
            DropTable("dbo.PoolShares");
            DropTable("dbo.Pools");
        }
    }
}
