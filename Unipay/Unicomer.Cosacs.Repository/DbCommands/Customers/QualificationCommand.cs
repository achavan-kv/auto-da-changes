/* 
Author: Swati -IGT
Date: Feb 10 2022
Description:JM BlueStart-Qualification API 
 */
using System;
using System.Data;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands.Customers
{
    public class QualificationCommand : Blue.Transactions.Command<ContextBase>
    {
        string salesCustomerId;
        string countryIsoCode;

        public QualificationCommand(string countryIsoCode, string salesCustomerId) : base("dbo.SP_BS_GetCollectDataQualification")
        {
            this.salesCustomerId = salesCustomerId;
            this.countryIsoCode = countryIsoCode;

            base.AddInParameter("@Custid", DbType.String);
            base.AddInParameter("@ISOCountryCode", DbType.String);
            base.AddOutParameter("@AccountBalanceTotal", DbType.Double, 10);
            base.AddOutParameter("@ActivedAccountQuantity", DbType.Int16, 5);
            base.AddOutParameter("@PurgedAccountQuantity", DbType.Int16, 5);
            base.AddOutParameter("@DaysPastDue", DbType.Int16, 5);
            base.AddOutParameter("@ActivatedCashQuantity", DbType.Int16, 5);
            base.AddOutParameter("@BalanceArrear", DbType.Double, 10);
            base.AddOutParameter("@WorstArrearDays", DbType.String, 50);
            base.AddOutParameter("@WorstQualification", DbType.String, 1);
            base.AddOutParameter("@ValueCMM", DbType.Double, 10);
            base.AddOutParameter("@ValueLEM", DbType.Double, 10);
            base.AddOutParameter("@Message", DbType.String, 100);
            base.AddOutParameter("@Error", DbType.Boolean, 1);

            base[0] = salesCustomerId;
            base[1] = countryIsoCode;
        }

        public QualificationResponseModel Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            string message = "";
            bool isError = false;

            var result = new QualificationResponseModel();

            try
            {
                result.Qualification = new QualificationModel
                {
                    VarCalculationData = new VarCalculationData { AccountData = new AccountData { } },
                    VarRulesData = new VarRulesData { },
                    VarScoringData = new VarScoringData { },
                    VarCreditData = new VarCreditData { },
                };

                base.ExecuteNonQuery();

                if (base[2] != null && base[2] != DBNull.Value)
                {
                    result.Qualification.VarScoringData.AccountBalanceTotal = Convert.ToDecimal(base[2]);
                }

                if (base[3] != null && base[3] != DBNull.Value)
                {
                    result.Qualification.VarScoringData.ActivedAccountQuantity = Convert.ToInt16(base[3]);
                }

                if (base[4] != null && base[4] != DBNull.Value)
                {
                    result.Qualification.VarScoringData.PurgedAccountQuantity = Convert.ToInt16(base[4]);
                }

                if (base[5] != null && base[5] != DBNull.Value)
                {
                    result.Qualification.VarRulesData.DaysPastDue = Convert.ToInt16(base[5]);
                }

                if (base[6] != null && base[6] != DBNull.Value)
                {
                    result.Qualification.VarRulesData.ActivatedCashQuantity = Convert.ToInt16(base[6]);
                }

                if (base[7] != null && base[7] != DBNull.Value)
                {
                    result.Qualification.VarRulesData.BalanceArrear = Convert.ToDecimal(base[7]);
                }

                if (base[8] != null && base[8] != DBNull.Value)
                {
                    result.Qualification.VarCalculationData.AccountData.WorstArrearDays = base[8].ToString();
                }

                if (base[9] != null && base[9] != DBNull.Value)
                {
                    result.Qualification.VarCalculationData.AccountData.WorstQualification = base[9].ToString();
                }

                if (base[10] != null && base[10] != DBNull.Value)
                {
                    result.Qualification.VarCreditData.ValueCMM = Convert.ToDecimal(base[10]);
                }

                if (base[11] != null && base[11] != DBNull.Value)
                {
                    result.Qualification.VarCreditData.ValueLEM = Convert.ToDecimal(base[11]);
                }

                if (base[12] != null && base[12] != DBNull.Value)
                {
                    message = base[12].ToString();
                }

                if (base[13] != null && base[13] != DBNull.Value)
                {
                    isError = (bool)base[13];
                }

                if (isError)
                {
                    throw new ValidationException(new ErrorMessage { Id = this.salesCustomerId, Detail = message },
                        new { this.salesCustomerId, Sp = "dbo.SP_BS_GetCollectDataQualification" });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetCollectDataQualification", salesCustomerId, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetCollectDataQualification", salesCustomerId, ex.Message);
                throw new DBException(ex, new { this.salesCustomerId, Sp = "dbo.SP_BS_GetCollectDataQualification" });
            }

            return result;
        }
    }
}

