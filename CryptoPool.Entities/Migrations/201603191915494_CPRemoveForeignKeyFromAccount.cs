namespace CryptoPool.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CPRemoveForeignKeyFromAccount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Accounts", "MemoryPoolID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "MemoryPoolID", c => c.Int(nullable: false));
        }
    }
}
