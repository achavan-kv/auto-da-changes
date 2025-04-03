using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DLineItemBfCollection.
    /// </summary>
    public class DLineItemBfCollection : DALObject
    {
        private string _acctNo = "";
        public string AccountNumber
        {
            get{return _acctNo;}
            set{_acctNo = value;}
        }
        private int _agreementNo = 1;
        public int AgreementNumber
        {
            get{return _agreementNo;}
            set{_agreementNo = value;}
        }
        private string _itemNo = "";
        public string ItemNumber
        {
            get{return _itemNo;}
            set{_itemNo = value;}
        }

        //IP/NM - 18/05/11 -CR1212 - #3627 
        private int _itemID = 0;
        public int ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        private double _quantity = 0;
        public double Quantity
        {
            get{return _quantity;}
            set{_quantity = value;}
        }
        private decimal _price = 0;
        public decimal Price
        {
            get{return _price;}
            set{_price = value;}
        }
        private decimal _orderVal = 0;
        public decimal OrderValue
        {
            get{return _orderVal;}
            set{_orderVal = value;}
        }

        private string _contractNo = "";
        public string ContractNo
        {
            get{return _contractNo;}
            set{_contractNo = value;}
        }

        private string _warrantyNo = "";
        public string WarrantyNo
        {
            get { return _warrantyNo; }
            set { _warrantyNo = value; }
        }

        private short _warrantyLocn = 0;
        public short WarrantyLocn
        {
            get { return _warrantyLocn; }
            set { _warrantyLocn = value; }
        }

        private string _exchangedWarrantyContractNo = "";
        public string ExchangedWarrantyContractNo
        {
            get { return _exchangedWarrantyContractNo; }
            set { _exchangedWarrantyContractNo = value; }
        }

        private DataTable _itemdetails;
        public DataTable ItemDetails
        {
            get	{return _itemdetails;}
        }

        // Constructors
        public DLineItemBfCollection()
        {
		
        }

        // Methods
        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);              //IP - 06/06/11 - CR1212 - RI - #3806
                parmArray[2].Value = this.ItemID;
                parmArray[3] = new SqlParameter("@quantity", SqlDbType.Float);
                parmArray[3].Value = this.Quantity;
                parmArray[4] = new SqlParameter("@price", SqlDbType.Money);
                parmArray[4].Value = this.Price;
                parmArray[5] = new SqlParameter("@orderValue", SqlDbType.Money);
                parmArray[5].Value = this.OrderValue;
                parmArray[6] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
                parmArray[6].Value = this.ContractNo;

                this.RunSP(conn, trans, "DN_LineItemBfCollectionSaveSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void GetLineItemBfCollection(SqlConnection conn, SqlTransaction trans,
                                            string accountNo, 
                                            int agreementNo,
                                            int itemID,                              //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
                                            string contractNo,
                                            out bool isCancellation)
        {
            _itemdetails = new DataTable();
            isCancellation = false;

            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar,12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@agrmtno", SqlDbType.Int);
                parmArray[1].Value = agreementNo;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);           //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
                parmArray[2].Value = itemID;
                parmArray[3] = new SqlParameter("@contractno", SqlDbType.NVarChar,10);
                parmArray[3].Value = contractNo;
                parmArray[4] = new SqlParameter("@cancellation", SqlDbType.SmallInt);
                parmArray[4].Direction = ParameterDirection.Output;

                if (conn != null && trans != null)
                    this.RunSP(conn, trans, "DN_LineItemBfCollectionGetSP", parmArray, _itemdetails);
                else
                    this.RunSP("DN_LineItemBfCollectionGetSP", parmArray, _itemdetails);

                // Should actually only be one row returned
                foreach(DataRow row in _itemdetails.Rows)
                {				
                    this.AccountNumber = accountNo;
                    this.AgreementNumber = agreementNo;
                    //this.ItemNumber = itemNo;
                    this.ItemID = itemID;                           //IP/NM - 18/05/11 -CR1212 - #3627        
                    this.ContractNo = contractNo;
                    this.Quantity	= (double)row[CN.Quantity];
                    this.Price		= (decimal)row[CN.Price];
                    this.OrderValue	= (decimal)row[CN.OrdVal];
                    this.WarrantyNo = (string)row[CN.WarrantyNo];
                    this.WarrantyLocn = (short)row[CN.WarrantyLocation];
                    this.ExchangedWarrantyContractNo = (string)row[CN.ExchangeContractNo];
                }

                if (!Convert.IsDBNull(parmArray[4].Value))
                    isCancellation = (Convert.ToInt16(parmArray[4].Value) > 0);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void Delete(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@acctNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
                parmArray[1].Value = this.AgreementNumber;
                parmArray[2] = new SqlParameter("@itemID", SqlDbType.Int);                  //IP - 06/06/11 - CR1212 - RI - #3806
                parmArray[2].Value = this.ItemID;                                       
                parmArray[3] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
                parmArray[3].Value = this.ContractNo;

                this.RunSP(conn, trans, "DN_LineItemBfCollectionDeleteSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
    }
}