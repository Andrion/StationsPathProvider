namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMapStationAddHandledStation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HandledStations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        StationID = c.Guid(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Stations", "Lat", c => c.Double(nullable: false));
            AddColumn("dbo.Stations", "Lng", c => c.Double(nullable: false));
            DropTable("dbo.GStations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GStations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Lat = c.Double(nullable: false),
                        Lng = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropColumn("dbo.Stations", "Lng");
            DropColumn("dbo.Stations", "Lat");
            DropTable("dbo.HandledStations");
        }
    }
}
