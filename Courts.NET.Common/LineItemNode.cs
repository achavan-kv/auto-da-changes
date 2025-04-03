using System;
using System.Xml;
using STL.Common.Constants.Tags;
using System.Collections;

namespace STL.Common
{
	public class LineItemNode
	{
		private XmlNode _node = null;
		private XmlDocument _doc = null;
		private XmlAttribute _attribute = null;
		private LineItemNodeCollection _related = null;
        private LineItemNodeCollection _servicerequests = null;

		public XmlDocument Document 
		{
			get{return _doc;}
			set{_doc = value;}
		}

		public XmlNode Node 
		{
			get{return _node;}
			set{_node = value;}
		}

		public LineItemNodeCollection RelatedItems
		{
			get{return _related;}
		}

        public LineItemNodeCollection ServiceRequests
        {
            get { return _servicerequests; }
        }


		public string Key
		{
			get{ return _node.Attributes[Tags.Key].Value;}
			set{ _node.Attributes[Tags.Key].Value = value;}
		}

		public string Type
		{
			get{ return _node.Attributes[Tags.Type].Value;}
			set{ _node.Attributes[Tags.Type].Value = value;}
		}

		public string Code
		{
			get{ return _node.Attributes[Tags.Code].Value;}
			set{ _node.Attributes[Tags.Code].Value = value;}
		}

        public int ItemId
        {
            get { return Convert.ToInt32(_node.Attributes[Tags.ItemId].Value); }
            set { _node.Attributes[Tags.ItemId].Value = value.ToString(); }
        }

        public short Category
        {
            get { return Convert.ToInt16(_node.Attributes[Tags.Category].Value); }
            set { _node.Attributes[Tags.Category].Value = value.ToString(); }
        }

		public short Location
		{
			get{ return Convert.ToInt16(_node.Attributes[Tags.Location].Value);}
			set{ _node.Attributes[Tags.Location].Value = value.ToString();}
		}

		public decimal AvailableStock
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.AvailableStock].Value);}
			set{ _node.Attributes[Tags.AvailableStock].Value = value.ToString();}
		}

		public decimal DamagedStock
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.DamagedStock].Value);}
			set{ _node.Attributes[Tags.DamagedStock].Value = value.ToString();}
		}

		public string Description1
		{
			get{ return _node.Attributes[Tags.Description1].Value;}
			set{ _node.Attributes[Tags.Description1].Value = value;}
		}

		public string Description2
		{
			get{ return _node.Attributes[Tags.Description2].Value;}
			set{ _node.Attributes[Tags.Description2].Value = value;}
		}

		public string SupplierCode
		{
			get{ return _node.Attributes[Tags.SupplierCode].Value;}
			set{ _node.Attributes[Tags.SupplierCode].Value = value;}
		}

		public decimal UnitPrice
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.UnitPrice].Value);}
			set{ _node.Attributes[Tags.UnitPrice].Value = value.ToString();}
		}

		public decimal CashPrice
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.CashPrice].Value);}
			set{ _node.Attributes[Tags.CashPrice].Value = value.ToString();}
		}

        public decimal CostPrice
        {
            get
            {
                return Convert.ToDecimal(_node.Attributes[Tags.CostPrice].Value);
            }
            set
            {
                _node.Attributes[Tags.CostPrice].Value = value.ToString();
            }
        }

		public decimal HPPrice
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.HPPrice].Value);}
			set{ _node.Attributes[Tags.HPPrice].Value = value.ToString();}
		}

		public decimal DutyFreePrice
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.DutyFreePrice].Value);}
			set{ _node.Attributes[Tags.DutyFreePrice].Value = value.ToString();}
		}

		public bool ValueControlled
		{
			get{ return Convert.ToBoolean(_node.Attributes[Tags.ValueControlled].Value);}
			set{ _node.Attributes[Tags.ValueControlled].Value = value.ToString();}
		}

		public decimal Quantity
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.Quantity].Value);}
			set{ _node.Attributes[Tags.Quantity].Value = value.ToString();}
		}

		public decimal Value
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.Value].Value);}
			set{ _node.Attributes[Tags.Value].Value = value.ToString();}
		}

		public string DeliveryDate
		{
			get{ return _node.Attributes[Tags.DeliveryDate].Value;}
			set{ _node.Attributes[Tags.DeliveryDate].Value = value;}
		}

		public string DeliveryTime
		{
			get{ return _node.Attributes[Tags.DeliveryTime].Value;}
			set{ _node.Attributes[Tags.DeliveryTime].Value = value;}
		}

		public short BranchForDeliveryNote
		{
			get{ return Convert.ToInt16(_node.Attributes[Tags.BranchForDeliveryNote].Value);}
			set{ _node.Attributes[Tags.BranchForDeliveryNote].Value = value.ToString();}
		}

		public string ColourTrim
		{
			get{ return _node.Attributes[Tags.ColourTrim].Value;}
			set{ _node.Attributes[Tags.ColourTrim].Value = value;}
		}

		public decimal TaxRate
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.TaxRate].Value);}
			set{ _node.Attributes[Tags.TaxRate].Value = value.ToString();}
		}

		public decimal DeliveredQuantity
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.DeliveredQuantity].Value);}
			set{ _node.Attributes[Tags.DeliveredQuantity].Value = value.ToString();}
		}

		public string PlannedDeliveryDate
		{
			get{ return _node.Attributes[Tags.PlannedDeliveryDate].Value;}
			set{ _node.Attributes[Tags.PlannedDeliveryDate].Value = value;}
		}

		public bool CanAddWarranty
		{
			get{ return Convert.ToBoolean(_node.Attributes[Tags.CanAddWarranty].Value);}
			set{ _node.Attributes[Tags.CanAddWarranty].Value = value.ToString();}
		}

		public string DeliveryAddress
		{
			get{ return _node.Attributes[Tags.DeliveryAddress].Value;}
			set{ _node.Attributes[Tags.DeliveryAddress].Value = value;}
		}

		public string DeliveryArea
		{
			get{ return _node.Attributes[Tags.DeliveryArea].Value;}
			set{ _node.Attributes[Tags.DeliveryArea].Value = value;}
		}

		public string DeliveryProcess
		{
			get{ return _node.Attributes[Tags.DeliveryProcess].Value;}
			set{ _node.Attributes[Tags.DeliveryProcess].Value = value;}
		}

		public string DateDelivered
		{
			get{ return _node.Attributes[Tags.DateDelivered].Value;}
			set{ _node.Attributes[Tags.DateDelivered].Value = value;}
		}

		public string QuantityDiff
		{
			get{ return _node.Attributes[Tags.QuantityDiff].Value;}
			set{ _node.Attributes[Tags.QuantityDiff].Value = value;}
		}

		public decimal ScheduledQuantity
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.ScheduledQuantity].Value);}
			set{ _node.Attributes[Tags.ScheduledQuantity].Value = value.ToString();}
		}

		public decimal TaxAmount
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.TaxAmount].Value);}
			set{ _node.Attributes[Tags.TaxAmount].Value = value.ToString();}
		}

		public string ContractNo
		{
			get{ return _node.Attributes[Tags.ContractNumber].Value;}
			set{ _node.Attributes[Tags.ContractNumber].Value = value;}
		}
        
        public string ParentItemNo
        {
            get { return _node.Attributes[Tags.ParentItemNo].Value; }
            set { _node.Attributes[Tags.ParentItemNo].Value = value; }
        }

        public int ParentItemId
        {
            get { return Convert.ToInt32(_node.Attributes[Tags.ParentItemId].Value); }
            set { _node.Attributes[Tags.ParentItemId].Value = value.ToString(); }
        }

        public bool RepoItem                // RI jec 16/06/11
        {
            get { return Convert.ToBoolean(_node.Attributes[Tags.RepoItem].Value); }
            set { _node.Attributes[Tags.RepoItem].Value = value.ToString(); }
        }

		public string ReturnItemNo
		{
			get{ return _node.Attributes[Tags.ReturnItemNo].Value;}
			set{ _node.Attributes[Tags.ReturnItemNo].Value = value;}
		}

		public short ReturnLocation
		{
			get{ return Convert.ToInt16(_node.Attributes[Tags.ReturnLocation].Value);}
			set{ _node.Attributes[Tags.ReturnLocation].Value = value.ToString();}
		}

		public bool FreeGift
		{
			get{ return Convert.ToBoolean(_node.Attributes[Tags.FreeGift].Value);}
			set{ _node.Attributes[Tags.FreeGift].Value = value.ToString();}
		}

		public string ExpectedReturnDate
		{
			get{ return _node.Attributes[Tags.ExpectedReturnDate].Value;}
			set{ _node.Attributes[Tags.ExpectedReturnDate].Value = value;}
		}

		public decimal QtyOnOrder
		{
			get{ return Convert.ToDecimal(_node.Attributes[Tags.QtyOnOrder].Value);}
			set{ _node.Attributes[Tags.QtyOnOrder].Value = value.ToString();}
		}

		public bool PurchaseOrder
		{
			get{ return Convert.ToBoolean(_node.Attributes[Tags.PurchaseOrder].Value);}
			set{ _node.Attributes[Tags.PurchaseOrder].Value = value.ToString();}
		}

		public short LeadTime
		{
			get{ return Convert.ToInt16(_node.Attributes[Tags.LeadTime].Value);}
			set{ _node.Attributes[Tags.LeadTime].Value = value.ToString();}
		}

		public string Damaged
		{
			get{ return _node.Attributes[Tags.Damaged].Value;}
			set{ _node.Attributes[Tags.Damaged].Value = value;}
		}

		public string Assembly
		{
			get{ return _node.Attributes[Tags.Assembly].Value;}
			set{ _node.Attributes[Tags.Assembly].Value = value;}
		}

		public string ProductCategory
		{
			get{ return _node.Attributes[Tags.ProductCategory].Value;}
			set{ _node.Attributes[Tags.ProductCategory].Value = value;}
		}

        public string Deleted
        {
            get { return _node.Attributes[Tags.Deleted].Value; }
            set { _node.Attributes[Tags.Deleted].Value = value; }
        }

  
        //IP - 28/07/11 - RI - #4415
        public string Class
        {
            get { return _node.Attributes[Tags.Class].Value; }
            set { _node.Attributes[Tags.Class].Value = value.ToString(); }
        }

        //IP - 28/07/11 - RI - #4415
        public string SubClass
        {
            get { return _node.Attributes[Tags.SubClass].Value; }
            set { _node.Attributes[Tags.SubClass].Value = value.ToString(); }
        }


		public void AddRelatedItem(LineItemNode li)
		{
			if(RelatedItems==null)
				_related = new LineItemNodeCollection();

			this.RelatedItems.Add(li);
			this.Node.SelectSingleNode("RelatedItems").AppendChild(li.Node);
		}

        public void AddServiceRequests(LineItemNode li)
        {
            if (ServiceRequests == null)
                _servicerequests = new LineItemNodeCollection();

            this.ServiceRequests.Add(li);
            this.Node.SelectSingleNode("ServiceRequests").AppendChild(li.Node);
        }

        public string PurchaseOrderNumber
        {
            get { return _node.Attributes[Tags.PurchaseOrderNumber].Value; }
            set { _node.Attributes[Tags.PurchaseOrderNumber].Value = value; }
        }

        public bool ReplacementItem
        {
            get { return Convert.ToBoolean(_node.Attributes[Tags.ReplacementItem].Value); }
            set { _node.Attributes[Tags.ReplacementItem].Value = value.ToString(); }
        }

        public bool SPIFFItem
        {
            get { return Convert.ToBoolean(_node.Attributes[Tags.SPIFFItem].Value); }
            set { _node.Attributes[Tags.SPIFFItem].Value = value.ToString(); }
        }

        public bool IsInsurance
        {
            get { return Convert.ToBoolean(_node.Attributes[Tags.IsInsurance].Value); }
            set { _node.Attributes[Tags.IsInsurance].Value = value.ToString(); }
        }

        //IP - 29/01/10 - LW 72136
        public string RefCode
        {
            get { return Convert.ToString(_node.Attributes[Tags.RefCode].Value); }
            set {_node.Attributes[Tags.RefCode].Value = value.ToString();}
        }

        //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
        public string VanNo
        {
            get { return _node.Attributes["VanNo"].Value; }
            set { _node.Attributes["VanNo"].Value = value; }
        }

        public DateTime DhlInterfaceDate
        {
            get { return Convert.ToDateTime(_node.Attributes["DhlInterfaceDate"].Value); }
            set { _node.Attributes["DhlInterfaceDate"].Value = value.ToString(); }
        }


        public DateTime DhlPickingDate
        {
            get { return Convert.ToDateTime(_node.Attributes["DhlPickingDate"].Value); }
            set { _node.Attributes["DhlPickingDate"].Value = value.ToString(); }
        }

        public string DhlDNNo
        {
            get { return _node.Attributes["DhlDNNo"].Value; }
            set { _node.Attributes["DhlDNNo"].Value = value; }
        }

        ////IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
        //public string OrigQty
        //{
        //    get { return _node.Attributes["OrigQty"].Value; }
        //    set { _node.Attributes["OrigQty"].Value = value; }
        //}

        public string ShipQty
        {
            get { return _node.Attributes["ShipQty"].Value; }
            set { _node.Attributes["ShipQty"].Value = value; }
        }

        public string SortOrder
        {
            get { return _node.Attributes["SortOrder"].Value; }
            set { _node.Attributes["SortOrder"].Value = value; }
        }
        //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
        public bool ItemRejected
        {
            get { return Convert.ToBoolean(_node.Attributes["ItemRejected"].Value); }
            set { _node.Attributes["ItemRejected"].Value = value.ToString(); }
        }

        public string ModelNumber
        {
            get { return _node.Attributes[Tags.ModelNumber].Value; }
            set { _node.Attributes[Tags.ModelNumber].Value = value; }
        }

        //IP - 23/05/11 - CR1212 - RI - #3651
        public short? SalesBrnNo
        {
            get { return Convert.ToString(_node.Attributes[Tags.SalesBrnNo].Value) == string.Empty? (Int16?)null: Convert.ToInt16(_node.Attributes[Tags.SalesBrnNo].Value); }
            set { _node.Attributes[Tags.SalesBrnNo].Value = value.ToString(); }
        }

        //IP - 19/09/11 - RI - #8218 - CR8201
        public string Brand
        {
            get { return _node.Attributes[Tags.Brand].Value; }
            set { _node.Attributes[Tags.Brand].Value = value; }
        }

        //IP - 20/09/11 - RI - #8218 - CR8201
        public string Style
        {
            get { return _node.Attributes[Tags.Style].Value; }
            set { _node.Attributes[Tags.Style].Value = value; }
        }

        //IP - 06/06/12 - #10229 - Warehouse & Deliveries
        public string Express
        {
            get { return _node.Attributes[Tags.Express].Value; }
            set { _node.Attributes[Tags.Express].Value = value; }
        }

        //#13716 - CR12949
        public int LineItemId
        {
            get{return Convert.ToInt32(_node.Attributes[Tags.LineItemId].Value);}
            set{_node.Attributes[Tags.LineItemId].Value = value.ToString();}
        }

        //#13716 - CR12949
        public bool ReadyAssist
        {
            get{return Convert.ToBoolean(_node.Attributes[Tags.ReadyAssist].Value);}
            set{_node.Attributes[Tags.ReadyAssist].Value = value.ToString();}
        }

        //#15888
        public string WarrantyType
        {
            get { return Convert.ToString(_node.Attributes[Tags.WarrantyType].Value); }
            set { _node.Attributes[Tags.WarrantyType].Value = value.ToString(); }
        }
        
        public decimal AdditionalTaxRate
        {
            get { return Convert.ToDecimal(_node.Attributes[Tags.AdditionalTaxRates].Value); }
            set { _node.Attributes[Tags.AdditionalTaxRates].Value = value.ToString(); }
        }


       
        public bool IsAmortized
        {
            get { return Convert.ToBoolean(_node.Attributes[Tags.IsAmortized].Value); }
            set { _node.Attributes[Tags.IsAmortized].Value = value.ToString(); }
        }

        public LineItemNode()
		{

		}

		public LineItemNode(XmlDocument doc)
		{
			_doc = doc;
			_node = _doc.CreateElement(Tags.Item);

			_attribute = _doc.CreateAttribute(Tags.Key);
			_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ModelNumber);
            _node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Type);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Code);
			_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ItemId);
            _node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Location);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.AvailableStock);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DamagedStock);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Description1);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Description2);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.SupplierCode);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.UnitPrice);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.CashPrice);
			_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.CostPrice);
            _node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.HPPrice);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DutyFreePrice);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ValueControlled);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Quantity);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Value);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveryDate);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveryTime);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.BranchForDeliveryNote);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ColourTrim);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.TaxRate);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveredQuantity);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.PlannedDeliveryDate);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.CanAddWarranty);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveryAddress);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveryArea);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DeliveryProcess);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.DateDelivered);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.QuantityDiff);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ScheduledQuantity);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.TaxAmount);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ContractNumber);
			_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ParentItemNo);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ParentItemId);
            _node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ReturnItemNo);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ReturnLocation);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.FreeGift);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.ExpectedReturnDate);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Key);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.QtyOnOrder);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.PurchaseOrder);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.LeadTime);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Damaged);
			_node.Attributes.Append(_attribute);

			_attribute = _doc.CreateAttribute(Tags.Assembly);
			_node.Attributes.Append(_attribute);
			
			_attribute = _doc.CreateAttribute(Tags.ProductCategory);
			_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Deleted);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.PurchaseOrderNumber);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ReplacementItem);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.SPIFFItem);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.SortOrder);
            _node.Attributes.Append(_attribute);


            _attribute = _doc.CreateAttribute(Tags.IsInsurance); //NM - CR1005
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.RefCode);
            _node.Attributes.Append(_attribute); //IP - 29/01/10 - LW 72136

            //IP/JC - 01/03/10 - CR1072 - Malaysia 3PL 
            // cr Malayisa 3rd party deliveries, VanNo required so we know if item schedules with 3rd party
            _attribute = _doc.CreateAttribute("VanNo");
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute("DhlInterfaceDate");
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute("DhlPickingDate");
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute("DhlDNNo");
            _node.Attributes.Append(_attribute);

            //_attribute = _doc.CreateAttribute("OrigQty");
            //_node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute("ShipQty");
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute("ItemRejected");              //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Category);
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.SalesBrnNo);             //IP - 23/05/11 - CR1212 - RI - #3651
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.RepoItem);              // RI jec 16/06/11
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Class);                //IP - 28/07/11 - RI - #4415
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.SubClass);              //IP - 28/07/11 - RI - #4415
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Brand);                //IP - 19/09/11 - RI - #8218 - CR8201
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Style);                //IP - 20/09/11 - RI - #8218 - CR8201
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.Express);              //IP - 06/06/12 -#10229 - Warehouse & Deliveries
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.LineItemId);           //#13716 - CR12949
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.ReadyAssist);          //#13716 - CR12949
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.WarrantyType);         //#17883  //#15888
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.AdditionalTaxRates);         //BCX : This is used for LUX tax for curacao 
            _node.Attributes.Append(_attribute);

            _attribute = _doc.CreateAttribute(Tags.IsAmortized);         
            _node.Attributes.Append(_attribute);

            XmlNode related = _doc.CreateElement("RelatedItems");
			_node.AppendChild(related);
		}
	}

	public class LineItemDocument
	{
		private XmlDocument _doc = null;

		public bool HasLineItems 
		{
			get
			{
				bool x = false;

				if(_doc!=null)
				{
					x = _doc.SelectSingleNode("Items").ChildNodes.Count>0;
				}
				return x;
			}
		}

		public XmlDocument Document 
		{
			get{ return _doc;}
		}

		public LineItemDocument()
		{
			_doc = new XmlDocument();
			_doc.LoadXml("<Items/>");
		}

		public LineItemNode CreateLineItemNode()
		{
			if(_doc==null)
			{
				_doc = new XmlDocument();
				_doc.LoadXml("<Items/>");
			}
			LineItemNode li = new LineItemNode(_doc);
			_doc.SelectSingleNode("Items").AppendChild(li.Node);

			return li;
		}

		public void AddLineItemNode(LineItemNode li)
		{
			if(_doc==null)
			{
				_doc = new XmlDocument();
				_doc.LoadXml("<Items/>");
			}
			li.Node = _doc.ImportNode(li.Node, true);
			_doc.SelectSingleNode("Items").AppendChild(li.Node);
		}

		public void AddLineItemNode(XmlNode li)
		{
			if(_doc==null)
			{
				_doc = new XmlDocument();
				_doc.LoadXml("<Items/>");
			}
			li = _doc.ImportNode(li, true);
			_doc.SelectSingleNode("Items").AppendChild(li);
		}


		public LineItemNode FindNode(string key)
		{
			string xPath = "//Item[@Key = '"+ key +"']";
			LineItemNode li = new LineItemNode();
			li.Node = _doc.DocumentElement.SelectSingleNode(xPath);
			return li;
		}
	}

	public class LineItemNodeCollection : CollectionBase
	{
		public LineItemNodeCollection()
		{
		}
		public void Add(LineItemNode lineItem)
		{
			List.Add(lineItem);
		}
		public void Remove(int index)
		{
			if (index <= Count - 1 && index < 0)
				List.RemoveAt(index); 
		}
		public LineItemNode Item(int index)
		{
			return (LineItemNode) List[index];
		}
	}
}
