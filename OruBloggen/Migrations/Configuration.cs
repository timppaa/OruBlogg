namespace OruBloggen.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<OruBloggen.Models.OruBloggenDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(OruBloggen.Models.OruBloggenDbContext context)
        {
            context.Teams.AddOrUpdate(
                new TeamModel() { TeamName = "Informatik" },
                new TeamModel() { TeamName = "Ekonomi" },
                new TeamModel() { TeamName = "Statistik" },
                new TeamModel() { TeamName = "Juridik" }
                );

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }

    }
}
