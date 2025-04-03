using Blue.Cosacs.Credit.Extensions;
using System;

namespace Blue.Cosacs.Credit.Model
{
    public class Applicant1
    {
        public Applicant1()
        {
        }

        internal Applicant1(Credit.Proposal a)
        {
            this.Alias = a.Alias;
            this.Branch = a.Branch;
            this.ApplicationType = a.ApplicationType;
            this.CreatedBy = a.CreatedBy;
            this.CreatedOn = a.CreatedOn;
            this.Email = a.Email;
            this.FirstName = a.FirstName;
            this.HomePhone = a.HomePhone;
            this.LastName = a.LastName;
            this.MobilePhone = a.MobilePhone;
            this.Title = a.Title;
            this.Source = a.Source;
            this.ApplicationStage = a.ApplicationStage;
            this.CustomerId = a.CustomerId;
            this.UpdatedOn = a.UpdatedOn;
            this.UpdatedBy = a.UpdatedBy;
            this.WorkPhone = a.WorkPhone;
        }

        public Credit.Proposal ToTable()
        {
            return new Credit.Proposal()
             {
                 Alias = this.Alias.SafeTrim(),
                 Branch = this.Branch,
                 ApplicationType = this.ApplicationType.SafeTrim(),
                 CreatedBy = this.CreatedBy,
                 CreatedOn = this.CreatedOn,
                 Email = this.Email.SafeTrim(),
                 FirstName = this.FirstName.SafeTrim(),
                 HomePhone = this.HomePhone.SafeTrim(),
                 LastName = this.LastName.SafeTrim(),
                 MobilePhone = this.MobilePhone.SafeTrim(),
                 Title = this.Title.SafeTrim(),
                 Source = this.Source.SafeTrim(),
                 CustomerId = this.CustomerId,
                 UpdatedBy = this.UpdatedBy,
                 UpdatedOn = this.UpdatedOn,
                 WorkPhone = this.WorkPhone
             };
        }

        public short Branch { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string ApplicationType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ApplicationStage { get; set; }
        public string Source { get; set; }
        public int? CustomerId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string WorkPhone { get; set; }
    }
}
