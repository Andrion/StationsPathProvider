namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIFStationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IFStations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        IFID = c.Int(nullable: false),
                        Name = c.String(),
                        Lat = c.Double(nullable: false),
                        Lng = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IFStations");
        }
    }
}
