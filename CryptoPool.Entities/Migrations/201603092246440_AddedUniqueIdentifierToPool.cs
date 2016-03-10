namespace CryptoPool.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUniqueIdentifierToPool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pools", "UniqueIdentifier", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pools", "UniqueIdentifier");
        }
    }
}
