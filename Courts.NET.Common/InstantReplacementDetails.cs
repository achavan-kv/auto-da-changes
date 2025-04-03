using System;
using System.Xml;
using STL.Common.Constants.Enums;

namespace STL.Common
{
	/// <summary>
	/// Summary description for InstantReplacementDetails.
	/// </summary>
	public class InstantReplacementDetails
	{
		#region private members
		private int _elapsedMonths = 0;
		private string _itemNo = "";
		private string _warrantyNo = "";
		private int _agreementNo = 0;
		private int _newBuffNo = 0;
		private string _returnReason = "";
		private string _returnItemNo = "";
		private short _returnStockLocn = 0;
		private decimal _price = 0;
		private decimal _quantity = 0;
		private decimal _orderValue = 0;
		private OneForOneTimePeriod _timePeriod;
		private decimal _taxAmount = 0;
		private decimal _taxRate = 0;
		private XmlNode _currentWarranty = null;
		private string _description = "";
		private string _contractno = "";
        private short _stockLocn = 0;
        private bool _notify = false;
        		
		private const string _xmlTemplate = 
			"<REPLACEMENT>"+
				"<ITEMNO/>"+
				"<WARRANTYNO/>"+
				"<AGREEMENTNO/>"+
				"<NEWBUFFNO/>"+
				"<RETURNREASON/>"+
				"<RETURNITEMNO/>"+
				"<RETURNSTOCKLOCN/>"+
				"<PRICE/>"+
				"<QUANTITY/>"+
				"<ORDERVALUE/>"+
				"<TIMEPERIOD/>"+
				"<TAXAMOUNT/>"+
				"<TAXRATE/>"+
				"<CURRENTWARRANTY/>"+
				"<ELAPSEDMONTHS/>"+
				"<DESCRIPTION/>"+
				"<CONTRACTNO/>"+
                "<STOCKLOCN/>" +
                "<NOTIFY/>" +
                "<ITEMID/>" +
                "<WARRANTYID/>" +                //IP/NM - 18/05/11 -CR1212 - #3627
                "<RETURNITEMID/>" +              //RI - CR1212
            "</REPLACEMENT>";
				
		#endregion

		#region public accessor properties 
		public string ContractNo
		{
			get{return _contractno;}
			set{_contractno = value;}
		}
		public string Description
		{
			get{return _description;}
			set{_description = value;}
		}
		public int ElapsedMonths
		{
			get{return _elapsedMonths;}
			set{_elapsedMonths = value;}
		}
		public XmlNode CurrentWarranty
		{
			get{return _currentWarranty;}
			set{_currentWarranty = value;}
		}
		public decimal TaxRate
		{
			get{return _taxRate;}
			set{_taxRate = value;}
		}
		public decimal TaxAmount
		{
			get{return _taxAmount;}
			set{_taxAmount = value;}
		}
		public OneForOneTimePeriod TimePeriod
		{
			get{return _timePeriod;}
			set{_timePeriod = value;}
		}
		public decimal Price
		{
			get{return _price;}
			set{_price = value;}
		}
		public decimal Quantity
		{
			get{return _quantity;}
			set{_quantity = value;}
		}
		public decimal OrderValue
		{
			get{return _orderValue;}
			set{_orderValue = value;}
		}
		public string ReturnReason
		{
			get{return _returnReason;}
			set{_returnReason = value;}
		}
		public string ReturnItemNo
		{
			get{return _returnItemNo;}
			set{_returnItemNo = value;}
		}
		public short ReturnStockLocn
		{
			get{return _returnStockLocn;}
			set{_returnStockLocn = value;}
		}
		public string ItemNo
		{
			get{return _itemNo;}
			set{_itemNo = value;}
		}

        public short StockLocn
        {
            get { return _stockLocn; }
            set { _stockLocn = value; }
        }
		public string WarrantyNo
		{
			get{return _warrantyNo;}
			set{_warrantyNo = value;}
		}
		public int AgreementNo
		{
			get{return _agreementNo;}
			set{_agreementNo = value;}
		}
		public int NewBuffNo
		{
			get{return _newBuffNo;}
			set{_newBuffNo = value;}
		}
        public bool Notify
        {
            get { return _notify; }
            set { _notify = value; }
        }
        public int ItemId {get; set;}

        public int WarrantyID { get; set; }     //IP/NM - 18/05/11 -CR1212 - #3627

        public int ReturnItemId { get; set; }    
        
        #endregion

        #region public methods
        public static InstantReplacementDetails DeSerialise(XmlNode xml)
		{
			InstantReplacementDetails ird = new InstantReplacementDetails();
			ird.ItemNo = (xml.SelectSingleNode("//ITEMNO")).InnerText;
			ird.WarrantyNo = (xml.SelectSingleNode("//WARRANTYNO")).InnerText;
			ird.AgreementNo = Convert.ToInt32((xml.SelectSingleNode("//AGREEMENTNO")).InnerText);
			ird.NewBuffNo = Convert.ToInt32((xml.SelectSingleNode("//NEWBUFFNO")).InnerText);
			ird.ReturnReason = (xml.SelectSingleNode("//RETURNREASON")).InnerText;
			ird.ReturnItemNo = (xml.SelectSingleNode("//RETURNITEMNO")).InnerText;
			ird.ReturnStockLocn = Convert.ToInt16((xml.SelectSingleNode("//RETURNSTOCKLOCN")).InnerText);
			ird.Price = Convert.ToDecimal((xml.SelectSingleNode("//PRICE")).InnerText);
			ird.Quantity = Convert.ToDecimal((xml.SelectSingleNode("//QUANTITY")).InnerText);
			ird.OrderValue = Convert.ToDecimal((xml.SelectSingleNode("//ORDERVALUE")).InnerText);
			ird.TaxRate = Convert.ToDecimal((xml.SelectSingleNode("//TAXRATE")).InnerText);
			ird.TaxAmount = Convert.ToDecimal((xml.SelectSingleNode("//TAXAMOUNT")).InnerText);
			ird.CurrentWarranty = xml.SelectSingleNode("//CURRENTWARRANTY").FirstChild;
			ird.ElapsedMonths = Convert.ToInt32(xml.SelectSingleNode("//ELAPSEDMONTHS").InnerText);
			ird.Description = xml.SelectSingleNode("//DESCRIPTION").InnerText;
			ird.ContractNo = xml.SelectSingleNode("//CONTRACTNO").InnerText;
            ird.StockLocn = Convert.ToInt16((xml.SelectSingleNode("//STOCKLOCN")).InnerText);
            ird.Notify = Convert.ToBoolean((xml.SelectSingleNode("//NOTIFY")).InnerText);
            ird.ItemId = Convert.ToInt32((xml.SelectSingleNode("//ITEMID")).InnerText);
            ird.WarrantyID = Convert.ToInt32((xml.SelectSingleNode("//WARRANTYID")).InnerText); //IP/NM - 18/05/11 -CR1212 - #3627
            ird.ReturnItemId = Convert.ToInt32((xml.SelectSingleNode("//RETURNITEMID")).InnerText);

			string timePeriod = (xml.SelectSingleNode("//TIMEPERIOD")).InnerText;
			if(timePeriod == OneForOneTimePeriod.IRPeriod4.ToString())
				ird.TimePeriod = OneForOneTimePeriod.IRPeriod4;
			else if(timePeriod == OneForOneTimePeriod.IRPeriod2.ToString())
				ird.TimePeriod = OneForOneTimePeriod.IRPeriod2;
			else if(timePeriod == OneForOneTimePeriod.IRPeriod1.ToString())
				ird.TimePeriod = OneForOneTimePeriod.IRPeriod1;
			else if(timePeriod == OneForOneTimePeriod.IRPeriod3.ToString())
				ird.TimePeriod = OneForOneTimePeriod.IRPeriod3;
			
			return ird;
		}

		public XmlNode Serialise()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(_xmlTemplate);
			(doc.SelectSingleNode("//ITEMNO")).InnerText = _itemNo;
			(doc.SelectSingleNode("//WARRANTYNO")).InnerText = _warrantyNo;
			(doc.SelectSingleNode("//AGREEMENTNO")).InnerText = _agreementNo.ToString();
			(doc.SelectSingleNode("//NEWBUFFNO")).InnerText = _newBuffNo.ToString();
			(doc.SelectSingleNode("//RETURNREASON")).InnerText = _returnReason;
			(doc.SelectSingleNode("//RETURNITEMNO")).InnerText = _returnItemNo;
			(doc.SelectSingleNode("//RETURNSTOCKLOCN")).InnerText = _returnStockLocn.ToString();
			(doc.SelectSingleNode("//PRICE")).InnerText = _price.ToString();
			(doc.SelectSingleNode("//QUANTITY")).InnerText = _quantity.ToString();
			(doc.SelectSingleNode("//ORDERVALUE")).InnerText = _orderValue.ToString();
			(doc.SelectSingleNode("//TIMEPERIOD")).InnerText = _timePeriod.ToString();
			(doc.SelectSingleNode("//TAXAMOUNT")).InnerText = _taxAmount.ToString();
			(doc.SelectSingleNode("//TAXRATE")).InnerText = _taxRate.ToString();
			if(_currentWarranty!=null)
				(doc.SelectSingleNode("//CURRENTWARRANTY")).AppendChild(_currentWarranty);
			(doc.SelectSingleNode("//ELAPSEDMONTHS")).InnerText = _elapsedMonths.ToString();
			(doc.SelectSingleNode("//DESCRIPTION")).InnerText = _description;
			(doc.SelectSingleNode("//CONTRACTNO")).InnerText = _contractno;
            (doc.SelectSingleNode("//STOCKLOCN")).InnerText = _stockLocn.ToString();
            (doc.SelectSingleNode("//NOTIFY")).InnerText = _notify.ToString();
            (doc.SelectSingleNode("//ITEMID")).InnerText = ItemId.ToString();
            (doc.SelectSingleNode("//WARRANTYID")).InnerText = WarrantyID.ToString();           //IP/NM - 18/05/11 -CR1212 - #3627
            (doc.SelectSingleNode("//RETURNITEMID")).InnerText = ReturnItemId.ToString();
            return doc.DocumentElement;
		}
		#endregion

		#region Constructors
		public InstantReplacementDetails( string itemNo,
			string warrantyNo,
			int agreementNo, 
			int newBuffNo,
			string returnReason,
			string returnItemNo,
			short returnStockLocn,
			decimal price,
			decimal quantity,
			decimal orderValue,
			//OneForOneTimePeriod timePeriod,
			decimal taxAmount,
			decimal taxRate,
			string contractno,
            short stockLocn,
            int itemId,
            int warrantyID,         //IP/NM - 18/05/11 -CR1212 - #3627
            int returnItemId)
		{
			_itemNo = itemNo;
			_warrantyNo = warrantyNo;
			_agreementNo = agreementNo;
			_newBuffNo = newBuffNo;
			_returnReason = returnReason;
			_returnItemNo = returnItemNo;
			_returnStockLocn = returnStockLocn;
			_price = price;
			_quantity = quantity;
			_orderValue = orderValue;
			//_timePeriod = timePeriod;
			_taxAmount = taxAmount;
			_taxRate = taxRate;
			_contractno = contractno;
            _stockLocn = stockLocn;
            ItemId = itemId;
            WarrantyID = warrantyID;       //IP/NM - 18/05/11 -CR1212 - #3627
            ReturnItemId = returnItemId;
		}

		public InstantReplacementDetails()
		{
		}
		#endregion
	}
}

