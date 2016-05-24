namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAAddStoreFileDeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoredFiles", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoredFiles", "IsDeleted");
        }
    }
}
