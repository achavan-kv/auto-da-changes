using Blue.Cosacs.Credit.Extensions;
using System;

namespace Blue.Cosacs.Credit.Model
{
    public class Applicant2
    {
        public Applicant2()
        { 
        }
        //public static explicit operator Credit.ProposalApplicant2(Applicant2 x)
        //{
        //    return 
        //}

        internal Applicant2(Credit.ProposalApplicant2 a)
        {
            this.FirstName = a.FirstName;
            this.LastName = a.LastName;
            this.Title = a.Title;
            this.Alias = a.Alias;
            this.DateOfBirth = a.DateOfBirth;
            this.Gender = a.Gender;
            this.CustomerId = a.CustomerId;
        }

        public Credit.ProposalApplicant2 ToTable(int prosposalId)
        {
            return new Credit.ProposalApplicant2()
            {
                FirstName = this.FirstName.SafeTrim(),
                LastName = this.LastName.SafeTrim(),
                Title = this.Title.SafeTrim(),
                Alias = this.Alias.SafeTrim(),
                DateOfBirth = this.DateOfBirth,
                Gender = this.Gender.SafeTrim(),
                ProposalId = prosposalId,
                CustomerId = this.CustomerId
            };
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CustomerId { get; set; }
    }
}
