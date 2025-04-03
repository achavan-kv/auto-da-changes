using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// This is the base class for CreditBureau requests and 
	/// responses 
	/// </summary>
	public class CreditBureauMessage
	{
		protected ConsumerCollection _consumers;
		public ConsumerCollection Consumers
		{
			get{return _consumers;}
			set{_consumers = value;}
		}
 
		protected string _accounttype = "";
		/// <summary>
		/// Product Type - must be valid value on table 'Product Type' INDPRDTYP
		/// </summary>
		public string ACCOUNT_TYPE
		{
			get{return _accounttype;}
			set{_accounttype = value;}
		}

		protected string _guarantor = "";
		/// <summary>
		/// Guarantor flag - must be valid value on table 'Guarantor' INDGARCOD
		/// </summary>
		public string GUARANTOR
		{
			get{return _guarantor;}
			set{_guarantor = value;}
		}
 
		protected string _service = "";
		/// <summary>
		/// Service code - must be a valid CreditBureauService value
		/// </summary>
		public string SERVICE
		{
			get{return _service;}
			set{_service = value;}
		}

		protected string _action = "";
		/// <summary>
		/// Action to be performed
		/// </summary>
		public string ACTION
		{
			get{return _action;}
			set{_action = value;}
		}

		protected string _enquirytype = "";
		/// <summary>
		/// Enquiry type - must be valid value on table 'Enquiry Type' INDENQTYP
		/// </summary>
		public string IPIENQTYP
		{
			get{return _enquirytype;}
			set{_enquirytype = value;}
		}

		protected int _numenquiries = 0;
		/// <summary>
		/// Number of consumers in this enquiry
		/// </summary>
		public int NUM_ENQUIRIES
		{
			get{return _numenquiries;}
			set{_numenquiries = value;}
		}
	
		protected string _clientref = "";
		/// <summary>
		/// Client reference - unique client reference, in this case the acctno
		/// </summary>
		public string CLIENT_REFERENCE
		{
			get{return _clientref;}
			set{_clientref = value;}
		}

		protected decimal _consumeramount = 0;
		/// <summary>
		/// agreement total for HP account
		/// </summary>
		public decimal CONSUMER_AMOUNT
		{
			get{return _consumeramount;}
			set{_consumeramount = value;}
		}

		protected string _validatingSchema = "";
		protected string _validationErrors = "";

		/// <summary>
		/// Gets or sets the xml schema which must be used to validate 
		/// this particular CreditBureau message.
		/// </summary>
		public string ValidatingSchema 
		{
			get{return _validatingSchema;}
			set{_validatingSchema = value;}
		}

		/// <summary>
		/// Gets a string containing all errors encountered
		/// validating the message agains the xsd.
		/// </summary>
		public string ValidationErrors
		{
			get{return _validationErrors;}
		}

		protected MemoryStream messageStream = null;
		protected XmlDocument messageXml = null;

		/// <summary>
		/// Takes the Xml document stored in the messageStream and 
		/// validates it against the schema described in ValidatingSchema
		/// </summary>
		public void Validate(XmlDocument xml)
		{
			/* convert the document to a memory stream */
			messageStream = new MemoryStream();
			xml.Save(messageStream);
			
			_validationErrors = "";
			messageStream.Position = 0;
			XmlTextReader tr = new XmlTextReader(messageStream);
			XmlValidatingReader vr = new XmlValidatingReader(tr);
			vr.ValidationType = ValidationType.Schema;
			vr.ValidationEventHandler += new ValidationEventHandler (ValidationHandler);

			XmlSchemaCollection xsc = new XmlSchemaCollection();
			xsc.Add("", _validatingSchema);  
			vr.Schemas.Add(xsc);

			while(vr.Read()) {}
		}

		public void ValidationHandler(object sender, ValidationEventArgs args)
		{
			string err = String.Format("Schema validation error:\n\tSeverity:{0}\n\tMessage:{1}\n\n", args.Severity, args.Message);
			_validationErrors += err;
		}

		protected XmlNode CreateNode(XmlDocument doc, string nodeName, string nodeValue)
		{
			XmlNode n = doc.CreateElement(nodeName);
			n.InnerText = nodeValue;
			return n;
		}

		public CreditBureauMessage()
		{
			_consumers = new ConsumerCollection();
		}
	}
}
