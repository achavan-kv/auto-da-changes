using System;
using System.Data;
using STL.Common;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{

    public partial class Payment : CommonForm
    {
        string LoyaltyAcct;
        bool IsLoyalty = false;
        decimal LoyaltyAmount = 0;

        private void LoyaltyGetCharges(string custid)
        {
            decimal amount = 0;
            string acctno = "";
            bool active = false;
            if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
            {
                CustomerManager.LoyaltyGetCharges(custid, ref acctno, ref amount, ref active);
                LoyaltyAcct = acctno;
                LoyaltyAmount = amount;
                CheckLogo(active);

            }
        }

        private void CheckLogo(bool active)
        {
            if (LoyaltyAcct != null && LoyaltyAcct.Length > 0)
            {
                LoyaltyLogo_pb.Visible = active;

                if (LoyaltyAmount > 0)
                {
                    Loyalty_lbl.Visible = true;
                }
                else
                {
                    Loyalty_lbl.Visible = false;
                }
                IsLoyalty = true;
            }
        }

        private void LoyaltyUpdateHCC(ref DataView Payments)
        {
            if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
            {
                if (IsLoyalty)
                {
                    foreach (DataRow row in Payments.Table.Rows)
                    {
                        if (row[CN.AcctNo].ToString() == LoyaltyAcct)
                        {
                            row[CN.AccountType] = "HCC";
                        }

                    }
                }
            }
        }

    }
}