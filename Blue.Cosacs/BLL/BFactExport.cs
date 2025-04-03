using Blue.Cosacs;
using Blue.Cosacs.Repositories;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;
using Message = Blue.Cosacs.Messages.Merchandising;



namespace STL.BLL
{
    using System.Linq;

    /// <summary>
    /// Summary description for BFactExport. This class used to export delivery and financial data
    /// to fact 2000 and also to update the summary control totals.
    /// </summary>

    public class BFactExport : CommonObject
    {
        #region properties and declarations
        private int _batchcount = 0;
        public int BatchCount
        {
            get { return _batchcount; }
            set { _batchcount = value; }
        }
        private double _quantity = 0;
        public double Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        private decimal _price = 0;
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }
        private decimal _value = 0;
        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private short _previousstocklocn = 0;
        public short PreviousStocklocn
        {
            get { return _previousstocklocn; }
            set { _previousstocklocn = value; }
        }
        private string _previoussalesperson = "";
        public string PreviousSalesPerson
        {
            get { return _previoussalesperson; }
            set { _previoussalesperson = value; }
        }



        private int _rowcount = 0;
        public int rowCount
        {
            get { return _rowcount; }
            set { _rowcount = value; }
        }
        private int _factrunno = 0;
        public int Factrunno
        {
            get { return _factrunno; }
            set { _factrunno = value; }
        }
        private int _summaryrunno = 0;
        public int SummaryRunno
        {
            get { return _summaryrunno; }
            set { _summaryrunno = value; }
        }

        private int _category = 0;
        public int Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private string _headerwarehouseno = "";
        public string HeaderWarehouseNo
        {
            get { return _headerwarehouseno; }
            set { _headerwarehouseno = value; }
        }
        private string _previousheaderwarehouseno = "";
        public string PreviousHeaderWarehouseNo
        {
            get { return _previousheaderwarehouseno; }
            set { _previousheaderwarehouseno = value; }
        }
        private string _linewarehouseno = "";
        public string LineWarehouseno
        {
            get { return _linewarehouseno; }
            set { _linewarehouseno = value; }
        }


        private string _tccode = "";
        public string TCCode
        {
            get { return _tccode; }
            set { _tccode = value; }
        }
        private string _facttrantype = "";
        public string FactTrantype
        {
            get { return _facttrantype; }
            set { _facttrantype = value; }
        }
        private string _sourceofattraction = "";
        public string SourceOfAttraction
        {
            get { return _sourceofattraction; }
            set { _sourceofattraction = value; }
        }

        private string _previoussourceofattraction = "";
        public string PreviousSourceOfAttraction
        {
            get { return _previoussourceofattraction; }
            set { _previoussourceofattraction = value; }
        }




        private decimal _batchvalue = 0;
        public decimal BatchValue
        {
            get { return _batchvalue; }
            set { _batchvalue = value; }
        }
        private decimal _totaltaxvalue = 0;
        public decimal TotalTaxValue
        {
            get { return _totaltaxvalue; }
            set { _totaltaxvalue = value; }
        }
        private decimal _totalfinancialvalue = 0;
        public decimal TotalFinancialValue
        {
            get { return _totalfinancialvalue; }
            set { _totalfinancialvalue = value; }
        }

        private decimal _taxvalue = 0;
        public decimal TaxValue
        {
            get { return _taxvalue; }
            set { _taxvalue = value; }
        }

        private decimal _linetaxvalue = 0;
        public decimal LineTaxValue
        {
            get { return _linetaxvalue; }
            set { _linetaxvalue = value; }
        }

        private decimal _totalfacttaxvalue = 0;
        public decimal TotalFACTTaxValue
        {
            get { return _totalfacttaxvalue; }
            set { _totalfacttaxvalue = value; }
        }



        private decimal _deltotal = 0;
        public decimal DelTotal
        {
            get { return _deltotal; }
            set { _deltotal = value; }
        }
        private decimal _orderanddelstotal = 0;
        public decimal OrdersandDeliveriesTotal
        {
            get { return _orderanddelstotal; }
            set { _orderanddelstotal = value; }
        }


        private decimal _nettval = 0;
        public decimal NettValue
        {
            get { return _nettval; }
            set { _nettval = value; }
        }


        private string _previousacctNo = "";
        public string PreviousAcctNo
        {
            get { return _previousacctNo; }
            set { _previousacctNo = value; }
        }
        private string _previousAgrmtno = "";
        public string PreviousAgreementNo
        {
            get { return _previousAgrmtno; }
            set { _previousAgrmtno = value; }
        }

        private string _acctNo = "";
        public string AcctNo
        {
            get { return _acctNo; }
            set { _acctNo = value; }
        }

        private string _buffno = "";
        public string Buffno
        {
            get { return _buffno; }
            set { _buffno = value; }
        }
        private string _previousbuffno = "";
        public string PreviousBuffno
        {
            get { return _previousbuffno; }
            set { _previousbuffno = value; }
        }

        private string _previoustccode = "";
        public string PreviousTCCode
        {
            get { return _previoustccode; }
            set { _previoustccode = value; }
        }

        public XDocument cintXML
        {
            get { return cintXML; }
            set { cintXML = value; }
        }
        /*public decimal TaxValue
        {
            get{return _taxval;}
            set{_taxval = value;}
        }*/
        #endregion
        private DataSet ds = null;
        private DDelivery NonStock = new DDelivery();

        private DataTable tab = new DataTable();


        public DataSet Address
        {
            get { return ds; }
        }
        public void StoreValues(DataRow row)
        {
            this.TCCode = row[CN.TCCode].ToString();
            if (row[CN.BuffNo].ToString().Length > 7)
                this.Buffno = row[CN.BuffNo].ToString();
            else
                this.Buffno = row[CN.BuffBranchNo].ToString() + row[CN.BuffNo].ToString();

            this.Price = Convert.ToDecimal(row[CN.Price]);
            this.Quantity = Convert.ToDouble(row[CN.Quantity]);
            this.Value = Convert.ToDecimal(row[CN.Value]);
            this.Category = Convert.ToInt32(row[CN.Category]);
            this.LineWarehouseno = row[CN.LineWareHouseNo].ToString();
            this.AcctNo = row[CN.AcctNo].ToString();

        }
        public void RenameFiles(string file)
        {
            string newfilename = "";
            string oldfilename = "";
            oldfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + "5.dat";
            newfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + "5.dat";
            if (File.Exists(oldfilename))
                File.Delete(oldfilename);

            for (int i = 5; i > 1; i--)
            {
                oldfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + i.ToString() + ".dat";
                newfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + (i - 1).ToString() + ".dat";
                if (File.Exists(newfilename))
                    File.Move(newfilename, oldfilename);

            }

            oldfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + "1.dat";
            newfilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + file + ".dat";
            if (File.Exists(newfilename))
                File.Move(newfilename, oldfilename);


        }

        public void WriteCintToHub(DFactExport export, SqlConnection conn, SqlTransaction tran)
        {

            var cint = new Message.Cints.CintSubmit();
            decimal total = 0;
            decimal delTotal = 0;

            cint.RunNo = this.Factrunno;

            var orders = new List<Message.Cints.CintOrder>();

            var deliveries = export.DeliveryLineItems.AsEnumerable();
            var discounts = (from d in deliveries.Where(x => x.Field<bool?>(CN.Discount) == true 
                                                        && !string.IsNullOrWhiteSpace(x[CN.ParentItemNo].ToString()))
                                                        //x.Field<string>(CN.ParentItemNo)))
                             group d by new { 
                                              ParentItemNo = d[CN.ParentItemNo].ToString(),
                                              ParentLocation = d[CN.ParentLocation].ToString(),
                                              Acctno = d[CN.AcctNo].ToString()
                                            }
                             into g
                             select new
                             {
                                 value = Convert.ToDecimal(g.Sum(x => x.Field<double>(CN.Value))).RoundTo(this.DecimalPlacesNo),
                                 key = new Tuple<string, string, string> (g.Key.ParentItemNo, g.Key.ParentLocation, g.Key.Acctno)
                             }).ToDictionary(d => d.key);
            
            foreach (DataRow row in export.DeliveryLineItems.Rows)
            {
                string secondaryReference = "";
                string referenceType = "";
                string transactionType = "";
                string saleType = "";
                decimal taxAmount = 0;
                var price = Convert.ToDecimal(row[CN.Price].ToString()).RoundTo(this.DecimalPlacesNo);
                var quantity = Convert.ToInt32(row[CN.Quantity].ToString());
                var stockLocn = row[CN.StockLocn].ToString();
                decimal? cashPrice = null;
                int parentId = 0;
                
                if (row[CN.ItemType].ToString() != "S")
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row[CN.CashPrice].ToString()))
                {
                    cashPrice = Convert.ToDecimal(row[CN.CashPrice].ToString()).RoundTo(this.DecimalPlacesNo);
                }
                                
                if (row[CN.ParentItemId].ToString() != "")
                {
                    parentId = Convert.ToInt32(row[CN.ParentItemId].ToString());
                }

                switch (row[CN.FACTTranType].ToString())
                {
                    case "01":
                        if (row[CN.Quantity].ToString() != "0" && row[CN.TCCode].ToString() == "61")
                        {
                            transactionType = "RegularOrder";
                        }
                        break;
                    case "03":
                        if (row[CN.DelOrColl].ToString() == "R")
                        {
                            transactionType = "Redelivery";
                        }
                        else
                        {
                            transactionType = "Delivery";
                        }
                        break;
                    case "04":
                        transactionType = "CancelOrder";
                        break;
                    case "13":
                        switch (row[CN.TCCode].ToString())
                        {
                            case "11":
                                transactionType = "Return";
                                stockLocn = row[CN.RetStockLocn].ToString();
                                break;
                            case "32":
                                transactionType = "OrderAndDelivery";
                                break;
                            case "17":
                                transactionType = "Repossession";
                                stockLocn = row[CN.RetStockLocn].ToString();
                                break;
                                //ignore "14" as this is rebates
                        }
                        break;
                }

                if (transactionType == "")
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row[CN.ServiceRequestNo].ToString()))
                {
                    secondaryReference = row[CN.ServiceRequestNo].ToString();
                    referenceType = "Service Request";
                }
                else if (row[CN.AgreementNo].ToString() == "1")
                {
                    secondaryReference = row[CN.BuffNo].ToString();
                    referenceType = "Delivery";
                }
                else
                {
                    secondaryReference = row[CN.AgreementNo].ToString();
                    referenceType = "Invoice";
                }

                if (row[CN.AcctNo].ToString().Substring(3, 1) == "4" || row[CN.AcctNo].ToString().Substring(3, 1) == "5")
                {
                    saleType = "Cash";
                }
                else
                {
                    saleType = "Credit";
                }

                var currentItemKey = new Tuple<string, string, string> (row[CN.ItemNo].ToString(), 
                                                                        row[CN.StockLocn].ToString(), 
                                                                        row[CN.AcctNo].ToString());
                var discountValue = discounts.ContainsKey(currentItemKey) ? discounts[currentItemKey].value : 0M;
                
                if (row[CN.TaxExempt].ToString() != "1")
                {
                    var taxRate = Convert.ToDecimal(row[CN.TaxRate].ToString()) / Convert.ToDecimal(100);
                    if (taxRate > 0)
                    {

                        if (row[CN.TaxType].ToString() == "I")
                        {
                            taxAmount = ((price * quantity) - ((price * quantity) / (1 + taxRate))).RoundTo(this.DecimalPlacesNo);
                            price = (price / (1 + taxRate)).RoundTo(this.DecimalPlacesNo);
                            discountValue = (discountValue / (1 + taxRate)).RoundTo(this.DecimalPlacesNo);
                            cashPrice = (cashPrice.Value / (1 + taxRate)).RoundTo(this.DecimalPlacesNo);
                        }
                        else if (row[CN.TaxType].ToString() == "E")
                        {
                            taxAmount = ((price * quantity) * (taxRate)).RoundTo(this.DecimalPlacesNo);
                        }
                    }
                }
                var order = new Message.Cints.CintOrder()
                {
                    Type = transactionType,
                    PrimaryReference = row[CN.AcctNo].ToString(),
                    SecondaryReference = secondaryReference,
                    ReferenceType = referenceType,
                    SaleType = saleType,
                    SaleLocation = row[CN.AcctNo].ToString().Substring(0, 3),
                    Sku = row[CN.ItemNo].ToString(),
                    ProductId = Convert.ToInt32(row[CN.ItemId].ToString()),
                    StockLocation = stockLocn,
                    ParentSku = row[CN.ParentItemNo].ToString(),
                    ParentId = parentId,
                    TransactionDate = Convert.ToDateTime(row[CN.DateDel].ToString()),
                    Quantity = quantity,
                    Price = price,
                    Discount = discountValue,
                    CashPrice = cashPrice,
                    Tax = taxAmount
                };
                if (!string.IsNullOrEmpty(row[CN.PromotionId].ToString()))
                {
                    order.PromotionId = Convert.ToInt32(row[CN.PromotionId].ToString());
                    order.PromotionIdSpecified = true;
                }
                orders.Add(order);

                if (transactionType == "Delivery" || transactionType == "Return" || transactionType == "Redelivery" || transactionType == "Repossession" || transactionType == "OrderAndDelivery")
                {
                    delTotal = (delTotal + (quantity * price)).RoundTo(this.DecimalPlacesNo);
                }
                total = (total + (quantity * price)).RoundTo(this.DecimalPlacesNo);
            }

            cint.CintOrder = orders.ToArray();
            cint.OrdersDeliveriesTotal = total;
            cint.DeliveriesTotal = delTotal;
            var chub = new Chub();
            chub.Submit(cint, conn, tran);
        }





        /// <summary>
        /// write the fact 2000 files bmsfcint.dat bmsffint.dat and FACTAUTO.TXT
        /// call the fact 2000 batch program to call the FACT2000 routine
        /// and also to update the summary control totals.
        /// </summary>

        public string Process(string countryCode, string configuration)
        {
            var Return = "";
            tabAddColumns(); //add some columns to data table
            this.RenameFiles("bmsfcint");

            //try
            //{
            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();
                // ?? trans.IsolationLevel="read uncommitted";
                DFactExport factExport = new DFactExport();
                factExport.User = this.User;
                factExport.BalanceServiceAccounts(conn, trans);
                //factExport.DeliverNonStocks(conn, trans);                 //IP - 18/10/11 - #8453 - LW74120 - No longer required as Non stocks should get delivered with parent
                factExport.LoadOrdersandDeliveries(conn, trans);
                this.Factrunno = factExport.RunNumber;
                this.DelTotal = factExport.DeliveryTotal;
                this.OrdersandDeliveriesTotal = factExport.OrderandDeliveryTotal;
                BCountry c = new BCountry();
                c.GetMaintenanceParameters(conn, trans, countryCode);
                this.WriteFileHeader();

                this.Cache = c.Cache;
                trans.Commit();
                //Count.DataSet=Ct;
                //string test =(string)Count[CountryParameterNames.TaxType];

                //cintXML.Add(new XElement("Body"));

                int rowCount = factExport.DeliveryLineItems.Rows.Count;
                int counter = 0;
                foreach (DataRow row in factExport.DeliveryLineItems.Rows)
                {
                    counter++;
                    this.StoreValues(row);
                    this.FactTrantype = row[CN.FACTTranType].ToString();
                    string Buffno = row[CN.BuffNo].ToString();

                    if (!((string)Country[CountryParameterNames.TaxType] == "I"
                        && (string)Country[CountryParameterNames.AgreementTaxType] == "E"
                        && Convert.ToDecimal(row[CN.TaxRate]) != 0 && row[CN.TaxExempt].ToString() != "1"
                        && row[CN.ItemNo].ToString() == "STAX"
                        && (string)Country[CountryParameterNames.CountryCode] != "H")) // ignore STAX row unless Thailand 
                    {
                        switch (row[CN.FACTTranType].ToString())
                        {
                            case "01":
                                this.PostOrder(row);
                                break;
                            case "04":	//cancellation
                                //{
                                //    if (this.TotalTaxValue != 0)
                                //    {
                                //        this.WriteTaxRow(row);
                                //        //this.WriteAsLine(row, false);
                                //        this.WriteAsLine(row, true);
                                //    }

                                //    this.WriteAsHeader(row);
                                //    this.BatchCount++; //increment batch count
                                //}
                                this.PostOrder(row);        // #16381   
                                break;
                            default:
                                this.PostDelivery(row);
                                break;
                        }

                    }

                    if (counter == rowCount) //this is the last row
                    {
                        if (this.TotalTaxValue != 0)
                        {
                            this.WriteTaxRow(row);
                            this.WriteAsLine(row, true);
                        }
                        this.BatchCount++; //increment by one for those non c++ people who dont know what this does!
                        this.BatchValue = this.BatchValue + this.NonStock.TransValue;

                    }
                    // storing previous valuesas will have to write tax line if buffno or account number change
                    this.PreviousAgreementNo = row[CN.AgreementNo].ToString();
                    if (row[CN.BuffNo].ToString().Length > 7)
                        this.PreviousBuffno = row[CN.BuffNo].ToString();
                    else
                        this.PreviousBuffno = row[CN.BuffBranchNo].ToString() + row[CN.BuffNo].ToString();
                    this.PreviousAcctNo = this.AcctNo;
                    this.PreviousHeaderWarehouseNo = this.HeaderWarehouseNo;
                    this.PreviousSalesPerson = (row[CN.SalesPerson]).ToString();
                    this.PreviousSourceOfAttraction = this.SourceOfAttraction;
                    this.PreviousStocklocn = Convert.ToInt16(row[CN.StockLocn]);
                    this.PreviousTCCode = this.TCCode;
                }//			foreach(DataRow row in factExport.DeliveryLineItems.Rows)	

                this.WriteAsTrailer();
                this.CreateCSVfile("bmsfcint");
                this.WriteCintToHub(factExport, conn, trans);
                this.tab.Rows.Clear();


                //now update runno stamp and remove from facttrans

                //trans.Commit(); //all done so commit#
                //int runNo = eodRun.StartNextRun(this._configuration, optionCode)
                //        DInterfaceControl ic = new DInterfaceControl();
                //    this.SummaryRunno = ic.StartNextRun(configuration, "UPDSMRY");
                //    this.BatchCount = 0; //reuse for financial export
                //update summary totals 
                //     factExport.EodUpdateSummaryTotals(conn, trans, this.SummaryRunno);
                // export summary data
                //   factExport.RunNumber = this.SummaryRunno;
                //    factExport.LoadforInterfaceFinancial(conn, trans);
                //    this.WriteFinFileHeader();
                //           foreach (DataRow row in factExport.FinancialExportTotals.Rows)
                //        {
                //           this.WriteFinFileRow(row);
                //           this.BatchCount++;
                //       }
                //      this.WriteFinFileTrailer();
                //if (string)Country[CountryParameterNames.e] == "I" 

                //    if ((bool)Country[CountryParameterNames.doFinancialInterface])
                //    {
                //        this.RenameFiles("bmsffint");
                //        this.CreateCSVfile("bmsffint");
                //     }


                //calls the batch file which will call the FACT2000 process....
                //     callfact2000(configuration);

                //      factExport.StampFintransRunno(conn, trans, this.SummaryRunno); //update fintrans run number 
                //     factExport.RunNumber = this.Factrunno;
                //update delivery run number and remove facttrans records
                //     factExport.DeliveriesandOrdersRemoveafterExport(conn, trans);


                //now update runno stamp and remove from facttrans

                //trans.Commit(); //all done so commit#
                //int runNo = eodRun.StartNextRun(this._configuration, optionCode)

                if ((bool)Country[CountryParameterNames.FinTotalsIncludedinDeliveriesExport])
                {  //create a new batch record.
                    try
                    {
                        bool rerun = false;
                        var filedate = "";
                        DInterfaceControl ic2 = new DInterfaceControl();
                        this.SummaryRunno = ic2.StartNextRun(configuration, "UPDSMRY", out rerun, out filedate);        //RI jec 13/04/11
                        this.DoFinancialTotals(conn, trans, configuration);
                        ic2.SetRunComplete(configuration, "UPDSMRY", this.SummaryRunno, "P");// summary update run finished
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

                //calls the batch file which will call the FACT2000 process....
                // callfact2000(configuration);

                factExport.RunNumber = this.Factrunno;
                //update delivery run number and remove facttrans records
                factExport.DeliveriesandOrdersRemoveafterExport(conn, trans);

                // Create export tables for Oracle - but only for Mauritius
                if ((string)Country[CountryParameterNames.CountryCode] == "M")
                {
                    new DOracleIntegration().CreateOracleData(conn, trans, this.Factrunno, this.SummaryRunno);
                }
            } // Dispose SqlConnection

            Return = "P"; //pass
            return Return;
            //}
            //catch (Exception ex)
            //{
            //    Return = "F";
            //    throw ex;

            //}
            /*finally
            {
                return Return;
            }*/

        }
        //Add Some columns to data table.
        private void tabAddColumns()
        {
            tab.Columns.Add("Col01", typeof(string));
            tab.Columns.Add("Col02", typeof(string));
            tab.Columns.Add("Col03", typeof(string));
            tab.Columns.Add("Col04", typeof(string));
            tab.Columns.Add("Col05", typeof(string));
            tab.Columns.Add("Col06", typeof(string));
            tab.Columns.Add("Col07", typeof(string));
            tab.Columns.Add("Col08", typeof(string));
            tab.Columns.Add("Col09", typeof(string));
            tab.Columns.Add("Col10", typeof(string));
            tab.Columns.Add("Col11", typeof(string));
            tab.Columns.Add("Col12", typeof(string));
            tab.Columns.Add("Col13", typeof(string));
            tab.Columns.Add("Col14", typeof(string));
            tab.Columns.Add("Col15", typeof(string));
            tab.Columns.Add("Col16", typeof(string));
            tab.Columns.Add("Col17", typeof(string));
            tab.Columns.Add("Col18", typeof(string));
            tab.Columns.Add("Col19", typeof(string));
            tab.Columns.Add("Col20", typeof(string));
            tab.Columns.Add("Col21", typeof(string));
        }

        public string DoFinancialTotals(SqlConnection conn, SqlTransaction trans, string configuration)
        {

            this.BatchCount = 0; //reuse for financial export
            //update summary totals 
            DFactExport factExport = new DFactExport();
            factExport.EodUpdateSummaryTotals(conn, trans, this.SummaryRunno);

            // export summary data
            factExport.RunNumber = this.SummaryRunno;
            factExport.LoadforInterfaceFinancial(conn, trans);
            this.WriteFinFileHeader();
            foreach (DataRow row in factExport.FinancialExportTotals.Rows)
            {
                this.WriteFinFileRow(row);
                this.BatchCount++;
            }
            this.WriteFinFileTrailer();


            if ((bool)Country[CountryParameterNames.doFinancialInterface])
            {
                this.RenameFiles("bmsffint");
                this.CreateCSVfile("bmsffint");
            }

            factExport.StampFintransRunno(conn, trans, this.SummaryRunno); //update fintrans run number 

            return "P";//pass


        }

        /// <summary>
        /// Copy the factauto.txt and the bmsffint and bmsfcint.dat files
        /// to the FACT 2000 program and files drive and directory
        /// </summary>
        private void callfact2000(string configuration)
        {

            if (File.Exists((string)Country[CountryParameterNames.FACT2000ProgramDirectory] + "\\factauto.bat"))
            {
                try
                {

                    string oldfilename = "";
                    string newfilename = "";

                    DFactExport factExport = new DFactExport();
                    factExport.GetControlFileDetails(configuration);// Getting details of control file so we can write to FACT 2000
                    this.WriteControlfile(factExport.ControlFile); //FACTAUTO.BAT will be running copy this file over will start the process.


                    oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsfcint.dat";
                    newfilename = "d:\\users\\default\\bmsfcint.dat";
                    if (File.Exists(oldfilename))
                        File.Delete(oldfilename);
                    File.Copy(newfilename, oldfilename);

                    oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsffint.dat";
                    newfilename = "d:\\users\\default\\bmsffint.dat";
                    if (File.Exists(oldfilename))
                        File.Delete(oldfilename);
                    File.Copy(newfilename, oldfilename);

                    //copy factauto.txt to both file and program directory
                    // as can't remember which one FACT 2000 wants this should kick off the FACT 2000 process
                    oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.TXT";
                    newfilename = "d:\\users\\default\\FACTAUTO.TXT";
                    //         if (File.Exists(oldfilename))
                    //         File.Delete(oldfilename);
                    File.Copy(newfilename, oldfilename);

                    //                    oldfilename = (string)Country[CountryParameterNames.FACT2000ProgramDirectory] + "\\FACTAUTO.TXT";
                    //                  newfilename = "d:\\users\\default\\FACTAUTO.TXT";
                    //                   if (File.Exists(oldfilename))
                    //                   File.Delete(oldfilename);
                    //                 File.Copy(newfilename, oldfilename);


                    FileInfo OldDel = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsfcint.dat");

                    FileInfo errInfo = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.ERR");
                    /*if (errInfo.Exists)
                    {
                       errInfo.Delete();
                    }*/
                    FileInfo okInfo = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\factauto.ok");
                    /* if (okInfo.Exists)
                     {
                        okInfo.Delete();
                     }*/
                    // removing this as this is used by the product file import for checking that procedure is complete
                    /*FileInfo AssoProdIndic = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "FACTAUTO.ODF");
                    if (AssoProdIndic.Exists)
                    {
                        AssoProdIndic.Delete();
                    }*/
                    // Not doing this now System.Diagnostics.Process.Start("h:/factauto.bat");
                    //FileInfo AssoProdIndic = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "BMSFAPRD.DAT");

                    //factExport


                    while (!errInfo.Exists) //wait for FACT 2000 to finish
                    {
                        FileInfo err2Info = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.ERR");

                        if (errInfo.Exists || err2Info.Exists)
                        {
                            break;
                        }
                        if (okInfo.Exists) //finish process successfully.
                        {
                            break;
                        }

                        FileInfo ok2Info = new FileInfo((string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.OK");
                        if (ok2Info.Exists) //finish process successfully.
                        {
                            break;
                        }
                    }//while ...

                }
                catch (Exception)
                {
                    BInterfaceError ie = new BInterfaceError(
                                null,
                                null,
                                "COS FACT",
                                Factrunno,
                                DateTime.Now,
                                "Error copying FACT 2000 files- manually copy the files across",
                                "W");
                }
            }
        }


        private void WriteControlfile(string line)
        {
            StreamWriter sw = new StreamWriter("d:/users/default/FACTAUTO.TXT", false);
            sw.Write(line);
            sw.Close();
            //string destfilename = "f:\\FACTAUTO.txt";
            //File.Copy("d:/users/defaul/FACTAUTO.TXT", destfilename);

        }




        public void PostOrder(DataRow row)
        {
            if (row[CN.AcctNo].ToString() != this.PreviousAcctNo  //the account number has changed so write the line
                || row[CN.TCCode].ToString() != this.PreviousTCCode
                )
            {
                if (this.TotalTaxValue != 0)
                {
                    this.WriteTaxRow(row); //=row); 
                    this.WriteAsLine(row /*  =row*/, true); //write nonstock line

                    this.BatchCount++;
                    this.BatchValue = this.BatchValue + Convert.ToDecimal(row[CN.Value]);

                };


                this.TotalTaxValue = 0;
                this.TotalFACTTaxValue = 0;

                /*
                ** Change of Account Number or TC Type
                ** (61 to 58), so write new header to
                ** feeder, though the source data itself
                ** doesn"t have headers, only lines.
                */
                this.WriteAsHeader(row);

                this.BatchCount++;
                this.PreviousAcctNo = row[CN.AccountNumber].ToString();
                this.PreviousTCCode = row[CN.TCCode].ToString();
            };
            /* row[AcctNo != this.PreviousAcctNo */
            /*
            ** Write the line.
            */

            this.CalculateTax(ref row, "v", false);
            /*
            ** now do the price
            */

            this.CalculateTax(ref row, "p", false);

            ReCheckValueequalsPriceQuantity(ref row);

            this.WriteAsLine(row, false);

            this.BatchCount++;
            this.BatchValue = this.BatchValue + Convert.ToDecimal(row[CN.Value]);



        }
        public string RemoveCents(decimal inputvalue)
        {
            string outputString = "";
            outputString = inputvalue.ToString();
            int whereDecimalPoint = outputString.IndexOf(".", 0);
            int outputlength = outputString.Length;
            int toremove = outputlength - whereDecimalPoint;
            if (whereDecimalPoint > 0)
                outputString = outputString.Remove(whereDecimalPoint, toremove) + outputString.Substring(whereDecimalPoint + 1, toremove); // remove .
            return outputString;
        }
        public string stringtimes100(decimal inputvalue)
        {
            string outputString = "";

            //UAT 404 If value is to more than 2 decimal places then needs to be rounded to 2
            outputString = (Math.Round(inputvalue, 2) * 100).ToString();
            int whereDecimalPoint = outputString.IndexOf(".", 0);
            int outputlength = outputString.Length;
            int toremove = outputlength - whereDecimalPoint;
            if (whereDecimalPoint > 0)
                outputString = outputString.Remove(whereDecimalPoint, toremove);

            return outputString;
        }

        /// <summary>
        /// Puts the order or delivery line into a data row in memory 
        /// in the format: buffno,itemno,warehouse,quantity,price,value, warranty/information
        /// </summary>

        public void WriteAsLine(DataRow row, bool doNonstock)
        {
            //select  :hasaffinitytext = value from countrymaintenance
            //where Name="Country has affinity";the
            DataRow Trow = null;
            Trow = this.tab.NewRow();
            Trow[0] = "9"; //dont forget to change this if any more fields required
            if (this.FactTrantype == "01" || this.FactTrantype == "04")
            {
                if (doNonstock == false)
                {
                    if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")               // #12948
                    {
                        Trow[1] = this.AcctNo.Remove(10, 2);
                    }
                    else
                    {
                        Trow[1] = row[CN.ServiceRequestNo].ToString();           // #12948
                    }
                }
                else
                {
                    Trow[1] = this.PreviousAcctNo.Remove(10, 2);
                }
            }
            else
                if (this.Buffno == "0")
            {
                if (doNonstock == false)
                {
                    if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")               // #12948
                    {
                        Trow[1] = this.AcctNo.Remove(10, 2);
                    }
                    else
                    {
                        Trow[1] = row[CN.ServiceRequestNo].ToString();           // #12948
                    }
                }
                else
                {
                    Trow[1] = this.PreviousAcctNo.Remove(10, 2);
                }

            }
            else
            {
                if (doNonstock == false)
                {
                    if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")               // #12948
                    {
                        Trow[1] = this.Buffno;
                    }
                    else
                    {
                        Trow[1] = row[CN.ServiceRequestNo].ToString();           // #12948
                    }
                }
                else
                {
                    Trow[1] = this.PreviousBuffno;
                }
            }

            if (doNonstock == false)
                Trow[2] = row[CN.ItemNo].ToString();
            else
                Trow[2] = "STAX";
            string itemno = row[CN.ItemNo].ToString();
            if (itemno == "dt")
                itemno = "df";

            if (row[CN.RetStockLocn].ToString() != "0" && row[CN.RetStockLocn].ToString() != "" && doNonstock == false)
                Trow[3] = row[CN.ReturnWarehouse].ToString();
            else
                Trow[3] = this.LineWarehouseno;

            decimal ordPrice = 0;
            //string ordValue="";
            //int whereDecimalPoint =0;
            if (doNonstock == false)
                Trow[4] = (this.Quantity * 100).ToString();
            else // for tax either 1 or -1
            {
                if (this.NonStock.TransValue >= 0)
                    Trow[4] = 100;
                else
                    Trow[4] = -100;

            }

            decimal thisprice = 0;
            if (doNonstock)
                thisprice = Math.Abs(this.NonStock.TransValue); //68925 investigating prevent negative prices going to FACT.
            else
                thisprice = this.Price;

            if (thisprice == 0)
            {
                if (this.Quantity > 0)
                {
                    if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                        Country[CountryParameterNames.CountryCode].ToString() == "C")
                    {
                        ordPrice = (this.Value / Convert.ToDecimal(this.Quantity));
                        Trow[5] = RemoveCents(ordPrice);
                    }
                    else
                    {
                        ordPrice = (this.Value * 100 / Convert.ToDecimal(this.Quantity));
                        Trow[5] = stringtimes100(ordPrice);
                    }
                }
                else
                    Trow[5] = "0";
            }
            else
            /* FR108198 Changed to call public void  removecents RD/AA 24/09/02 */
            /* but only for Madagascar || Indonesia  */
            {
                if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                Country[CountryParameterNames.CountryCode].ToString() == "C")
                    Trow[5] = RemoveCents(thisprice);
                else
                    Trow[5] = stringtimes100(thisprice);

                /*if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                    Country[CountryParameterNames.CountryCode].ToString()=="C")
                {
                    ordPrice =this.Price.ToString();
                    whereDecimalPoint=ordPrice.IndexOf(".",0);
                    if (whereDecimalPoint >0)
                        ordPrice = ordPrice.Remove(whereDecimalPoint,3) + ordValue.Substring(whereDecimalPoint+1,2); // remove .
                    Trow[5] = ordPrice;
                }
                else
                {
                    ordPrice =(this.Price * 100).ToString(); // values multiplied by 100 duh!
                    whereDecimalPoint=ordValue.IndexOf(".",0);
                    if (whereDecimalPoint >0)
                        ordPrice = ordPrice.Remove(whereDecimalPoint,3); // remove .00
                    Trow[5] = ordPrice;
                }*/

            }

            /* FR108198 Changed to call public void  removecents RD/AA 24/09/02 */
            /* Trow[6] = varchar(int4(money(row.Value) * 100)); */
            if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                Country[CountryParameterNames.CountryCode].ToString() == "C")
                if (doNonstock == false)
                    Trow[6] = RemoveCents(this.Value);
                else
                    Trow[6] = RemoveCents(this.NonStock.TransValue);
            else
                if (doNonstock == false)
                Trow[6] = stringtimes100(this.Value);
            else
                Trow[6] = stringtimes100(this.NonStock.TransValue);



            /* FR 1059 */
            string w_item = "";
            string w_cont = "";
            string orig_item = "";
            if (this.Category == 12 || this.Category == 82 && !doNonstock) //warranty categories
            { /*if this warranty is linked to kit product { retrieve details from lineitem warranty table*/
                orig_item = row[CN.ParentItemNo].ToString();
                w_item = row[CN.ItemNo].ToString();
                w_cont = row[CN.ContractNo].ToString();
            }



            Trow[7] = "";
            Trow[8] = "";
            /* check whether affinity item*/
            if (!doNonstock)
            {
                if (row[CN.Affinity].ToString() == "1" && this.Category == 11 || (this.Category >= 51 && this.Category <= 59 && !doNonstock))
                {
                    if (row[CN.RetItemNo].ToString() == "")
                    {  /* its delivery*/
                        Trow[7] = row[CN.AffinityFormatDate].ToString() + "/" + row[CN.ContractNo].ToString();
                    }
                    else   /*its reposession - Pete wants it in certain format */
                    {
                        Trow[7] = row[CN.OriginalItem] + "/" + row[CN.ContractNo].ToString() + "/" + row[CN.AffinityFormatDate].ToString();
                    }

                }

                if (w_item != "")
                {
                    /* if repossession || redelivery after reposession- can be zero value counter.e. warranty has a different code from the original */
                    if (row[CN.OriginalItem].ToString() != row[CN.RetItemNo].ToString() && row[CN.RetItemNo].ToString() != "")
                        Trow[7] = row[CN.OriginalItem].ToString() + "/" + w_cont + "/" + orig_item;
                    else   /*normal delivery/collection with same warranty number*/
                        if (row["originalStockacctno"].ToString() == "")  // normal delivery
                        Trow[7] = row[CN.ParentItemNo].ToString() + "/" + w_cont;
                    else // renewable warranty
                        Trow[7] = orig_item + '/' + w_cont + '/' + row["originalStockacctno"].ToString() + '/' + row["originalcontractno"].ToString();

                    if (row[CN.ReplacementMarker].ToString() != "")
                        Trow[8] = row[CN.ReplacementMarker].ToString();
                }
                else
                    if (row[CN.ReplacementMarker].ToString() != "" && doNonstock == false)
                {
                    Trow[7] = row[CN.ReplacementMarker].ToString();
                };
            }

            if (!doNonstock)
                Trow[9] = (row[CN.AssociatedItemIndicator]).ToString();

            this.tab.Rows.Add(Trow);
        }



        public void WriteAsTrailer()
        {
            DataRow Trow = null;
            Trow = this.tab.NewRow();


            Trow[0] = "16"; //this to store number of commas to print out
            Trow[1] = "9999999999";
            Trow[2] = "99999999";
            Trow[3] = "99";
            Trow[4] = (this.tab.Rows.Count + 1).ToString(); // number of records should be number of rows in array plus this trailer row
            //this.BatchCount+2).ToString()  was using this but unreliable
            if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                Country[CountryParameterNames.CountryCode].ToString() == "C")
            {
                Trow[5] = this.RemoveCents(this.OrdersandDeliveriesTotal);
                Trow[6] = this.RemoveCents(this.DelTotal);
            }
            else
            {
                Trow[5] = this.stringtimes100(this.OrdersandDeliveriesTotal);
                Trow[6] = this.stringtimes100(this.DelTotal);
            }
            Trow[7] = "";
            Trow[8] = "";
            Trow[9] = "";
            Trow[10] = "";
            Trow[11] = "";
            Trow[12] = "";
            Trow[13] = "99";
            // hoping to get ddmmyy
            string thismonth = DateTime.Today.Month.ToString();
            if (thismonth.Length == 1)
                thismonth = "0" + thismonth; //pad it for the trailer
            string today = DateTime.Today.Day.ToString();
            if (today.Length == 1)
                today = "0" + today;
            Trow[14] = today + thismonth + DateTime.Today.Year.ToString().Remove(0, 2);
            Trow[15] = "";
            Trow[16] = "";


            this.tab.Rows.Add(Trow);

            //cintXML.Add(new XElement("Footer", 
            //            new XElement("OrdersDeliveriesTotal", this.OrdersandDeliveriesTotal),
            //           new XElement("DeliveriesTotal", this.DelTotal)));
        }



        /// <summary>
        /// Puts the order or delivery line into a data row in memory 
        /// in the format: buffno,itemno,warehouse,quantity,price,value, warranty/information
        /// </summary>
        public void PostDelivery(DataRow row)
        {
            /*status =integer not null;
            litem               = array of crtlineitem;
            del                 = crtdelivery;
            fint                = crtfintrans;
    
            custo               = cust_o;
            ipupdateflag        = smallint not null with default;
            datedel             = date not null with default "today";
            accttype            = varchar(1) not null with default;
            deliveryflag_value  = varchar(1) not null with default;
            remainder =0;
            lineitem            = crtlineitem;
            j=integer not null;
            k=integer not null; */
            //if (row[CN.AccountNumber]=="720500009100")
            //{
            //    //int i = 1;
            //}
            if (row[CN.BuffNo].ToString() != this.PreviousBuffno
            || row[CN.AccountNumber].ToString() != this.PreviousAcctNo)
            {//  Change of Delivery, so write new header to feeder, though
                // the source data itself doesn"t have headers, only lines.   
                if (this.TotalFACTTaxValue != 0) /* changed from tottax */
                {
                    this.WriteTaxRow(row);

                    this.WriteAsLine(row, true);
                    this.BatchCount++;
                    this.BatchValue = this.BatchValue + Convert.ToDecimal((row[CN.Value]));

                };
                this.TotalTaxValue = 0;
                this.TotalFACTTaxValue = 0;
                short branchNo = 0;
                this.WriteAsHeader(row);
                this.BatchCount++;
                this.PreviousBuffno = row[CN.BuffNo].ToString();
                this.PreviousAcctNo = row[CN.AccountNumber].ToString();
                branchNo = Convert.ToInt16(Country[CountryParameterNames.HOBranchNo]);
                DBranch branch = new DBranch();
                int transRefNo = 0;
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    using (var trans = conn.BeginTransaction())
                    {
                        transRefNo = branch.GetTransRefNo(conn, trans, branchNo);
                    }
                }
                string countryCode = "";
                countryCode = Country[CountryParameterNames.CountryCode].ToString();
                countryCode = countryCode.Trim();
                //decimal transValue = 0;
                BDelivery del = new BDelivery();
                //del.DeliverNonStocks(conn, trans,  this.PreviousAcctNo, 
                //	row[CN.AccountType].ToString(), countryCode, branchNo, transRefNo, ref transValue, addToAgreement.AgreementNumber);

            };

            /*
            ** Calculate the  value line. - might have to take tax out
            */
            this.CalculateTax(ref row, "v", true);


            /*
            ** now do the price
            */
            this.CalculateTax(ref row, "p", false);

            ReCheckValueequalsPriceQuantity(ref row);

            this.WriteAsLine(row, false); /*,CheckWarranty = 1* To do?/); /* FR 1082 */

            this.BatchCount++;
            this.BatchValue = this.BatchValue + Convert.ToDecimal(row[CN.Value]);

            /*   As line was written ok above, do we need to stamp delivery line with runno */



        }

        private void ReCheckValueequalsPriceQuantity(ref DataRow row)
        {
            var rowprice = Convert.ToDecimal(row[CN.Price]);
            var rowvalue = Convert.ToDecimal(row[CN.Value]);
            var rowquantity = Convert.ToDecimal(row[CN.Quantity]);

            if (rowvalue != rowquantity * rowprice)
            {
                this.TotalTaxValue += (rowvalue - (rowquantity * rowprice));
                rowvalue = rowquantity * rowprice;
                row[CN.Value] = Convert.ToString(Math.Round(rowvalue, 2));
                this.Value = Math.Round(rowvalue, 2);
            }

        }

        public void WriteFinFileHeader()
        {
            DataRow Trow = null;
            if (this.tab.Columns.Count == 0)
            {
                tabAddColumns();
            }

            Trow = this.tab.NewRow();
            Trow[0] = "4"; //stores number of fields dont forget to change this if any more fields required
            Trow[1] = "0000000000";
            Trow[2] = this.SummaryRunno.ToString();
            Trow[3] = "00";
            int stringlength = 0;
            stringlength = (this.SummaryRunno).ToString().Length;
            Trow[4] = "00000";
            this.tab.Rows.Add(Trow);

        }

        public void WriteFinFileTrailer()
        {
            DataRow Trow = null;
            Trow = this.tab.NewRow();
            Trow[0] = "4"; //dont forget to change this if any more fields required
            Trow[1] = "9999999999";
            Trow[2] = (this.BatchCount + 2).ToString();


            if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                Country[CountryParameterNames.CountryCode].ToString() == "C")
            {
                Trow[3] = this.RemoveCents(this.TotalFinancialValue);
            }
            else
            {
                Trow[3] = this.stringtimes100(this.TotalFinancialValue);
            }

            string thismonth = DateTime.Today.Month.ToString();
            if (thismonth.Length == 1)
                thismonth = "0" + thismonth; //pad it for the trailer
            string today = DateTime.Today.Day.ToString();
            if (today.Length == 1)
                today = "0" + today;
            Trow[4] = today + thismonth + DateTime.Today.Year.ToString().Remove(0, 2);

            this.tab.Rows.Add(Trow);

        }
        public void WriteFinFileRow(DataRow row)
        {
            DataRow Trow = null;
            Trow = this.tab.NewRow();
            Trow[0] = "3"; //stores number of fields dont forget to change this if any more fields required
            Trow[1] = row[CN.InterfaceAccount].ToString();
            if (Country[CountryParameterNames.CountryCode].ToString() == "I" ||
                        Country[CountryParameterNames.CountryCode].ToString() == "C")
                Trow[2] = this.RemoveCents(Convert.ToDecimal(row[CN.TransValue]));
            else
                Trow[2] = this.stringtimes100(Convert.ToDecimal(row[CN.TransValue]));

            Trow[3] = row[CN.Reference].ToString();
            this.tab.Rows.Add(Trow);
            this.TotalFinancialValue += Convert.ToDecimal(row[CN.TransValue]);
        }
        /// <summary>
        /// Writes 2 lines of header record- customer and account information into the in memory datarow
        /// </summary>
        public void WriteAsHeader(DataRow row)
        {
            DataRow Trow = null;
            Trow = this.tab.NewRow();
            string BuffnoAcctno = "";
            /*Trow[0] = "Name" + i;
            Trow[1] = i.ToString();
            Trow[2] = (i*2.5).ToString();
            this.LineFields.Clear();*/
            Trow[0] = "20"; //this is the number of columns to write was incorrect 71011
            if (this.FactTrantype == "01" || this.FactTrantype == "04")
            {
                if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")
                {
                    BuffnoAcctno = this.AcctNo.Remove(10, 2); //left 10 digits wanted - dont ask why
                }
                else
                {
                    BuffnoAcctno = row[CN.ServiceRequestNo].ToString();           // #12948
                }
            }
            else
                if (this.Buffno == "0")
            {
                if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")           // #12948
                {
                    BuffnoAcctno = this.AcctNo.Remove(10, 2);
                }
                else
                {
                    BuffnoAcctno = row[CN.ServiceRequestNo].ToString();             // #12948
                }
            }
            else
            {
                if (row[CN.ServiceRequestNo] == null || Convert.ToString(row[CN.ServiceRequestNo]) == "")           // #12948
                {
                    BuffnoAcctno = this.Buffno;
                }
                else
                {
                    BuffnoAcctno = row[CN.ServiceRequestNo].ToString();             // #12948
                }
            };

            Trow[1] = BuffnoAcctno;
            Trow[2] = "00000000";
            Trow[3] = this.FactTrantype;
            Trow[4] = this.AcctNo;
            Trow[5] = row[CN.CustomerName].ToString();
            Trow[6] = row[CN.HeaderWarehouseNo].ToString();

            /* CR324 Output the payment public void if appropriate */
            if (Country[CountryParameterNames.PaymentMethod].ToString() == "Y")
            {
                Trow[7] = row[CN.PaymentMethod].ToString();
            }
            else
            {
                Trow[7] = "";
            }

            Trow[8] = "";
            Trow[9] = "";
            Trow[10] = "";
            Trow[11] = row[CN.SOA];
            /* CR710 DSR 15 Oct 2004 - FACT may use a FACT Empee No which is alphanumeric */
            Trow[12] = row[CN.SalesPerson].ToString();
            Trow[13] = this.TCCode;
            Trow[14] = row[CN.AgreementDate].ToString(); //format is already set to fact format for date -

            Trow[15] = row[CN.DateDel].ToString();
            Trow[16] = row[CN.Code].ToString();

            /* CR285 Output the loyalty card if appropriate */
            if (Country[CountryParameterNames.LoyaltyCard].ToString() == "Y")
                Trow[17] = row[CN.MoreRewardsNo].ToString();
            else
                Trow[17] = " ";

            if (this.AcctNo.Substring(3, 1) == "0")
                /*append termstype for hp accounts*/
                Trow[18] = row[CN.TermsType].ToString();
            else
                Trow[18] = "";

            // Duty Free indicator (not implemented!)
            Trow[19] = "";

            // Service Request Charge Type indicator
            // if (this.AcctNo == "998400024130")
            //   this.AcctNo = AcctNo; for testing... 

            Trow[20] = row[CN.ChargeType].ToString();

            this.tab.Rows.Add(Trow);
            //Trow.ItemArray..Delete();
            Trow = this.tab.NewRow();

            // now we are going to append another line with address details

            this.BatchCount++; /* add a line so add to batchcount*/
            Trow[0] = "10";	//number of columns to print out should be changed if more required
            Trow[1] = BuffnoAcctno;
            Trow[2] = "00000001";
            Trow[3] = this.FactTrantype;
            Trow[4] = this.AcctNo;
            if (Country[CountryParameterNames.CountryCode].ToString() != "H") //Not Thailand....
            {
                Trow[5] = row[CN.CustomerName];
                Trow[6] = row[CN.cusaddr1];
                Trow[7] = row[CN.cusaddr2];
                Trow[8] = row[CN.cusaddr3];
                Trow[9] = row[CN.cuspocode];
                Trow[10] = row[CN.TelNo]; // row[CN.PhoneNo];
            }
            else
            {

                Trow[5] = "";
                Trow[6] = "";
                Trow[7] = "";
                Trow[8] = "";
                Trow[9] = "";
                Trow[10] = "";
            };

            this.tab.Rows.Add(Trow);
        }
        /// <summary>
        /// Depending on whether v for value or p for price is passed into v_calctype system will calculate tax value of
        /// line to post to bmsfcint file and also adds up the tax amount for later adding to the file.
        // this all depends on taxtype and agreementtaxtype
        /// </summary>
        public void CalculateTax(ref DataRow row, string v_calctype, bool DeliveryLine)
        {
            decimal v_value = 0;

            /*SourceData     = BMSFCINTSetO;
            counter    =0;
            decimal v_value        = float not null;
theof			decimal ThisItemTaxValue       = float not null;
            string v_calctype     = varchar(1) not null;
            delline =0;
            decimal tottax         = float not null;
            decimal tottaxfact     = float not null;
            decimal deltotal       = money not null;
    
            int rowcount       = integer not null)=*/


            if ((string)Country[CountryParameterNames.TaxType] == "I"
                && (string)Country[CountryParameterNames.AgreementTaxType] == "E"
                && Convert.ToInt32(row[CN.TaxRate]) != 0
                && row[CN.TaxExempt].ToString() != "1"
                && (string)Country[CountryParameterNames.CountryCode] != "H")
            {
                if (v_calctype == "v") //v == order value
                {
                    v_value = (Convert.ToDecimal(row[CN.Value])) * (100 + (Convert.ToDecimal(row[CN.TaxRate]))) / 100; // Fixed
                    v_value = Math.Round(v_value, 2);
                    this.TaxValue = Convert.ToDecimal(row[CN.Value]) - v_value;
                    if (Convert.ToDecimal(row[CN.Value]) != 0)
                    {
                        if (DeliveryLine == true)
                        {
                            if (row[CN.CustomerID].ToString() == "PAID & TAKEN")
                            {
                                /* A PAID & TAKEN account - calculate tax separately  - FR732 */
                                // TO DO row[CN.TaxAmt] = this.ItemTaxValue;
                                /* Overwrite the TOTAL tax value - not what we need here */
                            };
                            /* A delivery, so need to total up the actual tax that was applied */
                            this.TotalTaxValue = this.TotalTaxValue + this.TaxValue; // Convert.ToDecimal(row[CN.TaxAmt]);
                            this.TotalFACTTaxValue = this.TotalFACTTaxValue + this.TaxValue; // Convert.ToDecimal(row[CN.TaxAmt]);
                        }
                        else //reducing totals as tax isn't posted as part of an order
                            this.OrdersandDeliveriesTotal = this.OrdersandDeliveriesTotal - this.TaxValue;

                        row[CN.Value] = v_value.ToString();
                        this.Value = v_value;
                    };
                }
                if (v_calctype == "p")
                {
                    v_value = Convert.ToDecimal(row[CN.Price]) * (100 + Convert.ToDecimal(row[CN.TaxRate])) / 100;
                    v_value = Math.Round(v_value, 2);
                    if (Convert.ToDecimal(row[CN.Price]) != 0)
                    {
                        // Can we do this ?? Convert.ToDecimal(row[CN.Price]) = v_value;
                        row[CN.Price] = v_value.ToString();
                        this.Price = v_value;
                    };
                }
            }
            if ((string)Country[CountryParameterNames.TaxType] == "E"
                && (string)Country[CountryParameterNames.AgreementTaxType] == "I"
                && Convert.ToInt32(row[CN.TaxRate]) != 0
                && row[CN.TaxExempt].ToString() != "1")
            {
                /*
                ** Tax is contained within the price && order value, so need to extract it
                */
                if (v_calctype == "v")
                {
                    v_value = ((Convert.ToDecimal(row[CN.Value]) * 100) / (100 + Convert.ToDecimal(row[CN.TaxRate])));
                    v_value = Math.Round(v_value, 2);
                    this.LineTaxValue = Convert.ToDecimal(row[CN.Value]) - v_value;

                    if (Convert.ToDecimal(row[CN.Value]) != 0)
                    {
                        if (DeliveryLine == true)
                        {
                            if (row[CN.CustomerID].ToString() == "PAID & TAKEN")
                            {
                                /* A PAID & TAKEN account - calculate tax separately  - FR732 */
                                /* Overwrite the TOTAL tax value - not what we need here */
                                row[CN.TaxAmt] = this.LineTaxValue;
                            };

                            /* A delivery, so need to total up the actual tax that was applied */
                            this.TotalTaxValue = this.TotalTaxValue + this.LineTaxValue;  /* FR 1140 */
                            this.TotalFACTTaxValue = this.TotalFACTTaxValue + this.LineTaxValue;
                        }
                        else //reducing totals as tax isn't posted as part of an order
                            this.OrdersandDeliveriesTotal = this.OrdersandDeliveriesTotal - this.LineTaxValue;

                        //to do row[counter] = v_value;
                        row[CN.Value] = v_value.ToString();
                        this.Value = v_value;
                    }

                }
                else if (v_calctype == "p") //reducing the price of the item by the tax value 
                {
                    v_value = (((Convert.ToDecimal(row[CN.Price]) * 100) / (100 + Convert.ToDecimal(row[CN.TaxRate]))));
                    v_value = Math.Round(v_value, 2);
                    if (Convert.ToDecimal(row[CN.Price]) != 0)
                    {
                        row[CN.Price] = v_value.ToString();
                    };
                    row[CN.Price] = v_value.ToString();
                    this.Price = v_value;
                }
            }
            else if ((string)Country[CountryParameterNames.TaxType] == "E"
                        && (string)Country[CountryParameterNames.AgreementTaxType] == "E"
                        && Convert.ToDecimal(row[CN.TaxRate]) != 0
                        && row[CN.TaxExempt].ToString() != "1")
            {// Tax is held separately from the price  Need to post sales tax for Reposessions & Redeliveries
                if (row[CN.TCCode].ToString() == "17"
                    || ((row[CN.TCCode].ToString() == "32" && row[CN.DelOrColl].ToString() == "R")))
                {
                    if (v_calctype == "v")
                    {
                        v_value = Convert.ToDecimal(row[CN.Value]);
                        v_value = Math.Round(v_value, 2);
                        this.TaxValue = (Convert.ToDecimal(row[CN.Value]) * (Convert.ToDecimal(row[CN.TaxRate]) / 100));
                        this.TotalTaxValue = this.TotalTaxValue + this.LineTaxValue;
                        this.TotalFACTTaxValue = this.TotalFACTTaxValue + this.LineTaxValue;
                        this.DelTotal = this.DelTotal + this.LineTaxValue;/* FR 1145 */

                        /* Add fintrans && Delivery rows for STAX 
                        CallProc posttax(SourceData = ByRef(SourceData),
                        counter          = counter,
                        tottaxfact = ByRef(tottaxfact),
                        tottax     = ByRef(tottax));*/
                    }
                    else if (v_calctype == "p")
                    {
                        v_value = Convert.ToDecimal(row[CN.Price]);
                        v_value = Math.Round(v_value, 2);
                        if (Convert.ToDecimal(row[CN.Price]) != 0)
                        {
                            this.TaxValue = (Convert.ToDecimal(row[CN.Price]) * Convert.ToDecimal((row[CN.TaxRate])) / 100);
                        };
                    };
                };
            };






        }
        /// <summary>
        /// Writes the header for the bmsfcint.dat file which consists of lots of zeros and a run number
        /// </summary>
        public void WriteFileHeader()
        {
            DataRow Trow = null;
            Trow = this.tab.NewRow();

            Trow[0] = "4";
            Trow[1] = "0000000000";
            Trow[2] = "00000000";
            Trow[3] = "00";
            int stringlength = 0;
            stringlength = (this.Factrunno).ToString().Length;
            Trow[4] = "00000".Remove(5 - stringlength, stringlength) + (this.Factrunno).ToString();

            this.tab.Rows.Add(Trow);

            //cintXML = new XDocument(new XElement("Header",
            //                                 new XElement("RunNo", this.Factrunno)));


        }

        /// <summary>
        /// Puts the tax row in this.NonStock for when the Taxtype = I and agreement tax type =E so need to extract
        /// </summary>
        public void WriteTaxRow(DataRow row)
        //	SourceData    = BMSFCINTSetO;

        //i   =0;
        //TotTaxfact    = float not null default 0;
        //TotTax        = float not null default 0)=
        {

            /* 
            **write the tax line here for previous details
            */

            this.NonStock.AccountNumber = this.PreviousAcctNo;
            if (this.PreviousAgreementNo != "")
                this.NonStock.AgreementNumber = Convert.ToInt32(this.PreviousAgreementNo);
            else
                this.NonStock.AgreementNumber = 0;
            /*this.NonStock.code        = "";*/
            //this.NonStock.BuffNo      = Convert.ToInt32(this.PreviousBuffno);
            this.NonStock.ItemNumber = "STAX";

            //this.Price =Math.Abs(this.TotalTaxValue);
            //if (this.TotalFACTTaxValue >=0)
            //   		this.Quantity    = 1;
            //else
            //          this.Quantity = -1;

            this.NonStock.TransValue = this.TotalTaxValue;

            this.NonStock.StockLocation = this.PreviousStocklocn;
            //this.SalesPerson = this.PreviousSalesPerson;
            //this.HeaderWarehouseNo = this.PreviousHeaderWarehouse;
            //this.LineWarehouseno=this.PreviousHeaderWarehouseNo.ToString();
            this.SourceOfAttraction = this.PreviousSourceOfAttraction;
        }


        public void PostTax(DataRow row)
        /*SourceData   = BMSFCINTSetO;
        counter  =0;
        tottax       = money not null default 0;
        tottaxfact   = money not null default 0;
        ifcontrol    = crtinterfacecontrol;
        rowcount     = integer not null)=
    declare
        status       = integer not null;
        hi_ref       = integer not null;
        datenow      = date not null;
        del          = crtdelivery;
        fint         = crtfintrans;
        delarray     = array of crtdelivery;
        ifaceerror   = crtinterfaceerror;
        temp_posted  = varchar(32)not null with default "";
        taxline      = BMSFCINTDataO;*/
        {

            DBranch branch = new DBranch();
            int refNo = 0;
            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    refNo = branch.GetTransRefNo(conn, trans, Convert.ToInt16(row[CN.BranchNo]));
                }
            }
            DFinTrans fint = new DFinTrans();

            fint.BranchNumber = Convert.ToInt16(row[CN.StockLocn]);
            fint.AccountNumber = row[CN.AccountNumber].ToString();
            fint.FTNotes = "FACN"; /* AA change */
            fint.DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
            fint.TransTypeCode = "DEL";
            fint.EmployeeNumber = 99999;
            fint.TransUpdated = "N";
            fint.TransPrinted = "N";
            fint.TransValue = this.TaxValue;
            fint.ChequeNumber = "";
            fint.BankAccountNumber = "";
            fint.TransRefNo = refNo;
            fint.BankCode = "";


            this.TotalTaxValue = 0; /* FR 1156 - Initialise STAX for multiple REPOS/ REDELS */

        }

        public void CreateCSVfile(string filename)
        {
            StreamWriter sw = new StreamWriter((string)Country[CountryParameterNames.SystemDrive] + "\\" + filename + ".dat", false);
            int iColCount = 0;
            foreach (DataRow row in this.tab.Rows)
            {   //set the number of fields to write for header 1, header 2 and lineitems
                iColCount = Convert.ToInt32(row[0]);

                for (int i = 1; i <= iColCount; i++)
                {
                    if (!Convert.IsDBNull(row[i]))
                    {
                        sw.Write(row[i].ToString());
                    }
                    if (i < iColCount)
                    {
                        sw.Write(",");
                    }
                    if (i == iColCount)
                    {
                        sw.WriteLine("");
                        break;
                    }
                }

            }

            sw.Close();

        }


    }

}

