using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Loans.Schedule
{
    public class LoanScheduleResponseModel
    {
        public LoanScheduleModel Data { get; set; }
        public ApiResult Result { get; set; }
    }

    public class LoanScheduleModel
    {
        public Currency Currency { get; set; }
        public List<LoanScheduleInstallment> Installments { get; set; }
        public LoanAccountDetail LoanAccountDetails { get; set; }
    }
}
