using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Blue.Cosacs.Credit
{
    public partial class Context
    {
        public virtual DbSet<ScoreCard> ScoreCard { get; set; }
    }

    [Serializable]
    public partial class ScoreCard
    {
        public int Id { get; set; }
        public string CardType { get; set; }
        public string Card { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ScoreCardModelBuilder
    {
        public static void Build(DbModelBuilder modelBuilder)
        {
            var scoreCardTable = modelBuilder.Entity<ScoreCard>();
            scoreCardTable.ToTable("ScoreCard", "Credit");
            scoreCardTable.HasKey(t => t.Id);
            scoreCardTable.Property(t => t.Id)
            .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);

            scoreCardTable.Property(t => t.CardType)
            .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None)
            .IsUnicode(false)
            .HasMaxLength(20);

            scoreCardTable.Property(t => t.Card)
            .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None);

            scoreCardTable.Property(t => t.CreatedBy)
            .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None);

            scoreCardTable.Property(t => t.CreatedOn)
           .HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.None);
        }
    }
}
