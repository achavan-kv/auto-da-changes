using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Blue.Cosacs.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Blue.Cosacs.Sales.Repositories
{
    public class WarrantyContractRepository : IWarrantyContractRepository
    {
        public IEnumerable<IEnumerable<DocumentCopy<WarrantyContractDetailsResult>>> GetWarrantyContractDetails(
            string agreementNo1, string[] contractNos, bool multiple = false)
        {
            var retList = new List<IEnumerable<DocumentCopy<WarrantyContractDetailsResult>>>();

            Int64 val = 0;
            Int64.TryParse(agreementNo1, out val);
            int? agreementNo = 0;
            if (Convert.ToString(val).Length >= 14)
            {
                agreementNo = GetSalesOrderID(agreementNo1);
            }
            else
            {
                agreementNo = Convert.ToInt32(agreementNo1);
            }            

            foreach (var contractNo in contractNos)
            {
                if (!string.IsNullOrEmpty(contractNo))
                {
                    var copyLst = GetWarrantyContractDetails(agreementNo, contractNo, multiple);

                    retList.Add(copyLst);
                }
            }

            return retList;
        }

        private IEnumerable<DocumentCopy<WarrantyContractDetailsResult>> GetWarrantyContractDetails(
            int? agreementNo, string contractNo, bool multiple)
        {
            var ds = new DataSet();
            var settings = new Settings();
            var localFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            var decimalplaces = settings.DecimalPlaces;
            localFormat.CurrencySymbol = settings.CurrencySymbolForPrint;            

            new WarrantyContractDetails() { agreementNo = agreementNo, contractNo = contractNo }.Fill(ds);

            var contracts = from DataRow b in ds.Tables[0].Rows
                            select new WarrantyContractDetailsResult
                            {
                                ItemNo = b.Field<string>("ItemNo"),
                                WarrantyNo = b.Field<string>("WarrantyNo"),
                                BranchNo = b.Field<short>("BranchNo"),
                                BranchName = b.Field<string>("BranchName"),
                                EmployeeNo = b.Field<int>("EmployeeNo"),
                                AgreementDate = b.Field<DateTime>("AgreementDate"),
                                ItemDescription = b.Field<string>("ItemDescription"),
                                WarrantyLength = b.Field<byte?>("WarrantyLength"),
                                ItemPrice = b.Field<decimal>("ItemPrice").ToString(decimalplaces, localFormat),
                                WarrantyPrice = b.Field<decimal>("WarrantyPrice").ToString(decimalplaces, localFormat),
                                WarrantyDescription = b.Field<string>("WarrantyDescription"),
                                EmployeeName = b.Field<string>("EmployeeName"),
                                AccountNo = b.Field<string>("AccountNo"),
                                CustomerTitle = b.Field<string>("CustomerTitle"),
                                CustomerFirstName = b.Field<string>("CustomerFirstName"),
                                CustomerLastName = b.Field<string>("CustomerLastName"),
                                CustomerAddressLine1 = b.Field<string>("CustomerAddressLine1"),
                                CustomerAddressLine2 = b.Field<string>("CustomerAddressLine2"),
                                CustomerAddressLine3 = b.Field<string>("CustomerAddressLine3"),
                                CustomerPostCode = b.Field<string>("CustomerPostCode"),
                                CustomerMobilePhone = b.Field<string>("CustomerMobilePhone"),
                                CustomerHomePhone = b.Field<string>("CustomerHomePhone"),
                                ContractNo = contractNo,
                                WarrantyCredit = 30, //2DO: Get this
                                //Copy = "Customer Copy", //2DO: Get this
                                IsLast = false
                            };

            return CreateCopies(contracts, settings, multiple);
        }

        private IEnumerable<DocumentCopy<WarrantyContractDetailsResult>> CreateCopies(
            IEnumerable<WarrantyContractDetailsResult> contracts,
            Settings settings,
            bool multiple)
        {
            var creditCopies = multiple ? settings.WarrantyCreditCopy : 1;
            var custCopies = multiple ? settings.WarrantyCustCopy : 0;
            var officeCopies = multiple ? settings.WarrantyHoCopy : 0;

            foreach (var contract in contracts)
            {
                foreach (var i in Enumerable.Range(1, creditCopies))
                {
                    yield return new DocumentCopy<WarrantyContractDetailsResult>
                    {
                        Document = contract,
                        CopyText = "Credit Department Copy",
                        CountryName = settings.CountryName,
                    };
                }

                foreach (var i in Enumerable.Range(1, custCopies))
                {
                    yield return new DocumentCopy<WarrantyContractDetailsResult>
                    {
                        Document = contract,
                        CopyText = "Customer Copy",
                        CountryName = settings.CountryName,
                    };
                }

                foreach (var i in Enumerable.Range(1, officeCopies))
                {
                    yield return new DocumentCopy<WarrantyContractDetailsResult>
                    {
                        Document = contract,
                        CopyText = "Head Office Copy",
                        CountryName = settings.CountryName,
                    };
                }
            }
        }

        //Suvidha
        public int? GetSalesOrderID(string agreementinvoicenumber)
        {
            using (var scope = Context.Read())
            {             
                var originalOrder = scope.Context.Order.Where(o => o.AgreementInvoiceNumber == agreementinvoicenumber).SingleOrDefault();

                return originalOrder != null ? originalOrder.Id : (int?)0;
            }
        }
    }
}
