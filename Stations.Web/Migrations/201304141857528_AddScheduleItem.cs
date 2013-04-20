namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScheduleItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduleItems",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        StationID = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScheduleItems");
        }
    }
}
