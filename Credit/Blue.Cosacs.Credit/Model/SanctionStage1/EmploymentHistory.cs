using Blue.Cosacs.Credit.Extensions;
using Newtonsoft.Json;
using System;

namespace Blue.Cosacs.Credit.Model.SanctionStage1
{
    public class EmploymentHistory
    {
        public EmploymentHistory()
        {
        }

        internal EmploymentHistory(Credit.EmploymentHistory e)
        {
            this.Occupation = e.Occupation;
            this.DateStart = e.DateStart;
            this.DateEnd = e.DateEnd;
            this.IsApplicant2 = e.IsApplicant2;
            this.EmployerName = e.EmployerName;
        }

        internal Credit.EmploymentHistory ToTable(int proposalId)
        {
            return new Credit.EmploymentHistory()
            {
                Occupation = this.Occupation.SafeTrim(),
                DateEnd = this.DateEnd,
                DateStart = this.DateStart,
                ProposalId = proposalId,
                IsApplicant2 = this.IsApplicant2,
                EmployerName = this.EmployerName.SafeTrim()
            };
        }

        public int ProposalId { get; set; }
        [JsonProperty("occupation")]
        public string Occupation { get; set; }
        [JsonProperty("dateStart")]
        public DateTime DateStart { get; set; }
        [JsonProperty("dateEnd")]
        public DateTime DateEnd { get; set; }
        [JsonProperty("isApplicant2")]
        public bool IsApplicant2 { get; set; }
        [JsonProperty("employerName")]
        public string EmployerName { get; set; }
    }
}
