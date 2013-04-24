namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reorganize : DbMigration
    {
        public override void Up()
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
            
            CreateTable(
                "dbo.Directions",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        TransportID = c.Guid(nullable: false),
                        StartID = c.Guid(nullable: false),
                        EndID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DirectionStations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DirectionID = c.Guid(nullable: false),
                        StationID = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        TransportID = c.Guid(nullable: false),
                        StationID = c.Guid(nullable: false),
                        IsWeekend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.ScheduleItems", "ScheduleID", c => c.Guid(nullable: false));
            DropColumn("dbo.Stations", "GroupID");
            DropColumn("dbo.Stations", "FullName");
            DropColumn("dbo.Transports", "Number");
            DropColumn("dbo.Transports", "StartID");
            DropColumn("dbo.Transports", "EndID");
            DropColumn("dbo.ScheduleItems", "StationID");
            DropTable("dbo.Groups");
            DropTable("dbo.IFStations");
        }
        
        public override void Down()
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
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.ScheduleItems", "StationID", c => c.Guid(nullable: false));
            AddColumn("dbo.Transports", "EndID", c => c.Guid(nullable: false));
            AddColumn("dbo.Transports", "StartID", c => c.Guid(nullable: false));
            AddColumn("dbo.Transports", "Number", c => c.String());
            AddColumn("dbo.Stations", "FullName", c => c.String());
            AddColumn("dbo.Stations", "GroupID", c => c.Guid(nullable: false));
            DropColumn("dbo.ScheduleItems", "ScheduleID");
            DropTable("dbo.Schedules");
            DropTable("dbo.DirectionStations");
            DropTable("dbo.Directions");
            DropTable("dbo.GStations");
        }
    }
}
