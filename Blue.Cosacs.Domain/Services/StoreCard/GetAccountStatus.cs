using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetAccountStatusRequest  
	{
        public string acctno;
        //public SqlConnection conn;
        //public SqlTransaction trans;

	}
	
	partial class GetAccountStatusResponse 
	{
        public string status;
	}
}
