namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAExchangeStoredFileEncrypted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoredFiles", "StoringMode", c => c.Int(nullable: false));
            DropColumn("dbo.StoredFiles", "Encrypted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoredFiles", "Encrypted", c => c.Boolean(nullable: false));
            DropColumn("dbo.StoredFiles", "StoringMode");
        }
    }
}
