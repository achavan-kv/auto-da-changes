using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Customer
{
    public partial class DataForReindexSolr
    {
        internal List<CustomerSearchResult> Fill()
        {
            var result = new List<CustomerSearchResult>();
            var reader = this.ExecuteReader();

            while (reader.Read())
            {
                var newRecord = new CustomerSearchResult();

                newRecord.TotalRows = reader.GetInt32(reader.GetOrdinal("TotalRows"));
                newRecord.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                newRecord.CustomerId = reader.GetString(reader.GetOrdinal("CustomerId"));
                newRecord.Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? string.Empty : reader.GetString(reader.GetOrdinal("Title"));
                newRecord.FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FirstName"));
                newRecord.LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastName"));
                newRecord.Alias = reader.IsDBNull(reader.GetOrdinal("Alias")) ? string.Empty : reader.GetString(reader.GetOrdinal("Alias"));
                newRecord.DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("DOB"));
                newRecord.HomePhoneNumber = reader.IsDBNull(reader.GetOrdinal("HomePhoneNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("HomePhoneNumber"));
                newRecord.MobileNumber = reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("MobileNumber"));
                newRecord.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email"));
                newRecord.HomeAddressLine1 = reader.IsDBNull(reader.GetOrdinal("HomeAddressLine1")) ? string.Empty : reader.GetString(reader.GetOrdinal("HomeAddressLine1"));
                newRecord.HomeAddressLine2 = reader.IsDBNull(reader.GetOrdinal("HomeAddressLine2")) ? string.Empty : reader.GetString(reader.GetOrdinal("HomeAddressLine2"));
                newRecord.City = reader.IsDBNull(reader.GetOrdinal("City")) ? string.Empty : reader.GetString(reader.GetOrdinal("City"));
                newRecord.HasRCreditSource = reader.GetBoolean(reader.GetOrdinal("HasRCreditSource"));
                newRecord.RCreditSourceDate = reader.IsDBNull(reader.GetOrdinal("RCreditSourceDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("RCreditSourceDate"));
                newRecord.HasOCreditSource = reader.GetBoolean(reader.GetOrdinal("HasOCreditSource"));
                newRecord.OCreditSourceDate = reader.IsDBNull(reader.GetOrdinal("OCreditSourceDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("OCreditSourceDate"));
                newRecord.HasCashSource = reader.GetBoolean(reader.GetOrdinal("HasCashSource"));
                newRecord.CashSourceDate = reader.IsDBNull(reader.GetOrdinal("CashSourceDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CashSourceDate"));
                newRecord.HasStoreCardSource = reader.GetBoolean(reader.GetOrdinal("HasStoreCardSource"));
                newRecord.StoreCardSourceDate = reader.IsDBNull(reader.GetOrdinal("StoreCardSourceDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("StoreCardSourceDate"));
                newRecord.HasWarrantySource = reader.GetBoolean(reader.GetOrdinal("HasWarrantySource"));
                newRecord.WarrantySourceDate = reader.IsDBNull(reader.GetOrdinal("WarrantySourceDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("WarrantySourceDate"));
                newRecord.HasInstallationSource = reader.GetBoolean(reader.GetOrdinal("HasInstallationSource"));
                newRecord.InstallationSourceDate = reader.IsDBNull(reader.GetOrdinal("InstallationSourceDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("InstallationSourceDate"));
                newRecord.AvailableSpend = reader.IsDBNull(reader.GetOrdinal("AvailableSpend")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AvailableSpend"));
                newRecord.CustomerBranchNo = reader.IsDBNull(reader.GetOrdinal("CustomerBranchNo")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("CustomerBranchNo"));
                newRecord.CustomerBranchName = reader.IsDBNull(reader.GetOrdinal("CustomerBranchName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CustomerBranchName"));
                newRecord.SalesPersonId = reader.IsDBNull(reader.GetOrdinal("SalesPersonId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SalesPersonId"));
                newRecord.SalesPerson = reader.IsDBNull(reader.GetOrdinal("SalesPerson")) ? string.Empty : reader.GetString(reader.GetOrdinal("SalesPerson"));
                newRecord.DateLastBought = reader.IsDBNull(reader.GetOrdinal("DateLastBought")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("DateLastBought"));
                newRecord.CustomerSource = reader.GetString(reader.GetOrdinal("CustomerSource"));
                newRecord.DoNotCallAgain = reader.IsDBNull(reader.GetOrdinal("DoNotCallAgain")) ? (Boolean?)null : reader.GetBoolean(reader.GetOrdinal("DoNotCallAgain"));
                newRecord.CalledAt = reader.IsDBNull(reader.GetOrdinal("CalledAt")) ? string.Empty : reader.GetDateTime(reader.GetOrdinal("CalledAt")).ToSolrDate();
                newRecord.HasPendingCalls = reader.GetBoolean(reader.GetOrdinal("HasPendingCalls"));
                newRecord.DateLastBoughtRange = reader.IsDBNull(reader.GetOrdinal("DateLastBoughtRange")) ? string.Empty : reader.GetString(reader.GetOrdinal("DateLastBoughtRange"));
                newRecord.IsActiveCashCustomer = reader.GetBoolean(reader.GetOrdinal("IsActiveCashCustomer"));
                newRecord.IsActiveCreditCustomer = reader.GetBoolean(reader.GetOrdinal("IsActiveCreditCustomer"));
                newRecord.CustomerCallType = reader.IsDBNull(reader.GetOrdinal("CustomerCallType")) ? string.Empty : reader.GetString(reader.GetOrdinal("CustomerCallType"));
                newRecord.HasBirthday = reader.IsDBNull(reader.GetOrdinal("HasBirthday")) ? string.Empty : reader.GetString(reader.GetOrdinal("HasBirthday"));
                newRecord.ReceiveEmails = reader.GetBoolean(reader.GetOrdinal("ReceiveEmails"));
                newRecord.ResieveSms = reader.GetBoolean(reader.GetOrdinal("ResieveSms"));
                newRecord.LastEmailSentOn = reader.IsDBNull(reader.GetOrdinal("LastEmailSentOn")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEmailSentOn"));
                newRecord.LastSmsSentOn = reader.IsDBNull(reader.GetOrdinal("LastSmsSentOn")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastSmsSentOn"));

                result.Add(newRecord);
            }

            return result;
        }
    }

    public partial class CustomerSearchResult
    {
        public int TotalRows
        {
            get;
            set;
        }
        public long ID
        {
            get;
            set;
        }
        public string CustomerId
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Alias
        {
            get;
            set;
        }
        public DateTime? DOB
        {
            get;
            set;
        }
        public string HomePhoneNumber
        {
            get;
            set;
        }
        public string MobileNumber
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string HomeAddressLine1
        {
            get;
            set;
        }
        public string HomeAddressLine2
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }
        public string PostCode
        {
            get;
            set;
        }
        public bool HasRCreditSource
        {
            get;
            set;
        }
        public DateTime? RCreditSourceDate
        {
            get;
            set;
        }
        public bool HasOCreditSource
        {
            get;
            set;
        }
        public DateTime? OCreditSourceDate
        {
            get;
            set;
        }
        public bool HasCashSource
        {
            get;
            set;
        }
        public DateTime? CashSourceDate
        {
            get;
            set;
        }
        public bool HasStoreCardSource
        {
            get;
            set;
        }
        public DateTime? StoreCardSourceDate
        {
            get;
            set;
        }
        public bool HasWarrantySource
        {
            get;
            set;
        }
        public DateTime? WarrantySourceDate
        {
            get;
            set;
        }
        public bool HasInstallationSource
        {
            get;
            set;
        }
        public DateTime? InstallationSourceDate
        {
            get;
            set;
        }
        public int? AvailableSpend
        {
            get;
            set;
        }
        public short? CustomerBranchNo
        {
            get;
            set;
        }
        public string CustomerBranchName
        {
            get;
            set;
        }
        public int? SalesPersonId
        {
            get;
            set;
        }
        public string SalesPerson
        {
            get;
            set;
        }
        public DateTime? DateLastBought
        {
            get;
            set;
        }
        public string CustomerSource
        {
            get;
            set;
        }
        public bool? DoNotCallAgain
        {
            get;
            set;
        }
        public string CalledAt
        {
            get;
            set;
        }
        public bool HasPendingCalls
        {
            get;
            set;
        }
        public string DateLastBoughtRange
        {
            get;
            set;
        }
        public bool IsActiveCashCustomer
        {
            get;
            set;
        }
        public bool IsActiveCreditCustomer
        {
            get;
            set;
        }
        public string CustomerCallType
        {
            get;
            set;
        }
        public string HasBirthday
        {
            get;
            set;
        }
        public bool ReceiveEmails
        {
            get;
            set;
        }
        public bool ResieveSms { get; set; }
        public DateTime? LastEmailSentOn
        {
            get;
            set;
        }
        public DateTime? LastSmsSentOn
        {
            get;
            set;
        }
        public static explicit operator CustomerSearchResult(CustomerSearchView value)
        {
            return new CustomerSearchResult
            {
                AvailableSpend = value.AvailableSpend,
                City = value.City,
                CustomerId = value.CustomerId,
                CustomerSource = value.CustomerSource,
                Email = value.Email,
                FirstName = value.FirstName,
                HasCashSource = value.HasCashSource == 1,
                HasInstallationSource = value.HasInstallationSource.Value,
                HasOCreditSource = value.HasOCreditSource == 1,
                HasRCreditSource = value.HasRCreditSource == 1,
                HasStoreCardSource = value.HasStoreCardSource == 1,
                HasWarrantySource = value.HasWarrantySource.Value,
                HomeAddressLine1 = value.HomeAddressLine1,
                HomeAddressLine2 = value.HomeAddressLine2,
                HomePhoneNumber = value.HomePhoneNumber,
                ID = value.ID.Value,
                InstallationSourceDate = value.InstallationSourceDate,
                LastName = value.LastName,
                MobileNumber = value.MobileNumber,
                PostCode = value.PostCode,
                SalesPersonId = value.SalesPersonId,
                Title = value.Title,
                TotalRows = 0,
                WarrantySourceDate = value.WarrantySourceDate,
                ReceiveEmails = true,
                ResieveSms = true,
                LastEmailSentOn = null,
                LastSmsSentOn = null
            };
        }
    }
}
