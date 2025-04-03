using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{
    #region  CustomerSearch    
    public partial class CustomerSearch : Blue.Transactions.Command<ContextBase>
    {
        public CustomerSearch() : base("dbo.VE_CustomerSearch")
        {
            base.AddInParameter("@CustId", DbType.String);
            base.AddInParameter("@FirstName", DbType.String);
            base.AddInParameter("@LastName", DbType.String);
            base.AddInParameter("@PostalCode", DbType.String);
            base.AddInParameter("@PhoneNumber", DbType.String);
        }
    }

    partial class CustomerSearch
    {
        public new void Fill(DataSet ds)
        {
            base.Fill(ds);
        }

        public void Fill(DataSet ds, Model.CustomerRequest customerRequest)
        {
            this.CustId = customerRequest.CustId;
            this.FirstName = customerRequest.FirstName;
            this.LastName = customerRequest.LastName;
            this.PostalCode = customerRequest.PostalCode;
            this.PhoneNumber = customerRequest.PhoneNumber;
            Fill(ds);
        }

        public string CustId
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }

        public string FirstName
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }

        public string LastName
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }

        public string PostalCode
        {
            get { return (string)base[3]; }
            set { base[3] = value; }
        }

        public string PhoneNumber
        {
            get { return (string)base[4]; }
            set { base[4] = value; }
        }

        //public string PhoneNumber
        //{
        //    get { return (string)base[4]; }
        //    set { base[4] = value; }
        //}
    }
    #endregion
}
