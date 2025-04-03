using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Blue.Transactions;

namespace Unicomer.Cosacs.Repository
{


    public partial class ContextBase : DbContextBase
    {
        public const string ConnectionStringName = "Default";
        public ContextBase()
            : base(ConnectionStringName)
        {
        }

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
        }
        //public ContextBase()
        //    : base("name=ContextBase")
        //{
        //}

        //public virtual DbSet<customer> customers { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.custid)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.otherid)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.name)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.firstname)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.title)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.alias)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.addrsort)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.namesort)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.sex)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.ethnicity)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.morerewardsno)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.IdNumber)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.IdType)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.RFCreditLimit)
        //        .HasPrecision(19, 4);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.RFCardPrinted)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.maidenname)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.OldRFCreditLimit)
        //        .HasPrecision(19, 4);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.LimitType)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.AvailableSpend)
        //        .HasPrecision(19, 4);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.ScoringBand)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.InstantCredit)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.StoreType)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.maritalstat)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.Nationality)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.ScoreCardType)
        //        .IsFixedLength()
        //        .IsUnicode(false);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.StoreCardLimit)
        //        .HasPrecision(19, 4);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.StoreCardAvailable)
        //        .HasPrecision(19, 4);

        //    modelBuilder.Entity<customer>()
        //        .Property(e => e.CashLoanBlocked)
        //        .IsFixedLength()
        //        .IsUnicode(false);
        //}
    }
}
