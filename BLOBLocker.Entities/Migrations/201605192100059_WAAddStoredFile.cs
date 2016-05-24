namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAAddStoredFile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoredFiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OwnerID = c.Int(nullable: false),
                        OriginalFileName = c.String(),
                        StoredFileName = c.String(),
                        FileExtension = c.String(),
                        FileSize = c.Long(nullable: false),
                        FileSignature = c.String(),
                        Description = c.String(),
                        UploadedOn = c.DateTime(),
                        IPv4Address = c.String(),
                        MD5Checksum = c.String(),
                        SHA1Checksum = c.String(),
                        Visible = c.Boolean(nullable: false),
                        Pool_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.OwnerID, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.Pool_ID)
                .Index(t => t.OwnerID)
                .Index(t => t.Pool_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoredFiles", "Pool_ID", "dbo.Pools");
            DropForeignKey("dbo.StoredFiles", "OwnerID", "dbo.Accounts");
            DropIndex("dbo.StoredFiles", new[] { "Pool_ID" });
            DropIndex("dbo.StoredFiles", new[] { "OwnerID" });
            DropTable("dbo.StoredFiles");
        }
    }
}
