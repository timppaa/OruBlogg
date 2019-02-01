namespace OruBloggen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormelPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "IsFormel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryModels", "IsFormel");
        }
    }
}
