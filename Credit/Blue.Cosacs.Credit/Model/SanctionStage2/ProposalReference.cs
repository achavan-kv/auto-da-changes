using Blue.Cosacs.Credit.Extensions;

namespace Blue.Cosacs.Credit.Model.SanctionStage2
{
    public class ProposalReference
    {
        public int Id { get; set; }
        public bool? IsFamily { get; set; }
        public string Relationship { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? YearsKnown { get; set; }
        public int ProposalId { get; set; }
        public string Email { get; set; }
        public string HomeAddress { get; set; }
        public string HomePhone { get; set; }
        public string WorkAddress { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public bool Verified { get; set; }
        public string Comments { get; set; }
        public string ReferenceDescription { get; set; }
        public string HomeDirections { get; set; }
        public bool Filled { get; set; }

        public ProposalReference()
        {
        }

        internal ProposalReference(Credit.ProposalReference proposalRef, bool filled = false)
        {
            this.Id = proposalRef.Id;
            this.IsFamily = proposalRef.IsFamily;
            this.Relationship = proposalRef.Relationship;
            this.FirstName = proposalRef.FirstName;

            this.LastName = proposalRef.LastName;
            this.YearsKnown = proposalRef.YearsKnown;
            this.ProposalId = proposalRef.ProposalId;
            this.Email = proposalRef.Email;

            this.HomeAddress = proposalRef.HomeAddress;
            this.HomePhone = proposalRef.HomePhone;
            this.WorkAddress = proposalRef.WorkAddress;
            this.WorkPhone = proposalRef.WorkPhone;

            this.MobilePhone = proposalRef.MobilePhone;
            this.Verified = proposalRef.Verified;
            this.Comments = proposalRef.Comments;
            this.ReferenceDescription = proposalRef.ReferenceDescription;

            this.HomeDirections = proposalRef.HomeDirections;
            this.Filled = filled;
        }

        public Credit.ProposalReference ToTable(int proposalId, bool isApplicant2)
        {
            return new Credit.ProposalReference()
            {
                IsFamily = this.IsFamily,
                Relationship = this.Relationship.SafeTrim(),
                FirstName = this.FirstName.SafeTrim(),
                LastName = this.LastName.SafeTrim(),
                YearsKnown = this.YearsKnown,
                ProposalId = this.ProposalId,
                Email = this.Email.SafeTrim(),
                HomeAddress = this.HomeAddress.SafeTrim(),
                HomePhone = this.HomePhone.SafeTrim(),
                WorkAddress = this.WorkAddress.SafeTrim(),
                WorkPhone = this.WorkPhone.SafeTrim(),
                MobilePhone = this.MobilePhone.SafeTrim(),
                Verified = this.Verified,
                Comments = this.Comments.SafeTrim(),
                ReferenceDescription = this.ReferenceDescription.SafeTrim(),
                HomeDirections = this.HomeDirections.SafeTrim(),
                IsApplicant2 = isApplicant2
            };
        }
    }
}
