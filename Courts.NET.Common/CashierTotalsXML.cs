using System;
using STL.Common.Printing.AgreementPrinting;
using System.Data;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;

namespace STL.Common
{
	/// <summary>
	/// Summary description for CashierTotalsXML.
	/// </summary>
	public class CashierTotalsXML : PrintXML
	{
		public CashierTotalsXML(string country)
		{
			this.XmlTemplate = XMLTemplates.CashierTotalsXML;
			this.CountryName = country;
		}

		public void AddTransactions( DataSet totals, string numberFormat, bool listCheques )
		{
			XmlNode transactions = _doc.DocumentElement.SelectSingleNode("//TRANSACTIONS");
			
			if(transactions!=null)
			{
				XmlNode transaction = transactions.SelectSingleNode("//TRANSACTION");
				transactions.RemoveChild(transaction);

				if(listCheques)
				{
					foreach(DataRow r in totals.Tables[1].Rows)
					{
						transaction = transaction.CloneNode(true);
						transaction.SelectSingleNode("ACCTNO").InnerText = Convert.ToString(r[CN.AccountNumber]);
						transaction.SelectSingleNode("CUSTNAME").InnerText = Convert.ToString(r[CN.name]);
						transaction.SelectSingleNode("CHEQUENO").InnerText = Convert.ToString(r[CN.ChequeNo]);
						transaction.SelectSingleNode("BANKCODE").InnerText = Convert.ToString(r[CN.BankCode]);
						transaction.SelectSingleNode("BANKACCTNO").InnerText = Convert.ToString(r[CN.BankAccountNo2]);
						transaction.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.TransValue]).ToString();
						transactions.AppendChild(transaction);
					}
				}
				else
				{
					foreach(DataRow r in totals.Tables[1].Rows)
					{
						transaction = transaction.CloneNode(true);
						transaction.SelectSingleNode("EMPLOYEE").InnerText = Convert.ToString(r[CN.EmployeeNo.ToLower()]);
						transaction.SelectSingleNode("TRANSTYPE").InnerText = Convert.ToString(r[CN.TransTypeCode]);
						transaction.SelectSingleNode("PAYMETHOD").InnerText = Convert.ToString(r[CN.PayMethod]);
						transaction.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.TransValue]).ToString();
						transactions.AppendChild(transaction);
					}
                    //IP - 29/01/09 - 70634 - loop through each row in the 'CashierDeposits' table to include the 
                    //deposits in the 'transactions' node if the deposit is a disbursements. This will then be used
                    //to populate the 'Other' on the CDR printout.
                    foreach (DataRow r in totals.Tables[0].Rows)
                    {
                        //If the deposit is a disbursement then add to 'transaction' node. 
                        //IP - 12/03/09 - (70634) - was previously excluding other transtypes which could have been 
                        //setup as other deposit types such as (Petty Cash-tax exempt), (Disbursement-tax exempt).
                        //Therefore select all where the 'IsDeposit' > 1. This directly relates to the 'Include as Deposit Transaction'
                        //drop down on the 'Transtype Maintenance' screen and the selected index of the deposit type.
                        if ((Convert.ToInt32(r[CN.IsDeposit]) > 1))
                        {
                            transaction = transaction.CloneNode(true);
						    transaction.SelectSingleNode("EMPLOYEE").InnerText = Convert.ToString(r[CN.EmployeeNo.ToLower()]);
						    transaction.SelectSingleNode("TRANSTYPE").InnerText = Convert.ToString(r[CN.TransTypeCode]);
						    transaction.SelectSingleNode("PAYMETHOD").InnerText = Convert.ToString(r[CN.Code]);
						    transaction.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.DepositValue] * -1).ToString();
						    transactions.AppendChild(transaction);
                        }
                    }
				}
			}
		}

		public void AddValues(DataSet ds, bool history)
		{
			XmlNode totalVals = _doc.DocumentElement.SelectSingleNode("//TOTALVALS");
			
			if(totalVals!=null)
			{
				XmlNode total = totalVals.SelectSingleNode("//TOTAL");
				totalVals.RemoveChild(total);

				foreach(DataRow r in ds.Tables[0].Rows)
				{
					total = total.CloneNode(true);

					total.SelectSingleNode("PAYMETHOD").InnerText = (string)r[CN.PayMethod];
					total.SelectSingleNode("SYSTEMTOTAL").InnerText = ((decimal)r[CN.SystemTotal]).ToString("F2");
					total.SelectSingleNode("USERTOTAL").InnerText = ((decimal)r[CN.UserTotal]).ToString("F2");
					total.SelectSingleNode("DIFFERENCE").InnerText = ((decimal)r[CN.Difference]).ToString("F2");
					total.SelectSingleNode("DEPOSIT").InnerText = ((decimal)r[CN.Deposit]).ToString("F2");
					total.SelectSingleNode("REASON").InnerText = (string)r[CN.Reason];
					total.SelectSingleNode("DESCRIPTION").InnerText = (string)r[CN.CodeDescript];
					totalVals.AppendChild(total);
				}
			}
		}
	}
}
