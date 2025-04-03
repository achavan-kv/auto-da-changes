namespace Blue.Cosacs.NonStocks.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blue.Cosacs.NonStocks.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Blue.Cosacs.NonStocks.Context context)
        {
        }
    }
}
