using System;
using System.Xml;
using STL.Common;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for ConsumerEnquiryRequest.
	/// </summary>
	public class ConsumerEnquiryRequest : CreditBureauMessage, ICreditBureauRequest
	{
		public ConsumerEnquiryRequest() 
		{
			ValidatingSchema = "http://localhost/Courts.NET.WS/schemas/enquiry.xsd";
		}

		/// <summary>
		/// Formats a subset of inherited properties into a ConsumerEnquiryRequest
		/// Xml document, calls the base class Validate method to validate the
		/// document against the Bayscorp schema. 
		/// </summary>
		/// <returns>Validated ConsumerEnquiryRequest XmlDocument</returns>
		public XmlDocument Serialise()
		{ 
			/* create the overall request node and add header information */
			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement(CreditBureauElements.REQUEST));
			XmlNode request = doc.DocumentElement;
			request.AppendChild(CreateNode(doc, CreditBureauElements.SERVICE, CreditBureauService.ConsumerEnquiryRequest));
			request.AppendChild(CreateNode(doc, CreditBureauElements.ACTION, CreditBureauAction.ConsumerEnquiryRequest));

			/* create the message node */
			XmlNode messageNode = CreateNode(doc, CreditBureauElements.MESSAGE, "");
			request.AppendChild(messageNode);

			/* create the data node */
			XmlNode dataNode = CreateNode(doc, CreditBureauElements.DATA, "");
			messageNode.AppendChild(dataNode);
            dataNode.AppendChild(CreateNode(doc, CreditBureauElements.IPIENQTYP, this.IPIENQTYP));
			dataNode.AppendChild(CreateNode(doc, CreditBureauElements.ACCOUNT_TYPE, this.ACCOUNT_TYPE));
			dataNode.AppendChild(CreateNode(doc, CreditBureauElements.CLIENT_REFERENCE, this.CLIENT_REFERENCE));			/* tag mandatory but data optional */
			dataNode.AppendChild(CreateNode(doc, CreditBureauElements.CONSUMER_AMOUNT, this.CONSUMER_AMOUNT.ToString("F2")));	/* tag mandatory but data optional */
			dataNode.AppendChild(CreateNode(doc, CreditBureauElements.GUARANTOR, this.GUARANTOR));
			dataNode.AppendChild(CreateNode(doc, CreditBureauElements.NUM_ENQUIRIES, this.NUM_ENQUIRIES.ToString()));

			/* create a consumer node for each consumer in the request's consumer collection */
			foreach (Consumer consumer in Consumers)
			{
				XmlNode consumerNode = CreateNode(doc, CreditBureauElements.CONSUMER, "");
				dataNode.AppendChild(consumerNode);
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICIDTYP, consumer.IICIDTYP));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICIDCOD, consumer.IICIDCOD));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICNAM1, consumer.IICNAM1));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICDOB, consumer.IICDOB.ToString("dd/MM/yyyy")));	
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICGENCOD, consumer.IICGENCOD));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IICNATCOD, consumer.IICNATCOD));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IINNUMTYP, consumer.IINNUMTYP));
				consumerNode.AppendChild(CreateNode(doc, CreditBureauElements.IINNUM, consumer.IINNUM));

				if(consumer.Addresses != null)
				{
					foreach (Address addr in consumer.Addresses)
					{
						/* create the single address node for the consumer */
						XmlNode addressNode = CreateNode(doc, CreditBureauElements.ADDRESS, "");
						consumerNode.AppendChild(addressNode);					
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIASTRNUMF, addr.IIASTRNUMF));	
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIASTR, addr.IIASTR));
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIAADRTYP, addr.IIAADRTYP));
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIAADRLN2, addr.IIAADRLN2));
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIACOUCOD, addr.IIACOUCOD));
						addressNode.AppendChild(CreateNode(doc, CreditBureauElements.IIAPOSCOD, addr.IIAPOSCOD));
					}
				}
			}
#if(DEBUG)
			CommonObject co = new CommonObject();
			co.logMessage(doc.OuterXml, "99999", System.Diagnostics.EventLogEntryType.Information);
#endif
		
			/* Validate against the schema */
			// 28/9/07  rdb i have turned validation off as house numbers are sometimes strings
			// i propose we see how this goes
			//Validate(doc);
			if(ValidationErrors.Length > 0)
			{
				string error = "Credit Bureau Request failed validation: " + Environment.NewLine + Environment.NewLine;
				throw new CreditBureauException(error+ValidationErrors);
			}

			return doc;
		}		
	}
}
