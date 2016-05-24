namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAAddEncryptedBoolToStoredFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoredFiles", "Encrypted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoredFiles", "Encrypted");
        }
    }
}
