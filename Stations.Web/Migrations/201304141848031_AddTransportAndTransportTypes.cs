namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransportAndTransportTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transports",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Number = c.String(),
                        TypeID = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TransportTypes",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransportTypes");
            DropTable("dbo.Transports");
        }
    }
}
