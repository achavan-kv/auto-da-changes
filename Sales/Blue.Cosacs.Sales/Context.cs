using System.Data.Entity;
using Blue.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blue.Cosacs.Sales
{
   public partial class Context : ContextBase
    {
        public static ReadScope<Context> Read()
        {
            return new ReadScope<Context>();
        }

        public static WriteScope<Context> Write()
        {
            return new WriteScope<Context>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LinkedContractsSetupView>().HasKey(l => new { l.Contract, l.ItemNo, l.Category });

            modelBuilder.Entity<Order>().HasMany<OrderItem>(s => s.Items)
                .WithRequired(s => s.Order).HasForeignKey(s => s.OrderId);

            modelBuilder.Entity<Order>().HasMany<OrderPayment>(s => s.Payments)
                .WithRequired(s => s.Order).HasForeignKey(s => s.OrderId);

            modelBuilder.Entity<OrderCustomer>().HasRequired(s => s.Order);

            modelBuilder.Entity<OrderItem>().HasMany<OrderItem>(s => s.OrderItems)
                .WithRequired(s => s.ParentItem).HasForeignKey(s => s.ParentId);

            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(18, 4));
        }
    }
}
