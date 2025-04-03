using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using System.Collections;
using STL.Common.ServiceRequest;
 

namespace STL.DAL
{
    public partial class DServiceRequest : DALObject
    {
        // SR_ServiceRequest
        private DateTime _dateLogged;
        private DateTime _dateReopened;
        private DateTime _purchaseDate;
        private DateTime _receivedDate;
        private decimal _depositAmount;
        private decimal _repairEstimate;
        private short _stockLocn;
        private string _deliveryDamage;
        private string _description;
        private string _retailer;       //CR1030 jec
        private string _custID;       //CR1030 jec
        private string _extWarranty;
        private string _goodsOnLoan;
        private string _modelNo;
        private string _productCode;
        private decimal _unitPrice;
        private string _serialNo;
        private string _serviceEvaln;
        private string _serviceLocn;
        private string _status;
        private string _depositPaid;
        private string _transitNotes;
        private string _comments;
        private DateTime _dateCollected;
        // SR_Allocation
        private DateTime _dateAllocated;
        private string _zone;
        private int _technicianId;
        private DateTime _partsDate;
        private DateTime _repairDate;
        private string _isAM;
        private string _instructions;
        private string _reAssignCode;       //CR1030
        private string _reAssignedBy;       //CR1030
        // SR_Resolution
        private DateTime _dateClosed;
        private string _resolution;
        private int _resolutionChangedBy;
        private string _chargeTo;
        private int _chargeToChangedBy;
        private string _chargeToMake;
        private string _chargeToModel;
        private decimal _hourlyRate;
        private decimal _hours;
        private decimal _labourCost;
        private decimal _additionalCost;
        private decimal _transportCost;
        private decimal _totalCost;
        private string _goodsOnLoanCollected;
        private string _replacement;
        private string _foodLoss;
        private string _softScript;
        private string _deliverer;
        private string _fault;
       private string _actionRequired;
       private int _printLocn;
       private int _serviceBranch;
       private bool _quarters;
       private DateTime _returnDate;
       private string _failureReason;
       private string _delivered;
       private string _collected;
       private string _repaired;
       //CR1030 - needs to be included with Reports Release
        private DateTime _softscriptdate;
        # region Public (SR_ServiceRequest)
        public DateTime dateLogged
        {
            get
            {
                return _dateLogged;
            }
            set
            {
                _dateLogged = value;
            }
        }

        public DateTime dateReopened
        {
            get
            {
                return _dateReopened;
            }
            set
            {
                _dateReopened = value;
            }
        }

        public DateTime purchaseDate
        {
            get
            {
                return _purchaseDate;
            }
            set
            {
                _purchaseDate = value;
            }
        }

        public DateTime receivedDate
        {
            get
            {
                return _receivedDate;
            }
            set
            {
                _receivedDate = value;
            }
        }

        public decimal depositAmount
        {
            get
            {
                return _depositAmount;
            }
            set
            {
                _depositAmount = value;
            }
        }

        public decimal repairEstimate
        {
            get
            {
                return _repairEstimate;
            }
            set
            {
                _repairEstimate = value;
            }
        }

        public short stockLocn
        {
            get
            {
                return _stockLocn;
            }
            set
            {
                _stockLocn = value;
            }
        }

        public string deliveryDamage
        {
            get
            {
                return _deliveryDamage;
            }
            set
            {
                _deliveryDamage = value;
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string extWarranty
        {
            get
            {
                return _extWarranty;
            }
            set
            {
                _extWarranty = value;
            }
        }

        public string goodsOnLoan
        {
            get
            {
                return _goodsOnLoan;
            }
            set
            {
                _goodsOnLoan = value;
            }
        }

        public string modelNo
        {
            get
            {
                return _modelNo;
            }
            set
            {
                _modelNo = value;
            }
        }

        public string productCode
        {
            get
            {
                return _productCode;
            }
            set
            {
                _productCode = value;
            }
        }

        public string retailer      //CR1030 jec
        {
            get
            {
                return _retailer;
            }
            set
            {
                _retailer = value;
            }
        }

        public string custID      //CR1030 jec
        {
            get
            {
                return _custID;
            }
            set
            {
                _custID = value;
            }
        }
        public string reAssignCode      //CR1030 jec
        {
            get
            {
                return _reAssignCode;
            }
            set
            {
                _reAssignCode = value;
            }
        }
        public string reAssignedBy      //CR1030 jec
        {
            get
            {
                return _reAssignedBy;
            }
            set
            {
                _reAssignedBy = value;
            }
        }
        public decimal unitPrice
        {
            get
            {
                return _unitPrice;
            }
            set
            {
                _unitPrice = value;
            }
        }

        public string serialNo
        {
            get
            {
                return _serialNo;
            }
            set
            {
                _serialNo = value;
            }
        }

        public string serviceEvaln
        {
            get
            {
                return _serviceEvaln;
            }
            set
            {
                _serviceEvaln = value;
            }
        }

        public string serviceLocn
        {
            get
            {
                return _serviceLocn;
            }
            set
            {
                _serviceLocn = value;
            }
        }

        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public string depositPaid
        {
            get
            {
                return _depositPaid;
            }
            set
            {
                _depositPaid = value;
            }
        }

        public string transitNotes
        {
            get
            {
                return _transitNotes;
            }
            set
            {
                _transitNotes = value;
            }
        }

        //CR1030 - needs to be included with Reports Release
        public DateTime softscriptdate
        {
            get
            {
                return _softscriptdate;
            }
            set
            {
                _softscriptdate = value;
            }
        }

        public string comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        public DateTime dateCollected
        {
            get
            {
                return _dateCollected;
            }
            set
            {
                _dateCollected = value;
            }
        }
        #endregion

        #region Public (SR_Allocation)
        public DateTime dateAllocated
        {
            get
            {
                return _dateAllocated;
            }
            set
            {
                _dateAllocated = value;
            }
        }

        public string zone
        {
            get
            {
                return _zone;
            }
            set
            {
                _zone = value;
            }
        }

        public int technicianId
        {
            get
            {
                return _technicianId;
            }
            set
            {
                _technicianId = value;
            }
        }

        public DateTime partsDate
        {
            get
            {
                return _partsDate;
            }
            set
            {
                _partsDate = value;
            }
        }

        public DateTime repairDate
        {
            get
            {
                return _repairDate;
            }
            set
            {
                _repairDate = value;
            }
        }

        public string isAM
        {
            get
            {
                return _isAM;
            }
            set
            {
                _isAM = value;
            }
        }

        public string instructions
        {
            get
            {
                return _instructions;
            }
            set
            {
                _instructions = value;
            }
        }

        #endregion

        #region Public (SR_Resolution)
        public DateTime dateClosed
        {
            get
            {
                return _dateClosed;
            }
            set
            {
                _dateClosed = value;
            }
        }

        public string resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;
            }
        }

        public int resolutionChangedBy
        {
            get
            {
                return _resolutionChangedBy;
            }
            set
            {
                _resolutionChangedBy = value;
            }
        }

        public string chargeTo
        {
            get
            {
                return _chargeTo;
            }
            set
            {
                _chargeTo = value;
            }
        }

        public int chargeToChangedBy
        {
            get
            {
                return _chargeToChangedBy;
            }
            set
            {
                _chargeToChangedBy = value;
            }
        }

        public string chargeToMake
        {
            get
            {
                return _chargeToMake;
            }
            set
            {
                _chargeToMake = value;
            }
        }

        public string chargeToModel
        {
            get
            {
                return _chargeToModel;
            }
            set
            {
                _chargeToModel = value;
            }
        }

        public decimal hourlyRate
        {
            get
            {
                return _hourlyRate;
            }
            set
            {
                _hourlyRate = value;
            }
        }

        public decimal hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
            }
        }

        public decimal labourCost
        {
            get
            {
                return _labourCost;
            }
            set
            {
                _labourCost = value;
            }
        }

        public decimal additionalCost
        {
            get
            {
                return _additionalCost;
            }
            set
            {
                _additionalCost = value;
            }
        }

        public decimal TransportCost
        {
            get
            {
                return _transportCost;
            }
            set
            {
                _transportCost = value;
            }
        }

        public decimal totalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                _totalCost = value;
            }
        }

        public string goodsOnLoanCollected
        {
            get
            {
                return _goodsOnLoanCollected;
            }
            set
            {
                _goodsOnLoanCollected = value;
            }
        }

        public string replacement
        {
            get
            {
                return _replacement;
            }
            set
            {
                _replacement = value;
            }
        }

        public string foodLoss
        {
            get
            {
                return _foodLoss;
            }
            set
            {
                _foodLoss = value;
            }
        }

        public string softScript
        {
            get
            {
                return _softScript;
            }
            set
            {
                _softScript = value;
            }
        }

        public string deliverer
        {
            get
            {
                return _deliverer;
            }
            set
            {
                _deliverer = value;
            }
        }

       //UAT 453
       public string fault
       {
          get
          {
             return _fault;
          }
          set
         {
          _fault = value;
         }
       }

       //CR 949/958
       public string actionRequired
       {
          get { return _actionRequired; }
          set { _actionRequired = value;}
       }

       public int printLocn
       {
          get { return _printLocn; }
          set { _printLocn = value; }
       }

       public int serviceBranch
       {
          get { return _serviceBranch; }
          set { _serviceBranch = value; }
       }

       public bool quarters
       {
          get { return _quarters; }
          set { _quarters = value; }
       }

       public DateTime returnDate
       {
          get { return _returnDate; }
          set { _returnDate = value; }
       }

       public string failureReason
       {
          get { return _failureReason; }
          set { _failureReason = value; }
       }

       public string collected
       {
          get { return _collected; }
          set { _collected = value; }
       }

       public string delivered
       {
          get { return _delivered; }
          set { _delivered = value; }
       }

       public string repaired
       {
          get { return _repaired; }
          set { _repaired = value; }
       }

       private decimal _lbrCostEstimate; // CR 1024 (NM 29/04/2009)
       public decimal LbrCostEstimate
       {
           get { return _lbrCostEstimate; }
           set { _lbrCostEstimate = value; }
       }

       private decimal _adtnlLbrCostEstimate; // CR 1024 (NM 29/04/2009)
       public decimal AdtnlLbrCostEstimate
       {
           get { return _adtnlLbrCostEstimate; }
           set { _adtnlLbrCostEstimate = value; }
       }


       private decimal _transportCostEstimate; // CR 1024 (NM 29/04/2009)
       public decimal TransportCostEstimate
       {
           get { return _transportCostEstimate; }
           set { _transportCostEstimate = value; }
       }

       private string _technicianReport; // CR 1024 (NM 29/04/2009)
       public string TechnicianReport
       {
           get { return _technicianReport; }
           set { _technicianReport = value; }
       }

        #endregion

        #region Technician Fields
        private int _techId;
        public int TechId
        {
            get { return this._techId; }
            set { this._techId = value; }
        }

        private string _techTitle;
        public string TechTitle
        {
            get { return this._techTitle; }
            set { this._techTitle = value; }
        }

        private string _techFirstName;
        public string TechFirstName
        {
            get { return this._techFirstName; }
            set { this._techFirstName = value; }
        }

        private string _techLastName;
        public string TechLastName
        {
            get { return this._techLastName; }
            set { this._techLastName = value; }
        }

        private string _techAddress1;
        public string TechAddress1
        {
            get { return this._techAddress1; }
            set { this._techAddress1 = value; }
        }

        private string _techAddress2;
        public string TechAddress2
        {
            get { return this._techAddress2; }
            set { this._techAddress2 = value; }
        }

        private string _techAddress3;
        public string TechAddress3
        {
            get { return this._techAddress3; }
            set { this._techAddress3 = value; }
        }

        private string _techAddressPC;
        public string TechAddressPC
        {
            get { return this._techAddressPC; }
            set { this._techAddressPC = value; }
        }

        private string _techPhoneNo;
        public string TechPhoneNo
        {
            get { return this._techPhoneNo; }
            set { this._techPhoneNo = value; }
        }

        private string _techMobileNo;
        public string TechMobileNo
        {
            get { return this._techMobileNo; }
            set { this._techMobileNo = value; }
        }

        private string _techInternal;
        public string TechInternal
        {
            get { return this._techInternal; }
            set { this._techInternal = value; }
        }

        private string _techHoursFrom;
        public string TechHoursFrom
        {
            get { return this._techHoursFrom; }
            set { this._techHoursFrom = value; }
        }

        private string _techHoursTo;
        public string TechHoursTo
        {
            get { return this._techHoursTo; }
            set { this._techHoursTo = value; }
        }

        private int _techCallsPerDay;
        public int TechCallsPerDay
        {
            get { return this._techCallsPerDay; }
            set { this._techCallsPerDay = value; }
        }

        private object _vacationFrom;
        public object VacationFrom
        {
            get
            {
                return this._vacationFrom;
            }
            set
            {
                this._vacationFrom = value;
            }
        }

        private object _vacationTo;
        public object VacationTo
        {
            get
            {
                return this._vacationTo;
            }
            set
            {
                this._vacationTo = value;
            }
        }

        private string _techComments;
        public string TechComments
        {
            get
            {
                return _techComments;
            }
            set
            {
                _techComments = value;
            }
        }

        /*Below code added a part of - CR2018-010 - 31/10/18 - 
         Setting of max no. of Jobs & Validation with allocated jobs for a technician.*/

        private int _maxJobs;
        public int MaxJobs
        {
            get { return this._maxJobs; }
            set { this._maxJobs = value; }
        }


        private int _currJobs;
        public int CurrJobs
        {
            get { return this._currJobs; }
            set { this._currJobs = value; }
        }
        //CR Changes End




        #endregion

        #region ServiceRequestFunctions
        public DataTable CreateServiceRequest(SqlConnection conn, SqlTransaction trans,
            short serviceBranchNo,
            string serviceType,
            string custId,
            string accountNo,
            int invoiceNo,
            string user,
            short stockLocn,
            string prodCode,
            string description,
            DateTime purchaseDate,
            decimal unitPrice,
            string serialNo,
            int? printLocn,
            int itemId         // RI
            )
        {
            DataTable newServiceRequest = new DataTable();
            try
            {
                parmArray = new SqlParameter[14];
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceType", SqlDbType.Char, 1);
                parmArray[1].Value = serviceType;
                parmArray[2] = new SqlParameter("@CustId", SqlDbType.VarChar, 20);
                parmArray[2].Value = custId;
                parmArray[3] = new SqlParameter("@AccountNo", SqlDbType.Char, 12);
                parmArray[3].Value = accountNo;
                parmArray[4] = new SqlParameter("@InvoiceNo", SqlDbType.Int);
                parmArray[4].Value = invoiceNo;
                parmArray[5] = new SqlParameter("@User", SqlDbType.Int);
                parmArray[5].Value = user;
                parmArray[6] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
                parmArray[6].Value = stockLocn;
                parmArray[7] = new SqlParameter("@ProdCode", SqlDbType.VarChar, 18);            // RI
                parmArray[7].Value = prodCode;
                parmArray[8] = new SqlParameter("@UnitPrice", SqlDbType.Money);
                parmArray[8].Value = unitPrice;
                parmArray[9] = new SqlParameter("@Description", SqlDbType.VarChar, 50);
                parmArray[9].Value = description;
                parmArray[10] = new SqlParameter("@PurchaseDate", SqlDbType.SmallDateTime);
                parmArray[10].Value = purchaseDate;
                parmArray[11] = new SqlParameter("@SerialNo", SqlDbType.VarChar,30);
                parmArray[11].Value = serialNo;
                parmArray[12] = new SqlParameter("@PrintLocn", SqlDbType.SmallInt);
                if (printLocn.HasValue)
                    parmArray[12].Value = printLocn;
                else
                    parmArray[12].Value = DBNull.Value;
                parmArray[13] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[13].Value = itemId;

                this.RunSP(conn, trans, "DN_SRCreateNewSP", parmArray, newServiceRequest);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return newServiceRequest;
        }


        public void SaveServiceRequest(SqlConnection conn, SqlTransaction trans,
            short serviceBranchNo, int serviceUniqueId)
        {
            try
            {
               //CR 949/958 7 new parameters added
                //CR1030 - needs to be included with Reports Release
                //parmArray = new SqlParameter[63];
                parmArray = new SqlParameter[64];
                // SR_ServiceRequest
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[1].Value = serviceUniqueId;
                parmArray[2] = new SqlParameter("@DateLogged", SqlDbType.SmallDateTime);
                parmArray[2].Value = this._dateLogged;
                parmArray[3] = new SqlParameter("@DateReopened", SqlDbType.SmallDateTime);
                parmArray[3].Value = this._dateReopened;
                parmArray[4] = new SqlParameter("@PurchaseDate", SqlDbType.SmallDateTime);
                parmArray[4].Value = this._purchaseDate;
                parmArray[5] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
                parmArray[5].Value = this._stockLocn;
                parmArray[6] = new SqlParameter("@ProductCode", SqlDbType.VarChar, 18);             // RI jec 30/06/11
                parmArray[6].Value = this._productCode;
                parmArray[7] = new SqlParameter("@UnitPrice", SqlDbType.Money);
                parmArray[7].Value = this._unitPrice;
                parmArray[8] = new SqlParameter("@Description", SqlDbType.VarChar, 25);
                parmArray[8].Value = this._description;
                parmArray[9] = new SqlParameter("@ModelNo", SqlDbType.VarChar, 15);
                parmArray[9].Value = this._modelNo;
                parmArray[10] = new SqlParameter("@SerialNo", SqlDbType.VarChar, 30);
                parmArray[10].Value = this._serialNo;
                parmArray[11] = new SqlParameter("@Status", SqlDbType.Char, 1);
                parmArray[11].Value = this._status;
                parmArray[12] = new SqlParameter("@ReceivedDate", SqlDbType.SmallDateTime);
                parmArray[12].Value = this._receivedDate;
                parmArray[13] = new SqlParameter("@ServiceEvaln", SqlDbType.VarChar, 12);
                parmArray[13].Value = this._serviceEvaln;
                parmArray[14] = new SqlParameter("@ServiceLocn", SqlDbType.VarChar, 12);
                parmArray[14].Value = this._serviceLocn;
                parmArray[15] = new SqlParameter("@RepairEstimate", SqlDbType.Money);
                parmArray[15].Value = this._repairEstimate;
                parmArray[16] = new SqlParameter("@DeliveryDamage", SqlDbType.Char, 1);
                parmArray[16].Value = this._deliveryDamage;
                parmArray[17] = new SqlParameter("@ExtWarranty", SqlDbType.Char, 1);
                parmArray[17].Value = this._extWarranty;
                parmArray[18] = new SqlParameter("@GoodsOnLoan", SqlDbType.Char, 1);
                parmArray[18].Value = this._goodsOnLoan;
                parmArray[19] = new SqlParameter("@DepositAmount", SqlDbType.Money);
                parmArray[19].Value = this._depositAmount;
                parmArray[20] = new SqlParameter("@DepositPaid", SqlDbType.Char, 1);
                parmArray[20].Value = this._depositPaid;
                parmArray[21] = new SqlParameter("@TransitNotes", SqlDbType.VarChar, 200);
                parmArray[21].Value = this._transitNotes;
                parmArray[22] = new SqlParameter("@Comments", SqlDbType.VarChar, 5000);
                parmArray[22].Value = this._comments;
                parmArray[23] = new SqlParameter("@DateCollected", SqlDbType.SmallDateTime);
                parmArray[23].Value = this._dateCollected;
                // SR_Allocation
                parmArray[24] = new SqlParameter("@Zone", SqlDbType.VarChar, 12);
                parmArray[24].Value = this._zone;
                parmArray[25] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[25].Value = this._technicianId;
                parmArray[26] = new SqlParameter("@PartsDate", SqlDbType.SmallDateTime);
                parmArray[26].Value = this._partsDate;
                parmArray[27] = new SqlParameter("@RepairDate", SqlDbType.SmallDateTime);
                parmArray[27].Value = this._repairDate;
                parmArray[28] = new SqlParameter("@IsAM", SqlDbType.Char, 1);
                parmArray[28].Value = this._isAM;
                parmArray[29] = new SqlParameter("@Instructions", SqlDbType.VarChar, 200);
                parmArray[29].Value = this._instructions;
                // SR_Resolution
                parmArray[30] = new SqlParameter("@DateClosed", SqlDbType.SmallDateTime);
                parmArray[30].Value = this._dateClosed;
                parmArray[31] = new SqlParameter("@Resolution", SqlDbType.VarChar, 12);
                parmArray[31].Value = this._resolution;
                parmArray[32] = new SqlParameter("@ResolutionChangedBy", SqlDbType.Int);
                parmArray[32].Value = this._resolutionChangedBy;
                parmArray[33] = new SqlParameter("@ChargeTo", SqlDbType.VarChar, 12);
                parmArray[33].Value = this._chargeTo;
                parmArray[34] = new SqlParameter("@ChargeToChangedBy", SqlDbType.Int);
                parmArray[34].Value = this._chargeToChangedBy;
                parmArray[35] = new SqlParameter("@ChargeToMake", SqlDbType.VarChar, 30);
                parmArray[35].Value = this._chargeToMake;
                parmArray[36] = new SqlParameter("@ChargeToModel", SqlDbType.VarChar, 30);
                parmArray[36].Value = this._chargeToModel;
                parmArray[37] = new SqlParameter("@HourlyRate", SqlDbType.Money);
                parmArray[37].Value = this._hourlyRate;
                parmArray[38] = new SqlParameter("@Hours", SqlDbType.Money);
                parmArray[38].Value = this._hours;
                parmArray[39] = new SqlParameter("@LabourCost", SqlDbType.Money);
                parmArray[39].Value = this._labourCost;
                parmArray[40] = new SqlParameter("@AdditionalCost", SqlDbType.Money);
                parmArray[40].Value = this._additionalCost;
                parmArray[41] = new SqlParameter("@TotalCost", SqlDbType.Money);
                parmArray[41].Value = this._totalCost;
                parmArray[42] = new SqlParameter("@GoodsOnLoanCollected", SqlDbType.Char, 1);
                parmArray[42].Value = this._goodsOnLoanCollected;
                parmArray[43] = new SqlParameter("@Replacement", SqlDbType.Char, 1);
                parmArray[43].Value = this._replacement;
                parmArray[44] = new SqlParameter("@FoodLoss", SqlDbType.Char, 1);
                parmArray[44].Value = this._foodLoss;
                parmArray[45] = new SqlParameter("@SoftScript", SqlDbType.Char, 1);
                parmArray[45].Value = this._softScript;
                parmArray[46] = new SqlParameter("@Deliverer", SqlDbType.VarChar, 12);
                parmArray[46].Value = this._deliverer;
                parmArray[47] = new SqlParameter("@Fault", SqlDbType.Char, 4);
                parmArray[47].Value = this._fault;
                parmArray[48] = new SqlParameter("@ActionRequired", SqlDbType.VarChar, 50);
                parmArray[48].Value = this._actionRequired;
                parmArray[49] = new SqlParameter("@PrintLocn", SqlDbType.SmallInt);
                parmArray[49].Value = this._printLocn;
                parmArray[50] = new SqlParameter("@ReturnDate", SqlDbType.DateTime);
                parmArray[50].Value = this._returnDate;
                parmArray[51] = new SqlParameter("@FailureReason", SqlDbType.VarChar, 12);
                parmArray[51].Value = this._failureReason;
                parmArray[52] = new SqlParameter("@Delivered", SqlDbType.Char, 1);
                parmArray[52].Value = this._delivered;
                parmArray[53] = new SqlParameter("@CustomerCollected", SqlDbType.Char,1);
                parmArray[53].Value = this._collected;
                parmArray[54] = new SqlParameter("@RepairedHome", SqlDbType.Char, 1);
                parmArray[54].Value = this._repaired;
                parmArray[55] = new SqlParameter("@LbrCostEstimate", SqlDbType.Money); // CR 1024 (NM 29/04/2009)
                parmArray[55].Value = this._lbrCostEstimate;
                parmArray[56] = new SqlParameter("@AdtnlLbrCostEstimate", SqlDbType.Money); // CR 1024 (NM 29/04/2009)
                parmArray[56].Value = this._adtnlLbrCostEstimate;
                parmArray[57] = new SqlParameter("@TransportCostEstimate", SqlDbType.Money); // CR 1024 (NM 29/04/2009)
                parmArray[57].Value = this._transportCostEstimate;
                parmArray[58] = new SqlParameter("@TechnicianReport", SqlDbType.VarChar, 200); // CR 1024 (NM 29/04/2009)
                parmArray[58].Value = this._technicianReport;
                parmArray[59] = new SqlParameter("@TransportCost", SqlDbType.Money);
                parmArray[59].Value = this._transportCost; // CR 1024 (NM 29/04/2009)
                parmArray[60] = new SqlParameter("@Retailer", SqlDbType.VarChar, 25);       //CR1030 jec
                parmArray[60].Value = this._retailer;           //CR1030 jec
                parmArray[61] = new SqlParameter("@CustomerID", SqlDbType.VarChar, 20);       //CR1030 jec
                parmArray[61].Value = this._custID;           //CR1030 jec
                parmArray[62] = new SqlParameter("@ReAssignCode", SqlDbType.VarChar, 12);       //CR1030 jec
                parmArray[62].Value = this._reAssignCode;           //CR1030 jec
                //CR1030 - needs to be included with Reports Release
                //parmArray[63] = new SqlParameter("@SoftScriptDate", SqlDbType.DateTime);       //CR1030 RM
                //parmArray[63].Value = this._softscriptdate;           //CR1030 RM
                parmArray[63] = new SqlParameter("@ReAssignedBy", SqlDbType.Int);       //CR1030 jec
                parmArray[63].Value = this._reAssignedBy;           //CR1030 jec

                this.RunSP(conn, trans, "DN_SRSaveProductSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveServiceRequestComments(SqlConnection conn, SqlTransaction trans,
            short serviceBranchNo, int serviceUniqueId, string comments, DateTime dateReopened)
        {
            try
            {
                parmArray = new SqlParameter[4];
                // SR_ServiceRequest
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[1].Value = serviceUniqueId;
                parmArray[2] = new SqlParameter("@Comments", SqlDbType.VarChar, 5000);
                parmArray[2].Value = comments;
                parmArray[3] = new SqlParameter("@DateReopened", SqlDbType.DateTime);
                parmArray[3].Value = dateReopened;
                this.RunSP(conn, trans, "DN_SRSaveProductCommentsSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 18/06/09 - UAT(687) - Added stockLocn
        //IP - 29/06/09 - Merged Service 5.1 fix UAT(687)
        public void SavePartResolved(SqlConnection conn, SqlTransaction trans,
            //int serviceRequestNo, string partNo, decimal quantity,
              int serviceRequestNo, string partNo, int partID, decimal quantity,                                   //IP - 04/07/11 - CR1254 - RI - #3994
            decimal unitPrice, string description, string partType, int stockLocn)
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;
                //parmArray[1] = new SqlParameter("@PartNo", SqlDbType.VarChar, 8);
                //parmArray[1].Value = partNo;
                parmArray[1] = new SqlParameter("@PartNo", SqlDbType.VarChar, 18);                   //IP - 04/07/11 - CR1254 - RI - #3994
                parmArray[1].Value = partNo;
                parmArray[2] = new SqlParameter("@PartID", SqlDbType.Int);                          //IP - 04/07/11 - CR1254 - RI - #3994
                parmArray[2].Value = partID;                                                        //IP - 04/07/11 - CR1254 - RI - #3994
                parmArray[3] = new SqlParameter("@Quantity", SqlDbType.Float);
                parmArray[3].Value = quantity;
                parmArray[4] = new SqlParameter("@UnitPrice", SqlDbType.Money);
                parmArray[4].Value = unitPrice;
                parmArray[5] = new SqlParameter("@Description", SqlDbType.VarChar, 25);
                parmArray[5].Value = description;
                parmArray[6] = new SqlParameter("@PartType", SqlDbType.VarChar, 30);
                parmArray[6].Value = partType;
                parmArray[7] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
                parmArray[7].Value = stockLocn;

                this.RunSP(conn, trans, "DN_SRSavePartResolvedSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveSRCustomer(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP(conn, trans, "DN_SRSaveSRCustomerSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

       public void UpdateSRCustomer(SqlConnection conn, SqlTransaction trans,int serviceRequestNo, string custID, string title,
          string firstName, string lastName, decimal arrears, string address1, string address2, string address3, string postCode, string directions,
          string telHome,string telWork,string telMobile)
       {
          try
          {
             parmArray = new SqlParameter[14];
             parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
             parmArray[0].Value = serviceRequestNo;
             parmArray[1] = new SqlParameter("@custID", SqlDbType.VarChar,20);
             parmArray[1].Value = custID;
             parmArray[2] = new SqlParameter("@title", SqlDbType.VarChar,25);
             parmArray[2].Value = title;
             parmArray[3] = new SqlParameter("@firstName", SqlDbType.VarChar,30);
             parmArray[3].Value = firstName;
             parmArray[4] = new SqlParameter("@lastName", SqlDbType.VarChar,60);
             parmArray[4].Value = lastName;
             parmArray[5] = new SqlParameter("@arrears", SqlDbType.Decimal);
             parmArray[5].Value = arrears;
             parmArray[6] = new SqlParameter("@address1", SqlDbType.VarChar,50);
             parmArray[6].Value = address1;
             parmArray[7] = new SqlParameter("@address2", SqlDbType.VarChar,50);
             parmArray[7].Value = address2;
             parmArray[8] = new SqlParameter("@address3", SqlDbType.VarChar, 50); //UAT 722
             parmArray[8].Value = address3;
             parmArray[9] = new SqlParameter("@postCode", SqlDbType.VarChar,10);
             parmArray[9].Value = postCode;
             parmArray[10] = new SqlParameter("@directions", SqlDbType.VarChar,2000);
             parmArray[10].Value = directions;
             parmArray[11] = new SqlParameter("@telHome", SqlDbType.VarChar,30);
             parmArray[11].Value = telHome;
             parmArray[12] = new SqlParameter("@telWork", SqlDbType.VarChar,30);
             parmArray[12].Value = telWork;
             parmArray[13] = new SqlParameter("@telMobile", SqlDbType.VarChar,30);
             parmArray[13].Value = telMobile;

             this.RunSP(conn, trans, "DN_SRCustomerUpdateSP", parmArray);
          }
          catch (SqlException ex)
          {
             LogSqlException(ex);
             throw ex;
          }
       }

        public void GetChargeToDeposit(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo, out string customerId,
            out string customerAcctNo, out decimal depositAmount, out decimal depositPaid)
        {
            string internalAcctNo = "";
            string warrantyAcctNo = "";
            string supplierAcctNo = "";
            string supplierSpecialAcctNo = "";
            string delivererAcctNo = "";
            string supplierId = "";
            string delivererId = "";

            this.GetChargeToAccts(conn, trans,
                serviceRequestNo,
                out internalAcctNo, out warrantyAcctNo,
                out supplierAcctNo, out supplierSpecialAcctNo,
                out delivererAcctNo, out customerAcctNo,
                out supplierId, out delivererId, out customerId,
                out depositAmount, out depositPaid);
        }

        public void GetChargeToAccts(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo,
            out string internalAcctNo, out string warrantyAcctNo,
            out string supplierAcctNo, out string supplierSpecialAcctNo,
            out string delivererAcctNo, out string customerAcctNo,
            out string supplierId, out string delivererId, out string customerId,
            out decimal depositAmount, out decimal depositPaid)
        {
            internalAcctNo = "";
            warrantyAcctNo = "";
            supplierAcctNo = "";
            supplierSpecialAcctNo = "";
            delivererAcctNo = "";
            customerAcctNo = "";
            supplierId = "";
            delivererId = "";
            customerId = "";
            depositAmount = -1;
            depositPaid = 0;

            try
            {
                parmArray = new SqlParameter[12];
                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;
                parmArray[1] = new SqlParameter("@InternalAcctNo", SqlDbType.Char, 12);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@WarrantyAcctNo", SqlDbType.Char, 12);
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@SupplierAcctNo", SqlDbType.Char, 12);
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@SupplierSpecialAcctNo", SqlDbType.Char, 12);
                parmArray[4].Direction = ParameterDirection.Output;
                parmArray[5] = new SqlParameter("@DelivererAcctNo", SqlDbType.Char, 12);
                parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@CustomerAcctNo", SqlDbType.Char, 12);
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@SupplierId", SqlDbType.VarChar, 12);
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@DelivererId", SqlDbType.VarChar, 12);
                parmArray[8].Direction = ParameterDirection.Output;
                parmArray[9] = new SqlParameter("@CustomerId", SqlDbType.VarChar, 20);
                parmArray[9].Direction = ParameterDirection.Output;
                parmArray[10] = new SqlParameter("@DepositAmount", SqlDbType.Money);
                parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@DepositPaid", SqlDbType.Money);
                parmArray[11].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_SRGetChargeToAcctsSP", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    internalAcctNo = Convert.ToString(parmArray[1].Value);
                if (parmArray[2].Value != DBNull.Value)
                    warrantyAcctNo = Convert.ToString(parmArray[2].Value);
                if (parmArray[3].Value != DBNull.Value)
                    supplierAcctNo = Convert.ToString(parmArray[3].Value);
                if (parmArray[4].Value != DBNull.Value)
                    supplierSpecialAcctNo = Convert.ToString(parmArray[4].Value);
                if (parmArray[5].Value != DBNull.Value)
                    delivererAcctNo = Convert.ToString(parmArray[5].Value);
                if (parmArray[6].Value != DBNull.Value)
                    customerAcctNo = Convert.ToString(parmArray[6].Value);
                if (parmArray[7].Value != DBNull.Value)
                    supplierId = Convert.ToString(parmArray[7].Value);
                if (parmArray[8].Value != DBNull.Value)
                    delivererId = Convert.ToString(parmArray[8].Value);
                if (parmArray[9].Value != DBNull.Value)
                    customerId = Convert.ToString(parmArray[9].Value);
                if (parmArray[10].Value != DBNull.Value)
                    depositAmount = Convert.ToDecimal(parmArray[10].Value);
                if (parmArray[11].Value != DBNull.Value)
                    depositPaid = Convert.ToDecimal(parmArray[11].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveChargeTo(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo, short sortOrder, decimal actualCost, decimal internalCharge,
            decimal extWarranty, decimal supplier, decimal deliverer, decimal customer)
        {
            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;
                parmArray[1] = new SqlParameter("@SortOrder", SqlDbType.SmallInt);
                parmArray[1].Value = sortOrder;
                parmArray[2] = new SqlParameter("@ActualCost", SqlDbType.Money);
                parmArray[2].Value = actualCost;
                parmArray[3] = new SqlParameter("@InternalCharge", SqlDbType.Money);
                parmArray[3].Value = internalCharge;
                parmArray[4] = new SqlParameter("@ExtWarranty", SqlDbType.Money);
                parmArray[4].Value = extWarranty;
                parmArray[5] = new SqlParameter("@Supplier", SqlDbType.Money);
                parmArray[5].Value = supplier;
                parmArray[6] = new SqlParameter("@Deliverer", SqlDbType.Money);
                parmArray[6].Value = deliverer;
                parmArray[7] = new SqlParameter("@Customer", SqlDbType.Money);
                parmArray[7].Value = customer;

                this.RunSP(conn, trans, "DN_SRSaveChargeToSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeletePartList(SqlConnection conn, SqlTransaction trans, int serviceRequestNo)
        {
            try
            {
                parmArray = new SqlParameter[1];
                // SR_ServiceRequest
                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP(conn, trans, "DN_SRDeletePartListSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveHistory(SqlConnection conn, SqlTransaction trans,
            string accountNo, int invoiceNo, short stockLocn, int itemId,string serialNo)           // RI
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.Char, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@InvoiceNo", SqlDbType.Int);
                parmArray[1].Value = invoiceNo;
                parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
                parmArray[2].Value = stockLocn;
                //parmArray[3] = new SqlParameter("@ProductCode", SqlDbType.VarChar, 8);
                //parmArray[3].Value = productCode;
                parmArray[3] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[3].Value = itemId;
                parmArray[4] = new SqlParameter("@SerialNo", SqlDbType.VarChar, 30);
                parmArray[4].Value = serialNo;


                this.RunSP(conn, trans, "DN_SRSaveHistorySP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void LinkChargeToAccount(SqlConnection conn, SqlTransaction trans,
            int serviceUniqueId, string acctNo, string chargeType)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[0].Value = serviceUniqueId;
                parmArray[1] = new SqlParameter("@AcctNo", SqlDbType.Char, 12);
                parmArray[1].Value = acctNo;
                parmArray[2] = new SqlParameter("@ChargeType", SqlDbType.Char, 1);
                parmArray[2].Value = chargeType;

                this.RunSP(conn, trans, "DN_SRLinkChargeToAccountSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

       public string GetDeliveryAccount(SqlConnection conn, SqlTransaction trans,
            int serviceUniqueId, string deliveryID)
       {
          string deliveryAcctNo;
          try
          {
             parmArray = new SqlParameter[2];
             parmArray[0] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
             parmArray[0].Value = serviceUniqueId;
             parmArray[1] = new SqlParameter("@deliveryID", SqlDbType.VarChar,20);
             parmArray[1].Value = deliveryID;

             deliveryAcctNo = RunSPdr(conn, trans, "DN_SRGetDeliveryAccountSP", parmArray);
          }
          catch (SqlException ex)
          {
             LogSqlException(ex);
             throw ex;
          }
          if (deliveryAcctNo != DBNull.Value.ToString())
          {
             return deliveryAcctNo;
          }
          else
          {
             return String.Empty;
          }
       }


        public DataSet GetSRCustomer(
            string custId, short serviceBranchNo, int serviceUniqueId, string acctNo, int invoiceNo, short branchNo)
        {
            DataSet srCustomer = new DataSet();
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@CustId", SqlDbType.VarChar, 20);
                parmArray[0].Value = custId;
                parmArray[1] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[1].Value = serviceBranchNo;
                parmArray[2] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[2].Value = serviceUniqueId;
                parmArray[3] = new SqlParameter("@AcctNo", SqlDbType.Char, 12);
                parmArray[3].Value = acctNo;
                parmArray[4] = new SqlParameter("@InvoiceNo", SqlDbType.Int);
                parmArray[4].Value = invoiceNo;
                parmArray[5] = new SqlParameter("@BranchNo", SqlDbType.SmallInt);
                parmArray[5].Value = branchNo;

                this.RunSP("DN_SRGetCustomerSP", parmArray, srCustomer);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            if (srCustomer != null)
                return srCustomer;
            else
                return null;
        }
        
        //IP - 31/07/09 - UAT(741) - added bool to check if the account being searched on is a Paid and Taken account.
        public DataSet GetCourtsAccount(short serviceBranchNo, int serviceUniqueId, string acctNo, int invoiceNo, short branchNo,int user, out bool isPaidAndTakenAcct)
        {
            DataSet serviceRequest = new DataSet();
            isPaidAndTakenAcct = false;

            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[1].Value = serviceUniqueId;
                parmArray[2] = new SqlParameter("@AcctNo", SqlDbType.Char, 12);
                parmArray[2].Value = acctNo;
                parmArray[3] = new SqlParameter("@InvoiceNo", SqlDbType.Int);
                parmArray[3].Value = invoiceNo;
                parmArray[4] = new SqlParameter("@BranchNo", SqlDbType.SmallInt);
                parmArray[4].Value = branchNo;
                parmArray[5] = new SqlParameter("@User", SqlDbType.Int);
                parmArray[5].Value = user;
                parmArray[6] = new SqlParameter("@IsPaidAndTakenAcct", SqlDbType.Bit);
                parmArray[6].Direction = ParameterDirection.Output;


                this.RunSP("DN_SRGetCourtsAccountSP", parmArray, serviceRequest);

                if (parmArray[6].Value != DBNull.Value)
                    isPaidAndTakenAcct = (Convert.ToBoolean(parmArray[6].Value));
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return serviceRequest;
        }


        public DataSet GetNonCourtsAccount(short serviceBranchNo, int serviceUniqueId, string custId)
        {
            DataSet serviceRequest = new DataSet();
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[1].Value = serviceUniqueId;
                parmArray[2] = new SqlParameter("@CustId", SqlDbType.NVarChar, 20);
                parmArray[2].Value = custId;

                this.RunSP("DN_SRGetNonCourtsAccountSP", parmArray, serviceRequest);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return serviceRequest;
        }


        public DataSet GetInternalStock(short serviceBranchNo, int serviceUniqueId, string custId)
        {
            DataSet serviceRequest = new DataSet();
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@ServiceBranchNo", SqlDbType.SmallInt);
                parmArray[0].Value = serviceBranchNo;
                parmArray[1] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[1].Value = serviceUniqueId;
                parmArray[2] = new SqlParameter("@CustId", SqlDbType.NVarChar, 20);
                parmArray[2].Value = custId;

                this.RunSP("DN_SRGetInternalStockSP", parmArray, serviceRequest);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return serviceRequest;
        }

        public DataSet GetServiceRequestNo(int serviceNo, int branchNo,int user)
        {
            DataSet serviceRequest = new DataSet();
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[0].Value = serviceNo;

                parmArray[1] = new SqlParameter("@ServiceBranchNo", SqlDbType.NVarChar, 20);
                parmArray[1].Value = branchNo;

                parmArray[2] = new SqlParameter("@User", SqlDbType.Int);
                parmArray[2].Value = user;

                this.RunSP("DN_SRGetSingleServiceRequestSP", parmArray, serviceRequest);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return serviceRequest;
        }

        /// <summary>
        ///  // added for CR ZEN/UNC/CRF/CR2018-009 Service Installation, cancel installation service when shipment is rejected.
        /// </summary>
        /// <param name="acctno"></param>
        /// <param name="itemno"></param>
        /// <returns></returns>
        public DataSet GetServiceRequestDetails(string acctno, string itemno)
        {
            DataSet serviceRequest = new DataSet();
            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,30);
                parmArray[0].Value = acctno;

                parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 30);
                parmArray[1].Value = itemno;

    

                this.RunSP("DN_SRGetServiceRequestDetailsSP", parmArray, serviceRequest);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return serviceRequest;
        }

        public DataTable GetTechnicianDiary(int technicianId)
        {
            DataTable techinicianDiary = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                this.RunSP("DN_SRGetTechnicianDiarySP", parmArray, techinicianDiary);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return techinicianDiary;
        }

        public DataTable CheckTechnicianSlots(int technicianId, int calls)
        {
            DataTable techinicianSlots = new DataTable();
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;
                parmArray[1] = new SqlParameter("@Calls", SqlDbType.SmallInt);
                parmArray[1].Value = Convert.ToInt16(calls);

                this.RunSP("DN_SRCheckCallsPerDaySP", parmArray, techinicianSlots);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return techinicianSlots;
        }

        public DataTable CheckPartsDate(int serviceRequestNo)
        {
            DataTable srPartsDates = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@SRno", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP("DN_SRCheckPartsDateSP", parmArray, srPartsDates);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return srPartsDates;
        }


        public void BookServiceRequest(SqlConnection conn, SqlTransaction trans,
            string zone, int technicianId, DateTime slotDate, short slotNo, short multiSlot, string bookingType, int serviceUniqueId,string IsAM,
            int allocatedBy, out bool notFound, out bool alreadyBooked, out DateTime curSlotDate, out string curBookingType, string reassignCode, string reassignedBy)
        {
            notFound = false;
            alreadyBooked = false;
            curSlotDate = Date.blankDate;
            curBookingType = "";

            try
            {
                parmArray = new SqlParameter[15];
                parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 12);
                parmArray[0].Value = zone;
                parmArray[1] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[1].Value = technicianId;
                parmArray[2] = new SqlParameter("@SlotDate", SqlDbType.SmallDateTime);
                parmArray[2].Value = slotDate;
                parmArray[3] = new SqlParameter("@SlotNo", SqlDbType.SmallInt);
                parmArray[3].Value = slotNo;
                parmArray[4] = new SqlParameter("@MultiSlot", SqlDbType.SmallInt);
                parmArray[4].Value = multiSlot;
                parmArray[5] = new SqlParameter("@BookingType", SqlDbType.Char, 1);
                parmArray[5].Value = bookingType;
                parmArray[6] = new SqlParameter("@ServiceUniqueId", SqlDbType.Int);
                parmArray[6].Value = serviceUniqueId;
                parmArray[7] = new SqlParameter("@IsAM", SqlDbType.Char,1);
                parmArray[7].Value = IsAM;
                parmArray[8] = new SqlParameter("@allocatedBy", SqlDbType.Int);
                parmArray[8].Value = allocatedBy;
                parmArray[9] = new SqlParameter("@NotFound", SqlDbType.Char, 1);
                parmArray[9].Direction = ParameterDirection.Output;
                parmArray[10] = new SqlParameter("@AlreadyBooked", SqlDbType.Char, 1);
                parmArray[10].Direction = ParameterDirection.Output;
                parmArray[11] = new SqlParameter("@CurSlotDate", SqlDbType.SmallDateTime);
                parmArray[11].Direction = ParameterDirection.Output;
                parmArray[12] = new SqlParameter("@CurBookingType", SqlDbType.Char, 1);
                parmArray[12].Direction = ParameterDirection.Output;
                parmArray[13] = new SqlParameter("@ReAssignCode", SqlDbType.VarChar, 12);       //CR1030 jec
                parmArray[13].Value = reassignCode;
                parmArray[14] = new SqlParameter("@ReAssignedBy", SqlDbType.Int);           //CR1030 jec
                parmArray[14].Value = reassignedBy;

                this.RunSP(conn, trans, "DN_SRBookServiceRequestSP", parmArray);

                if (parmArray[9].Value != DBNull.Value)
                    notFound = (Convert.ToString(parmArray[9].Value) == "Y");
                if (parmArray[10].Value != DBNull.Value)
                    alreadyBooked = (Convert.ToString(parmArray[10].Value) == "Y");
                if (parmArray[11].Value != DBNull.Value)
                    curSlotDate = Convert.ToDateTime(parmArray[11].Value);
                if (parmArray[12].Value != DBNull.Value)
                    curBookingType = Convert.ToString(parmArray[12].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void FreeServiceRequest(SqlConnection conn, SqlTransaction trans,
            int technicianId, DateTime slotDate, short slotNo)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;
                parmArray[1] = new SqlParameter("@SlotDate", SqlDbType.SmallDateTime);
                parmArray[1].Value = slotDate;
                parmArray[2] = new SqlParameter("@SlotNo", SqlDbType.SmallInt);
                parmArray[2].Value = slotNo;

                this.RunSP(conn, trans, "DN_SRFreeServiceRequestSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable LoadPriceIndexMatrix()
        {
            DataTable priceIndexMatrix = new DataTable();
            try
            {
                this.RunSP("DN_SRLoadPriceIndexMatrixSP", priceIndexMatrix);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return priceIndexMatrix;
        }


        public void ClearPriceIndexMatrix(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_SRClearPriceIndexMatrixSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SavePriceIndexMatrix(SqlConnection conn, SqlTransaction trans,
            string supplier, string product, short year, string partType,
            short partPercent, decimal partLimit,
            short labourPercent, decimal labourLimit,
            short additionalPercent, decimal additionalLimit)
        {
            try
            {
                parmArray = new SqlParameter[10];
                parmArray[0] = new SqlParameter("@Supplier", SqlDbType.VarChar, 30);
                parmArray[0].Value = supplier;
                parmArray[1] = new SqlParameter("@Product", SqlDbType.VarChar, 30);
                parmArray[1].Value = product;
                parmArray[2] = new SqlParameter("@Year", SqlDbType.SmallInt);
                parmArray[2].Value = year;
                parmArray[3] = new SqlParameter("@PartType", SqlDbType.VarChar, 30);
                parmArray[3].Value = partType;
                parmArray[4] = new SqlParameter("@PartPercent", SqlDbType.Money);
                parmArray[4].Value = partPercent;
                parmArray[5] = new SqlParameter("@PartLimit", SqlDbType.Money);
                parmArray[5].Value = partLimit;
                parmArray[6] = new SqlParameter("@LabourPercent", SqlDbType.Money);
                parmArray[6].Value = labourPercent;
                parmArray[7] = new SqlParameter("@LabourLimit", SqlDbType.Money);
                parmArray[7].Value = labourLimit;
                parmArray[8] = new SqlParameter("@AdditionalPercent", SqlDbType.Money);
                parmArray[8].Value = additionalPercent;
                parmArray[9] = new SqlParameter("@AdditionalLimit", SqlDbType.Money);
                parmArray[9].Value = additionalLimit;

                this.RunSP(conn, trans, "DN_SRSavePriceIndexMatrixSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable LoadPriceIndexMakes()
        {
            DataTable priceIndexMakes = new DataTable(TN.ServiceMake);
            try
            {
                this.RunSP("DN_SRLoadPriceIndexMakesSP", priceIndexMakes);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return priceIndexMakes;
        }


        public DataTable LoadPriceIndexModels()
        {
            DataTable priceIndexModels = new DataTable(TN.ServiceModel);
            try
            {
                this.RunSP("DN_SRLoadPriceIndexModelsSP", priceIndexModels);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return priceIndexModels;
        }


        public DataTable GetTechnicians(DateTime dateAvailable)
        {
            DataTable technicianList = new DataTable(TN.Technician);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@DateAvailable", SqlDbType.SmallDateTime);
                parmArray[0].Value = dateAvailable;

                this.RunSP("DN_SRGetTechniciansSP", parmArray, technicianList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technicianList;
        }


        public DataTable GetTechniciansByZone(DateTime dateAvailable)
        {
            DataTable technicianList = new DataTable(TN.TechnicianByZone);
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@DateAvailable", SqlDbType.SmallDateTime);
                parmArray[0].Value = dateAvailable;

                this.RunSP("DN_SRGetTechniciansByZoneSP", parmArray, technicianList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technicianList;
        }


        public DataTable GetTechnicianList()
        {
            try
            {
                DataTable technician = new DataTable(TN.Technician);
                this.RunSP("DN_SRGetTechnicianSP", technician);
                return technician;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



        /// <summary>
        /// Saves a zone for a specified technician
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="technicianId"></param>
        /// <param name="zoneCode"></param>
        public void SaveTechnicianZones(SqlConnection conn, SqlTransaction trans,
            int technicianId, string zoneCode)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;
                parmArray[1] = new SqlParameter("@ZoneCode", SqlDbType.NVarChar, 100);
                parmArray[1].Value = zoneCode;

                this.RunSP(conn, trans, "DN_SRSaveTechnicianZones", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Saves the same zone for all technicians 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="zoneCode"></param>
        public void SaveTechnicianZones(SqlConnection conn, SqlTransaction trans
            , string zoneCode)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@ZoneCode", SqlDbType.NVarChar, 100);
                parmArray[0].Value = zoneCode;

                this.RunSP(conn, trans, "DN_SRSaveTechnicianZones", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetServiceAllocationPayment(int technicianId)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                DataTable serviceAllocation = new DataTable(TN.Allocations); 
                this.RunSP("DN_SRGetServiceAllocationPayment", parmArray, serviceAllocation);

                return serviceAllocation;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void BalanceAccounts(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_SRBalanceAccountsSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

#endregion

        public bool CheckTechMainPermissions(int userId)
        {
            bool permit = false;
            try
            {

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@userId", SqlDbType.NVarChar, 12);
                parmArray[0].Value = userId;
                parmArray[1] = new SqlParameter("@permit", SqlDbType.SmallInt);
                parmArray[1].Value = 0;
                parmArray[1].Direction = ParameterDirection.Output;

                RunSP("DN_SRTechMaintPermission", parmArray);

                if (parmArray[1].Value != DBNull.Value)
                    permit = Convert.ToBoolean(parmArray[1].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return permit;
        }
        public void SaveCustomerDetails(SqlConnection conn, SqlTransaction trans, CustomerDetails cusDetails)
        {
           try
           {
              parmArray = new SqlParameter[13];
              parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
              parmArray[0].Value = cusDetails.customerID;
              parmArray[1] = new SqlParameter("@title", SqlDbType.VarChar, 20);
              parmArray[1].Value = cusDetails.title;
              parmArray[2] = new SqlParameter("@firstname", SqlDbType.VarChar, 20);
              parmArray[2].Value = cusDetails.firstName;
              parmArray[3] = new SqlParameter("@lastname", SqlDbType.VarChar, 20);
              parmArray[3].Value = cusDetails.lastName;
              parmArray[4] = new SqlParameter("@address1", SqlDbType.VarChar, 20);
              parmArray[4].Value = cusDetails.address1;
              parmArray[5] = new SqlParameter("@address2", SqlDbType.VarChar, 20);
              parmArray[5].Value = cusDetails.address2;
              parmArray[6] = new SqlParameter("@address3", SqlDbType.VarChar, 20);
              parmArray[6].Value = cusDetails.address3;
              parmArray[7] = new SqlParameter("@postcode", SqlDbType.VarChar, 20);
              parmArray[7].Value = cusDetails.postcode;
              parmArray[8] = new SqlParameter("@directions", SqlDbType.VarChar, 400);
              parmArray[8].Value = cusDetails.directions;
              parmArray[9] = new SqlParameter("@hometel", SqlDbType.VarChar, 20);
              parmArray[9].Value = cusDetails.homePhone;
              parmArray[10] = new SqlParameter("@worktel", SqlDbType.VarChar, 20);
              parmArray[10].Value = cusDetails.workPhone;
              parmArray[11] = new SqlParameter("@mobile", SqlDbType.VarChar, 20);
              parmArray[11].Value = cusDetails.mobile;
              parmArray[12] = new SqlParameter("@branchNo", SqlDbType.Int);
              parmArray[12].Value = cusDetails.branchNo;
              this.RunSP(conn, trans, "DN_SRCustomerUpdateForCashGoSP", parmArray);
           }
           catch (SqlException ex)
           {
              LogSqlException(ex);
              throw ex;
           }
        }

        public int GetWarrantyLength(string p, int l)
        {
            int n = 1;

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@prodCode", SqlDbType.VarChar);
                parmArray[0].Value = p;
                parmArray[1] = new SqlParameter("@stockLocn", SqlDbType.Int);
                parmArray[1].Value = l;

                n = this.RunSP("DN_SRGetWarrantyLength", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return n;

        }

        public void SRBatchPrintUpdatePrinted(string srPrintedList, Int16 branchno)
        {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@srPrintedList", SqlDbType.VarChar);
            parmArray[0].Value = srPrintedList;
            parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
            parmArray[1].Value = branchno;

            RunNonQuery("SRBatchPrintUpdatePrinted", parmArray, false);
        }

        /// <summary>
        ///  // added for CR ZEN/UNC/CRF/CR2018-009 Service Installation, cancel installation service when shipment is rejected.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Status"></param>
        /// <param name="userid"></param>
        /// <param name="DeliveryDate"></param>
        public void UpdateServiceRequest(int id ,string Status, int userid, string DeliveryDate)
        {
            
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@id", SqlDbType.Int);
                parmArray[0].Value = id;
                parmArray[1] = new SqlParameter("@userid", SqlDbType.Int);
                parmArray[1].Value = userid;
                parmArray[2] = new SqlParameter("@Status", SqlDbType.NVarChar);
                parmArray[2].Value = Status;
                parmArray[3] = new SqlParameter("@DeliveryDate", SqlDbType.NVarChar);
                parmArray[3].Value = DeliveryDate;

                RunNonQuery("SRUpdateServiceRequest", parmArray, false);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            
        }

    }
}

