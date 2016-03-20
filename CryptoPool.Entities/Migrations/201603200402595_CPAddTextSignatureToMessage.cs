namespace CryptoPool.Entities.Models.Migrations.CP
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CPAddTextSignatureToMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "TextSignature", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "TextSignature");
        }
    }
}
