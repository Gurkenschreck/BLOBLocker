namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAAddStoredFileMimeType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoredFiles", "MimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoredFiles", "MimeType");
        }
    }
}
