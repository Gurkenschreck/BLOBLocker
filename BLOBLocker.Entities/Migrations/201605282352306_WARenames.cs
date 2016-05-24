namespace BLOBLocker.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WARenames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoredFiles", "IsVisible", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoredFiles", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.StoredFiles", "FileSize", c => c.Int(nullable: false));
            DropColumn("dbo.StoredFiles", "Visible");
            DropColumn("dbo.StoredFiles", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoredFiles", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoredFiles", "Visible", c => c.Boolean(nullable: false));
            AlterColumn("dbo.StoredFiles", "FileSize", c => c.Long(nullable: false));
            DropColumn("dbo.StoredFiles", "IsDeleted");
            DropColumn("dbo.StoredFiles", "IsVisible");
        }
    }
}
