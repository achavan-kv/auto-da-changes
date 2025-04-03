using Blue.Transactions;
using StructureMap;

namespace Blue.Cosacs.Merchandising
{
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;

    public abstract partial class ContextBase : DbContextBase
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }

    public class ReportingContext : Context
    {
        public ReportingContext() : base("Report") { }

        public static ReadScope<ReportingContext> Read()
        {
            return ObjectFactory.TryGetInstance<ReadScope<ReportingContext>>() ?? new ReadScope<ReportingContext>();
        }
    }

    public class Context : ContextBase
    {
        public Context() : this("Default") { }

        public Context(string connectionString) : base(connectionString) { }

        public static ReadScope<Context> Read()
        {
            return ObjectFactory.TryGetInstance<ReadScope<Context>>() ?? new ReadScope<Context>();
        }

        public static WriteScope<Context> Write()
        {
            return new WriteScope<Context>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StockReceivedReportStockCountView>().HasKey(c => c.Id);
            modelBuilder.Entity<ProductsOnOrder>().HasKey(c => new { PurchaseOrderId = c.PurchaseOrderId, ProductId = c.ProductId});
            modelBuilder.Entity<LocationStockLevelView>().HasKey(c => new { ProductId = c.ProductId, LocationId = c.LocationId });
            modelBuilder.Entity<ForceMerchandiseStockLevelsView>().HasKey(c => new { ProductId = c.ProductId, LocationId = c.LocationId });
            modelBuilder.Entity<AssociatedProductsHierarchyView>().HasKey(c => new { Division = c.Division, Department = c.Department, Class = c.Class});
        }
    }
}
