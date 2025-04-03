using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;

/// <summary>
/// This is the wrapper for Oracle Interface objects
/// </summary>
///
namespace STL.BLL.OracleIntegration
{
    public class DataContainer
    {
        // Required in this order
        public Customer[] customers;
        public ARInvoice [] invoice;
        public Receipt[] receipts;

    }
}
