namespace Blue.Cosacs.SalesManagement.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blue.Cosacs.SalesManagement.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Blue.Cosacs.SalesManagement.Context context)
        {
        }
    }
}
