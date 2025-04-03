using System.Data.Entity;
using Blue.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blue.Cosacs.Warehouse
{
    using System.Data.Entity.Validation;
    using System.Linq;

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
			modelBuilder.Entity<PickingView>().HasKey(t => t.Id);
			modelBuilder.Entity<ZonePriorityView>().HasKey(z => z.code);
			modelBuilder.Entity<PickListView>().HasKey(p => p.BookingId);
			modelBuilder.Entity<LoadView>().HasKey(l => l.Id);
			modelBuilder.Entity<ScheduleView>().HasKey(l => l.BookingId);
			modelBuilder.Entity<Booking>().Ignore(b => b.Status);
			modelBuilder.Entity<Booking>().Property(e => e.CurrentQuantity).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

			modelBuilder.Entity<DriverCommissionView>().HasKey(l => new
			{
				l.ScheduleId,
				l.PickingId
			});
            modelBuilder.Entity<TruckConfirmedView>().HasKey(l => l.TruckId);       // #13677
			//.WithRequired().HasForeignKey(t => t.PickingId);
			//modelBuilder.Entity<PicklistView>().HasKey(v => new { v.Id, v.BookingId });
			//modelBuilder.Entity<Table1>().HasKey(t => t.Col1).ToTable("Table1");
			//modelBuilder.Entity<EntitiesNs.CustAcct>().HasKey(t => new { t.acctno, t.custid }).ToTable("CustAcct");
		}

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
}
