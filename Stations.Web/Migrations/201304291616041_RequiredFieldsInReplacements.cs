namespace Stations.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFieldsInReplacements : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Replacements", "Value", c => c.String(nullable: false));
            AlterColumn("dbo.Replacements", "ReplaceValue", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Replacements", "ReplaceValue", c => c.String());
            AlterColumn("dbo.Replacements", "Value", c => c.String());
        }
    }
}
