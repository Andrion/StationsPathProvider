namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GroupID = c.Guid(nullable: false),
                        Code = c.Int(nullable: false),
                        Name = c.String(),
                        FullName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stations");
            DropTable("dbo.Groups");
        }
    }
}
