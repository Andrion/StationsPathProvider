namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReplacementsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Replacements",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Value = c.String(),
                        ReplaceValue = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Replacements");
        }
    }
}
