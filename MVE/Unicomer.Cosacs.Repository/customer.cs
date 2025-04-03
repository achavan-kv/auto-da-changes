
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unicomer.Cosacs.Repository
{

    [Table("customer")]
    public partial class customer
    {
        public short? origbr { get; set; }

        [Key]
        [StringLength(20)]
        public string custid { get; set; }

        [StringLength(15)]
        public string otherid { get; set; }

        public short branchnohdle { get; set; }

        [Required]
        [StringLength(60)]
        public string name { get; set; }

        [StringLength(30)]
        public string firstname { get; set; }

        [StringLength(25)]
        public string title { get; set; }

        [StringLength(25)]
        public string alias { get; set; }

        [StringLength(20)]
        public string addrsort { get; set; }

        [StringLength(20)]
        public string namesort { get; set; }

        public DateTime dateborn { get; set; }

        [StringLength(1)]
        public string sex { get; set; }

        [StringLength(1)]
        public string ethnicity { get; set; }

        [Required]
        [StringLength(16)]
        public string morerewardsno { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? effectivedate { get; set; }

        [Required]
        [StringLength(30)]
        public string IdNumber { get; set; }

        [Required]
        [StringLength(4)]
        public string IdType { get; set; }

        public byte creditblocked { get; set; }

        [Column(TypeName = "money")]
        public decimal RFCreditLimit { get; set; }

        public byte RFCardSeqNo { get; set; }

        [Required]
        [StringLength(1)]
        public string RFCardPrinted { get; set; }

        public DateTime? datelastscored { get; set; }

        public DateTime? RFDateReminded { get; set; }

        public int empeenochange { get; set; }

        public DateTime? datechange { get; set; }

        [StringLength(30)]
        public string maidenname { get; set; }

        [Column(TypeName = "money")]
        public decimal OldRFCreditLimit { get; set; }

        [Required]
        [StringLength(1)]
        public string LimitType { get; set; }

        [Column(TypeName = "money")]
        public decimal AvailableSpend { get; set; }

        [StringLength(4)]
        public string ScoringBand { get; set; }

        [Required]
        [StringLength(1)]
        public string InstantCredit { get; set; }

        [Required]
        [StringLength(1)]
        public string StoreType { get; set; }

        public bool LoanQualified { get; set; }

        public int dependants { get; set; }

        [Required]
        [StringLength(1)]
        public string maritalstat { get; set; }

        [Required]
        [StringLength(4)]
        public string Nationality { get; set; }

        public byte? recurringarrears { get; set; }

        [Required]
        [StringLength(1)]
        public string ScoreCardType { get; set; }

        public bool? StoreCardApproved { get; set; }

        [Column(TypeName = "money")]
        public decimal? StoreCardLimit { get; set; }

        [Column(TypeName = "money")]
        public decimal? StoreCardAvailable { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? SCardApprovedDate { get; set; }

        [StringLength(1)]
        public string CashLoanBlocked { get; set; }

        public bool CashLoanNew { get; set; }

        public bool CashLoanRecent { get; set; }

        public bool CashLoanExisting { get; set; }

        public bool CashLoanStaff { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool? ResieveSms { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? age { get; set; }
    }
}
