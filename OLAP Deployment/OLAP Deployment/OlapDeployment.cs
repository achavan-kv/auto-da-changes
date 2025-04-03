using System;
using System.Xml;
using Microsoft.AnalysisServices;

namespace OLAP_Deployment
{
    public class OlapDeployment
    {
        public  event EventHandler<Progress> Progress;

        public string DatabaseDefinitionFile { get; set; }
        public string OlapServerConnection { get; set; }
        public string SourceDataMart { get; set; }
        public bool ProcessDb { get; set; }

        public OlapDeployment()
        {
            DatabaseDefinitionFile = @"\\GRID0\Build\$COSACS_Current\Files\Olap\CosacsReporting.asdatabase";
            OlapServerConnection = @"Provider=MSOLAP; Data Source=.;Initial Catalog=CosacsReporting";
            SourceDataMart = @"Provider=SQLNCLI11.1;Data Source=.;Integrated Security=SSPI;Initial Catalog=cosacs";
            ProcessDb = true;
        }

        public void Deploy()
        {
            var validationMsg = Validate();

            if (!string.IsNullOrEmpty(validationMsg.Trim()))
            {
                FireError(validationMsg);
                return;
            }

            bool hadError = false;

            string olapServerName = "";
            string olapDatabaseName = "";
            //string asDatabaseDefinitionFile = @"D:\Test\CosacsReporting.asdatabase";
            // string sourceDataMart = @"Provider=SQLNCLI11.1;Data Source=.;Integrated Security=SSPI;Initial Catalog=cosacs";

            #region Obtain OLAP server and target database name from AS connection string

            //string OLAPServerConnection = @"Provider=MSOLAP; Data Source=.;Initial Catalog=CosacsReporting";// Dts.Connections["TargetOlapServerCS"].ConnectionString;

            foreach (string connectionBit in OlapServerConnection.Split(';'))
            {

                if (connectionBit.Split('=')[0].Trim() == "Data Source")
                {
                    olapServerName = connectionBit.Split('=')[1].Trim();
                }
                if (connectionBit.Split('=')[0].Trim() == "Initial Catalog")
                {
                    olapDatabaseName = connectionBit.Split('=')[1].Trim();
                }
            }

            FireInformation(string.Format("Attempting to deploy '{0}' to server '{1}'", olapDatabaseName, olapServerName));

            #endregion

            using (Server svr = new Server())
            {
                // connect to the OLAP server
                svr.Connect(olapServerName);
                if (svr.Connected)
                {
                    FireInformation(string.Format("Connected to server {0}", olapServerName));

                    // Drop the OLAP database if it already exists
                    Database db = svr.Databases.FindByName(olapDatabaseName);
                    if (db != null)
                    {
                        FireInformation(string.Format("Dropping existing database {0} from {1}", olapDatabaseName, olapServerName));
                        db.Drop();
                    }

                    // Start generating the main part of the XMLA command
                    var xmlaCommand = new XmlDocument();
                    xmlaCommand.LoadXml(string.Format("<Batch Transaction='false' xmlns='http://schemas.microsoft.com/analysisservices/2003/engine'><Alter AllowCreate='true' ObjectExpansion='ExpandFull'><Object><DatabaseID>{0}</DatabaseID></Object><ObjectDefinition/></Alter></Batch>", olapDatabaseName));

                    // To work with the XML and XPath we need an XmlNamespaceManager 
                    var nt = new NameTable();
                    var nsManager = new XmlNamespaceManager(xmlaCommand.NameTable);
                    nsManager.AddNamespace("ns1", "http://schemas.microsoft.com/analysisservices/2003/engine");
                    nsManager.AddNamespace("ns2", "http://schemas.microsoft.com/analysisservices/2010/engine/200");

                    // Find the ObjectDefinition node and add the cube definition file as a child of this node
                    XmlNode oaRootNode = xmlaCommand.SelectSingleNode("//ns1:ObjectDefinition", nsManager);
                    if (oaRootNode != null)
                    {
                        // load OLAP Database definition from the .asdatabase file identified by the AsDatabaseDefinitionFile file connection
                        var olapCubeDef = new XmlDocument();
                        olapCubeDef.Load(DatabaseDefinitionFile);
                        // merge the two XML files by obtain a reference to the ObjectDefinition node 
                        oaRootNode.InnerXml = olapCubeDef.InnerXml;
                    }

                    // now remove all the nodes that cause errors when we deploy
                    RemoveAllReadOnlyNodes(xmlaCommand.DocumentElement, nsManager);

                    // change the name of the OLAP database in the two nodes (ID and Name)
                    XmlNode databaseId = xmlaCommand.SelectSingleNode("//ns1:Database/ns1:ID", nsManager);
                    if (databaseId != null)
                        databaseId.InnerText = olapDatabaseName;
                    XmlNode databaseName = xmlaCommand.SelectSingleNode("//ns1:Database/ns1:Name", nsManager);
                    if (databaseName != null)
                        databaseName.InnerText = olapDatabaseName;

                    // change the connection string to the relational database
                    XmlNode connectionStringNode = xmlaCommand.SelectSingleNode("//ns1:DataSources/ns1:DataSource/ns1:ConnectionString", nsManager);
                    if (connectionStringNode != null)
                    {
                        connectionStringNode.InnerText = SourceDataMart;
                    }

                    // change the impersonation mode of connection string to use the service account
                    XmlNode impersonationNode = xmlaCommand.SelectSingleNode("//ns1:DataSources/ns1:DataSource/ns1:ImpersonationInfo/ns1:ImpersonationMode", nsManager);
                    if (impersonationNode != null)
                    {
                        impersonationNode.InnerText = "ImpersonateServiceAccount";
                    }
                    // now deploy the new cube (structure only)
                    XmlaResultCollection oResults = svr.Execute(xmlaCommand.InnerXml);

                    // check for errors during deployment
                    foreach (XmlaResult oResult in oResults)
                    {
                        foreach (XmlaMessage oMessage in oResult.Messages)
                        {
                            if ((oMessage.GetType().Name == "XmlaError"))
                            {
                                FireError(oMessage.Description);
                                hadError = true;
                            }
                        }
                    }

                    // if you want to process the cube after deployment, uncomment the following lines
                    if (ProcessDb)
                    {
                        db = svr.Databases.FindByName(olapDatabaseName);
                        if (db != null)
                        {
                            FireInformation(string.Format("Processing database {0} on server {1}", olapDatabaseName, olapServerName));
                            db.Process(ProcessType.ProcessFull);
                        }
                    }

                    FireInformation(hadError
                        ? string.Format("Failed to deploy database '{0}' to server '{1}'", olapDatabaseName,
                            olapServerName)
                        : string.Format("Succeeded in deploying database '{0}' to server '{1}'", olapDatabaseName,
                            olapServerName));
                }
            }

        }

        private string Validate()
        {
            var ret = "";

            ret += ValidateString(DatabaseDefinitionFile, "Database Definition File");
            ret += ValidateString(OlapServerConnection, "Olap Server Connection String");
            ret += ValidateString(SourceDataMart, "Source Server Connection String");

            return ret;
        }

        private string ValidateString(string input, string label)
        {
            var ret = "";

            if (string.IsNullOrEmpty(input.Trim()))
            {
                ret = string.Format("{0} field cannot be empty.", label);
            }

            return ret;
        }

        static void RemoveAllReadOnlyNodes(XmlNode rootNode, XmlNamespaceManager nsManager)
        {
            #region Remove all the nodes that cause errors when we deploy
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:CreatedTimestamp", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:LastSchemaUpdate", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:LastUpdate", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:LastProcessed", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:State", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:ImpersonationInfoSecurity", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:CurrentStorageMode", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns2:ProcessingState", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            foreach (XmlNode node in rootNode.SelectNodes("//ns1:ConnectionStringSecurity", nsManager))
            {
                node.ParentNode.RemoveChild(node);
            }
            #endregion
        }

        private void FireInformation(string message)
        {
            OnProgress(new Progress("INFO: " + message));
        }

        private void FireError(string message)
        {
            OnProgress(new Progress("ERROR: " + message));
        }

        private void OnProgress(Progress e)
        {
            if (Progress != null)
                Progress(this, e);
        }
    }

    public class Progress : EventArgs
    {
        public string Status { get; private set; }

        private Progress() { }

        public Progress(string status)
        {
            Status = status;
        }
    }
}
