namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStartEndForTransport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transports", "StartID", c => c.Guid(nullable: false));
            AddColumn("dbo.Transports", "EndID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transports", "EndID");
            DropColumn("dbo.Transports", "StartID");
        }
    }
}
