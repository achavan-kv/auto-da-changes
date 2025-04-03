using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BTermsType.
    /// </summary>
    public class BTermsType : CommonObject
    {
        public BTermsType()
        {
        }


        public DataSet LoadTermsTypeDetails(string termsType)
        {
            DTermsType tt = new DTermsType();
            return tt.LoadTermsTypeDetails(termsType);
        }


        public void SaveTermsTypeDetails(SqlConnection conn, SqlTransaction trans,
            string termsType, DataSet tersmTypeDetails, int user)
        {
            DTermsType tt = new DTermsType();

            foreach (DataTable dt in tersmTypeDetails.Tables)
            {
                switch (dt.TableName)
                {
                    case TN.TermsType:
                        foreach (DataRow r in dt.Rows)
                        {
                            tt.TermsType = termsType;
                            tt.Description = (string)r[CN.Description];
                            tt.MonthInterestFree = (short)r[CN.MonthsIntFree];
                            tt.InstalPreDelivery = (string)r[CN.InstalPreDel];
                            tt.Affinity = (string)r[CN.Affinity];
                            tt.NoArrearsLetters = (short)r[CN.NoArrearsLetters];
                            tt.DefaultDeposit = (double)r[CN.DefaultDeposit];
                            tt.DepositIsPercentage = (bool)r[CN.DepositIsPercentage];
                            tt.IsActive = (short)r[CN.IsActive];
                            tt.PaymentHolidays = (bool)r[CN.PaymentHoliday];
                            tt.AgreementText = (string)r[CN.AgreementText];
                            tt.MinTerm = (int)r[CN.MinTerm];
                            tt.MaxTerm = (int)r[CN.MaxTerm];
                            tt.DefaultTerm = (int)r[CN.DefaultTerm];
                            tt.CashBackMonth = (short)r[CN.CashBackMonth];
                            tt.CashBackPc = (short)r[CN.CashBackPc];
                            tt.CashBackAmount = System.Convert.ToDecimal(r[CN.CashBackAmount]);
                            tt.AgreementPrint = (string)r[CN.AgreementPrint];
                            tt.DeferredMonths = (short)r[CN.DeferredMonths];
                            tt.FullRebateDays = (short)r[CN.FullRebateDays];
                            tt.STCPc = (short)r[CN.STCPc];
                            tt.STCAmount = System.Convert.ToDecimal(r[CN.STCAmount]);
                            tt.DelNonStocks = Convert.ToBoolean(r[CN.DeliverNonStocks]);
                            tt.APR = (string)r[CN.APR];
                            tt.DoNotSecuritise = (short)r[CN.DoNotSecuritise];
                            tt.StoreType = r[CN.StoreType].ToString();
                            //tt.pClubTier1 = (string)r[CN.PClubTier1];
                            //tt.pClubTier2 = (string)r[CN.PClubTier2];
                            tt.IsLoan = (bool)r[CN.IsLoan];         //CR906 jec
                            tt.LoanNewCustomer = (bool)r["LoanNewCustomer"];
                            tt.LoanRecentCustomer = (bool)r["LoanRecentCustomer"];
                            tt.LoanExistingCustomer = (bool)r["LoanExistingCustomer"];
                            tt.LoanStaff = (bool)r["LoanStaff"];
                            tt.IsMmiActive = (bool)r[CN.IsMmiActive];
                            tt.MmiThresholdPercentage = (double)r[CN.MmiThresholdPercentage];
                            tt.SaveTermsTypeDetails(conn, trans, user);
                        }
                        break;

                    case TN.TermsTypeAccountType:
                        dt.AcceptChanges();
                        tt.DeleteTermsTypeAccountTypes(conn, trans, termsType);

                        foreach (DataRow r in dt.Rows)
                        {
                            tt.SaveTermsTypeAccountType(conn, trans,
                                (string)r[CN.TermsType],
                                (string)r[CN.AccountType]);
                        }
                        break;

                    case TN.IntRateHistory:
                        dt.AcceptChanges();
                        tt.DeleteAllIntRateHistory(conn, trans, termsType);

                        foreach (DataRow r in dt.Rows)
                        {
                            tt.SaveIntRateHistory(conn, trans,
                                (string)r[CN.TermsType],
                                (DateTime)r[CN.DateFrom],
                                (DateTime)r[CN.DateTo],
                                (double)r[CN.IntRate],
                                (double)r[CN.InsPcent],
                                (double)r[CN.AdminPcent],
                                (short)r[CN.InsIncluded],
                                (short)r[CN.IncludeWarranty],
                                (string)r[CN.RateType],
                                (string)r[CN.Band],
                                (short)r[CN.PointsFrom],
                                (short)r[CN.PointsTo],
                                user,
                                (double)r["AdminValue"]);
                        }
                        break;

                    case TN.TermsTypeLength:
                        dt.AcceptChanges();
                        tt.DeleteAllTermsTypeLengths(conn, trans, termsType);

                        foreach (DataRow r in dt.Rows)
                        {
                            tt.SaveTermsTypeLength(conn, trans,
                                (string)r[CN.TermsType],
                                (int)r[CN.Length]);
                        }
                        break;

                    case TN.TermsTypeFreeInstallments:
                        dt.AcceptChanges();
                        tt.DeleteAllFreeInstallments(conn, trans, termsType);

                        foreach (DataRow r in dt.Rows)
                        {
                            tt.SaveFreeInstallment(conn, trans,
                                (string)r[CN.TermsType],
                                (DateTime)r[CN.IntRateFrom],
                                (DateTime)r[CN.IntRateTo],
                                (DateTime)r[CN.DateFrom],
                                (DateTime)r[CN.DateTo],
                                (int)r[CN.Month]);
                        }
                        break;

                    case TN.TermsTypeVariableRates:
                        dt.AcceptChanges();
                        tt.DeleteAllVariableRates(conn, trans, termsType);

                        foreach (DataRow r in dt.Rows)
                        {
                            tt.SaveVariableRate(conn, trans,
                                (string)r[CN.TermsType],
                                (DateTime)r[CN.IntRateFrom],
                                (DateTime)r[CN.IntRateTo],
                                (int)r[CN.FromMonth],
                                (int)r[CN.ToMonth],
                                System.Convert.ToDecimal(r[CN.Rate]));
                        }
                        break;

                    default:
                        break;
                }
            }
        }


        public DataSet TermsTypeBandsOverview()
        {
            DataSet ttOverviewSet = new DataSet();
            DTermsType tt = new DTermsType();
            DataTable ttOverviewList = tt.GetTermsTypeBandsOverview();

            // The list needs to be presented as one row per terms type
            DataTable ttOverviewGrid = new DataTable(TN.TTOverview);
            ttOverviewGrid.Columns.AddRange(
                       new DataColumn[] {new DataColumn(CN.TermsType),
										 new DataColumn(CN.Description),
										 new DataColumn(CN.InsPcent, Type.GetType("System.Decimal"))
									 });

            // First sort by band to add the columns in band order
            DataRow[] ttBandRows = ttOverviewList.Select("Band <> ''", "Band ASC");
            for (int i = 0; i < ttBandRows.Length; i++)
            {
                string bandName = (string)ttBandRows[i][CN.Band];
                string scName = bandName + " " + GetResource("T_SCPC");
                string totalName = bandName + " " + GetResource("T_TOTAL");

                if (!ttOverviewGrid.Columns.Contains(totalName))
                {
                    // Add a new band to the columns
                    ttOverviewGrid.Columns.AddRange(
                       new DataColumn[] {new DataColumn(scName, Type.GetType("System.Decimal")),
										 new DataColumn(totalName, Type.GetType("System.Decimal"))
									 });
                }
            }

            // Sort by terms type to add rows in terms type order
            DataRow[] ttRows = ttOverviewList.Select("Band <> ''", "TermsType ASC, Band ASC");
            // Create a report with one row per terms type
            string lastTermsType = "~";
            for (int i = 0; i < ttRows.Length; i++)
            {
                string bandName = (string)ttRows[i][CN.Band];
                string scName = bandName + " " + GetResource("T_SCPC");
                string totalName = bandName + " " + GetResource("T_TOTAL");

                string curTermsType = (string)ttRows[i][CN.TermsType];
                if (curTermsType != lastTermsType)
                {
                    // Add a new terms type to the rows
                    lastTermsType = curTermsType;
                    DataRow newRow = ttOverviewGrid.NewRow();
                    newRow[CN.TermsType] = (string)ttRows[i][CN.TermsType];
                    newRow[CN.Description] = (string)ttRows[i][CN.Description];
                    newRow[CN.InsPcent] = Convert.ToDecimal(ttRows[i][CN.InsPcent]);
                    newRow[scName] = Convert.ToDecimal(ttRows[i][CN.ServiceChargePC]);
                    newRow[totalName] = Convert.ToDecimal(ttRows[i][CN.ServiceChargePC]);
                    if (Convert.ToInt16(ttRows[i][CN.InsIncluded]) == 0)
                        newRow[totalName] = (decimal)newRow[totalName] + Convert.ToDecimal(ttRows[i][CN.InsPcent]);
                    ttOverviewGrid.Rows.Add(newRow);
                }
                else
                {
                    // Update the band for an existing terms type
                    int ttRowIndex = ttOverviewGrid.Rows.Count - 1;
                    ttOverviewGrid.Rows[ttRowIndex][scName] = Convert.ToDecimal(ttRows[i][CN.ServiceChargePC]);
                    ttOverviewGrid.Rows[ttRowIndex][totalName] = Convert.ToDecimal(ttRows[i][CN.ServiceChargePC]);
                    if (Convert.ToInt16(ttRows[i][CN.InsIncluded]) == 0)
                        ttOverviewGrid.Rows[ttRowIndex][totalName] = (decimal)ttOverviewGrid.Rows[ttRowIndex][totalName] + Convert.ToDecimal(ttRows[i][CN.InsPcent]);
                }
            }

            ttOverviewSet.Tables.Add(ttOverviewGrid);
            return ttOverviewSet;
        }


        public void TermsTypeBandsAdjust(SqlConnection conn, SqlTransaction trans, 
            DateTime adjustDate, decimal adjustIns, decimal adjustSC, int user)
        {
            DTermsType tt = new DTermsType();
            tt.TermsTypeBandsAdjust(adjustDate, adjustIns, adjustSC, user);
        }

    }
}

