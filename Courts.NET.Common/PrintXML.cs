using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Configuration;
using System.Net;
using System.Threading;

namespace STL.Common
{
    /// <summary>
    /// This is an abstract base class for deriving the XML classes
    /// to use for printing various documents. It implements common 
    /// functionality so that nodes can be retrieved and edited and
    /// transformations can be applied.
    /// </summary>
    public abstract class PrintXML : CommonObject
    {
        protected XslTransform _xslTrans = null;

        private string _country = "";
        public string CountryName
        {
            get { return _country; }
            set { _country = value; }
        }

        protected XmlDocument _doc;

        public string Xml
        {
            get { return _doc.OuterXml; }
        }

        public XmlDocument Document
        {
            get { return _doc; }
            set { _doc = value; }
        }

        protected string _xsltPath = string.Empty; // "http://localhost/Courts.NET.WS/Stylesheets/";
        public string XsltPath
        {
            get
            {
                if (String.IsNullOrEmpty(_xsltPath))
                {
                    _xsltPath = System.Web.HttpContext.Current.Server.MapPath("stylesheets");
                    //System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
                    //_xsltPath = String.Format("http://{0}/{1}stylesheets/", request.Url.Host, request.Url.Segments[1]);
                }
                return _xsltPath;
            }
            set { _xsltPath = value; }
        }

        protected string _xml = "";
        public string XmlTemplate
        {
            get { return _xml; }
            set { _xml = value; }
        }

        public PrintXML()
        {
            _doc = new XmlDocument();
            if (_xml != "")
                _doc.LoadXml(_xml);
        }

        public XmlNode CreateNode(string nodeName, object nodeValue)
        {
            XmlNode n = _doc.CreateElement(nodeName);
            n.InnerText = nodeValue.ToString();
            return n;
        }

        public void SetNode(string XPath, string Value)
        {
            XmlNode node = _doc.SelectSingleNode(XPath);
            if (node != null)
                node.InnerText = Value;
        }

        public void SetNode(string XPath, string Value, bool overWrite)
        {
            XmlNode node = _doc.SelectSingleNode(XPath);
            if (node != null)
                if (overWrite || node.InnerText.Length == 0)
                    node.InnerText = Value;
        }

        public string GetNodeValue(string XPath)
        {
            string Value = "";
            XmlNode node = _doc.SelectSingleNode(XPath);
            if (node != null)
                Value = node.InnerText;
            return Value;
        }

        public XmlNode GetNode(string XPath)
        {
            return _doc.SelectSingleNode(XPath);
        }

        /// <summary>
        /// This method will return a string writer containing the 
        /// transformed xml document. This will be an HTML stream 
        /// and can be sent back to the client using response.Write
        /// </summary>
        /// <returns></returns>
        public StringWriter Transform()
        {
            XslTransform _xslTrans = new XslTransform();
            StringWriter sw = null;

            _xslTrans.Load(XsltPath); //_xsltPath);
            sw = new StringWriter();
            _xslTrans.Transform(_doc, null, sw);
            return sw;
        }

        public StringWriter TransformXml()
        {
            XslCompiledTransform _xslTrans = new XslCompiledTransform();
            XPathDocument xPathDoc = new XPathDocument(new StringReader(_doc.OuterXml));

            //XmlTextWriter w = new XmlTextWriter("DeliveryNote.xslt", null);
            _xslTrans.Load(XsltPath); //_xsltPath);
            StringWriter sw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(sw);
            w.Formatting = Formatting.Indented;

            _xslTrans.Transform(xPathDoc, null, w);
            w.Flush();

            return sw;
        }

        public void Load()
        {
            if (_xml.Length != 0)
                _doc.LoadXml(_xml);
        }

        public void Load(string xml)
        {
            _doc.LoadXml(xml);
        }

        public void SetXsltPath(string fileName)
        {
            var filePath = Path.Combine(Path.Combine(XsltPath, _country), fileName);
            if (File.Exists(filePath))
                XsltPath = filePath;
            else
            {
                filePath = Path.Combine(XsltPath, fileName);
                if (File.Exists(filePath))
                    XsltPath = filePath;
                else
                    throw new IOException(String.Format("Stylesheet : {0} not found", fileName));

            }

            /////this is a bit of a hack I think but I can't find a better 
            /////way to check for the existence of a url
            //WebRequest wreq = WebRequest.Create(XsltPath + _country + "/" + fileName);
            //try
            //{
            //    WebResponse wresp = wreq.GetResponse();
            //    XsltPath += _country + "/" + fileName;
            //}
            //catch (WebException)
            //{
            //    XsltPath += fileName;
            //}
        }

    }
}
