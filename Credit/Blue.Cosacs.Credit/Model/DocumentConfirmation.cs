using Blue.Cosacs.Credit.Extensions;
using System;

namespace Blue.Cosacs.Credit.Model
{
    public class DocumentConfirmation
    {
        public int ProposalId { get; set; }

        public string CompulsoryIdType { get; set; }
        public string CompulsoryId { get; set; }
        public string CompulsoryIdNotes { get; set; }

        public string OptionalIdType { get; set; }
        public string OptionalId { get; set; }

        public string ProofAddress { get; set; }
        public string ProofAddressNotes { get; set; }
        public string ProofIncome { get; set; }
        public string ProofIncomeNotes { get; set; }

        public string ProofIncomeOther { get; set; }
        public string ProofIncomeOtherNotes { get; set; }
        public string References { get; set; }
        public string LandlordDetails { get; set; }

        public string SpouseDetails { get; set; }
        public string GovernmentEmployees { get; set; }
        public string OverseasPerson { get; set; }
        public bool Completed { get; set; }

        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public Credit.DocumentConfirmation ToTable()
        {
            return new Credit.DocumentConfirmation()
            {
                GovernmentEmployees = this.GovernmentEmployees.SafeTrim(),
                LandlordDetails = this.LandlordDetails.SafeTrim(),
                OptionalId = this.OptionalId.SafeTrim(),
                OptionalIdType = this.OptionalIdType.SafeTrim(),
                OverseasPerson = this.OverseasPerson.SafeTrim(),
                ProofAddress = this.ProofAddress.SafeTrim(),
                ProofAddressNotes = this.ProofAddressNotes.SafeTrim(),
                CompulsoryId = this.CompulsoryId.SafeTrim(),
                CompulsoryIdType = this.CompulsoryIdType.SafeTrim(),
                CompulsoryIdNotes = this.CompulsoryIdNotes.SafeTrim(),
                ProofIncomeNotes = this.ProofIncomeNotes.SafeTrim(),
                ProofIncomeOther = this.ProofIncomeOther.SafeTrim(),
                ProofIncomeOtherNotes = this.ProofIncomeOtherNotes.SafeTrim(),
                ProposalId = this.ProposalId,
                References = this.References.SafeTrim(),
                SpouseDetails = this.SpouseDetails.SafeTrim(),
                Comments = this.Comments,
                ProofIncome = this.ProofIncome
            };
        }
    }

    public class File
    {
        public string FileName { get; set; }
        public Guid Guid { get; set; }
        public string UploadName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
