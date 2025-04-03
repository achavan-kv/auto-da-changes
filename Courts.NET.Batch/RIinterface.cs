using System;
using STL.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using STL.DAL;
using STL.Common.Constants.EOD;
using System.Diagnostics;
using System.Data.SqlClient;
using STL.BLL;
using STL.Common.Constants.RItableNames;
using System.Threading;

namespace STL.Batch
{
    public class RIinterface : CommonObject
    {
        private string filename;
        private string path;
        private string tablename;
        private bool repo;
        private string sysDriveDirectory;
        private string progress = "";

        public RIinterface() { }

        public string CreateCos2RIExport(int runNo, string interfaceName, bool reRun)
        {
            var eodResult = EODResult.Pass;
            
            try
            {
                eodResult = CheckDestinationDirectories(interfaceName, runNo);       // check Directory path is valid

                if (eodResult != EODResult.Pass)
                    return eodResult;

                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        //IP - 16/06/11 - CR1212 - RI - #3961 - Deliver renewal warranties.
                        if (reRun == false)
                        {
                            Progress("Starting collecting warranties on credit not paid for within period...");

                            new DRI().CollectWarrantiesOnCredit();                          //IP - 21/06/11 - CR1212 - RI - #3979

                            Progress("Starting deliver renewal warranties...");

                            new DRI().DeliverWarrantyRenewals();
                        }

                        var MSGQArguments = new List<MSGQArgument>();

                        repo = false;

                        //---- Committed stock -------------------------------------------
                        Progress("Starting Committed Stock ...");

                        new DRI().CreateCos2RICommittedStock(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIQTYpath]);
                        tablename = RI.RICommittedStock;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RIQTYMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------


                        //---- Delivery Transfers ----------------------------------------
                        Progress("Starting Delivery Transfers ...");

                        new DRI().CreateCos2RIDeliveryTransfers(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIDTFpath]);
                        tablename = RI.RIDeliveryTransfers;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument 
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RIDTFMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------

                        //---- Sales & Returns -------------------------------------------
                        Progress("Starting Sales & Returns ...");

                        new DRI().CreateCos2RIDeliveriesReturns(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RISARpath]);
                        tablename = RI.RIDeliveriesReturns;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument 
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RISARMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------

                        repo = true;

                        //---- Committed Stock Repo --------------------------------------
                        Progress("Starting Committed Stock (Repossessed) ...");

                        new DRI().CreateCos2RICommittedStock(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIQTYpathRepo]);
                        tablename = RI.RICommittedStockRepo;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument
                            {
                                Value = Convert.ToString(Country[CountryParameterNames.RIQTYRepoMSGQArgument]),
                                Path = sysDriveDirectory,
                                FileName = filename
                            });
                        //----------------------------------------------------------------

                        //---- Delivery Transfers Repo -----------------------------------
                        Progress("Starting Delivery Transfers (Repossessed) ...");

                        new DRI().CreateCos2RIDeliveryTransfers(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIDTFpathRepo]);
                        tablename = RI.RIDeliveryTransfersRepo;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument 
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RIDTFRepoMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------

                        //---- Sales & Returns Repo --------------------------------------
                        Progress("Starting Sales & Returns (Repossessed) ...");

                        new DRI().CreateCos2RIDeliveriesReturns(runNo, reRun, repo, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RISARpathRepo]);
                        tablename = RI.RIDeliveriesReturnsRepo;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument 
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RISARRepoMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------

                        //---- Repossessions ---------------------------------------------
                        Progress("Starting Repossessions ...");

                        new DRI().CreateCos2RIRepossessions(runNo, reRun, out filename, out path);

                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIRPOpath]);
                        tablename = RI.RIRepossessions;
                        ExportRI(filename);
                        MSGQArguments.Add(
                            new MSGQArgument 
                            {
                                Value    = Convert.ToString(Country[CountryParameterNames.RIRPOMSGQArgument]),
                                Path     = sysDriveDirectory,
                                FileName =  filename
                            });
                        //----------------------------------------------------------------

                        if (new BCountry().IsRIFileTransfer())
                        {
                            foreach (var argument in MSGQArguments)
                            {
                                SendToMQ(interfaceName, argument, runNo);
                            }
                        }

                        trans.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            catch (Exception ex)
            {
                var s = new StringBuilder();

                for (var ex1 = ex; ex1 != null; ex1 = ex1.InnerException)
                    s.AppendFormat("{0} ({1})\n", ex1.Message, ex1.GetType()).AppendLine(ex1.StackTrace);

                Console.WriteLine(s);
            }
            finally
            {
                Progress("Finished CoSACS2RI ");
            }

            return eodResult;
        }

        private void ExportRI(string filename)
        {
            Progress("Exporting " + filename + " ...");

            try
            {
                string server = string.Empty;
                string username = string.Empty;
                string password = string.Empty;
                string dbname = string.Empty;
                string integrated = null;

                //Retrieve the connection string 
                string conn = Connections.Default;
                string[] connParts = conn.Split(';');
                foreach (string connPart in connParts)
                {
                    string[] splitPart = connPart.Split('=');

                    switch (splitPart[0].ToLower().Trim())
                    {
                        case "server":
                        case "data source":
                            server = splitPart[1];
                            break;
                        case "uid":
                        case "user id":
                            username = splitPart[1];
                            break;
                        case "pwd":
                        case "password":
                            password = splitPart[1];
                            break;
                        case "database":
                        case "initial catalog":
                            dbname = splitPart[1];
                            break;
                        case "integrated security":
                            integrated = (splitPart[1] ?? "").ToLower();
                            break;
                    }
                }

                var bcpPath = Convert.ToString(Country[CountryParameterNames.BCPpath]);

                var proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.WorkingDirectory = @bcpPath;
                proc.StartInfo.FileName = "bcp.exe";

                var path = Path.Combine(sysDriveDirectory, filename);
                var security = (integrated == "true" || integrated == "sspi") ? " -T " : " -U" + username + " -P" + password;

                proc.StartInfo.Arguments = dbname + @".dbo." + tablename + " out " + path + " -c -q " + security + " -S" + server;
                proc.StartInfo.UseShellExecute = false;

                proc.Start();
                proc.WaitForExit();
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                var s = new StringBuilder();

                for (var ex1 = ex; ex1 != null; ex1 = ex1.InnerException)
                    s.AppendFormat("{0} ({1})\n", ex1.Message, ex1.GetType()).AppendLine(ex1.StackTrace);

                Console.WriteLine(s);
                logMessage(s.ToString(), "", EventLogEntryType.Error);
                throw ex;
            }
        }

        public string RI2CosacsImport(int runNo, string interfaceName, bool reRun, string fileDate)
        {
            string eodResult = EODResult.Pass;
            try
            {
                eodResult = CheckSourceDirectories(interfaceName, runNo);       // check Directory path is valid

                if (eodResult != EODResult.Pass)
                    return eodResult;

                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        DRI init = new DRI();
                        init.Initialise();          //truncate load tables

                        CheckRIFiles(fileDate);

                        repo = false;

                        //---- ABC File ---- Product Info
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIABCpath]);
                        var fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("ABC" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no 
                            tablename = RI.RItemp_RawProductImport;
                            ImportRI(filename);

                            new DRI().ImportRI2CosProductInfo(conn, trans, runNo, reRun, repo, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "ABC" + fileDate + "*.fte"), runNo);

                        //---- KIT File ----
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIKITpath]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("KIT" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no
                            tablename = RI.RItemp_RawKitload;
                            ImportRI(filename);

                            new DRI().ImportRI2CosKitProductImport(conn, trans, runNo, reRun, interfaceName, repo); //IP - 26/08/11 - #4621 - Added Repo flag
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "KIT" + fileDate + "*.fte"), runNo);

                        //---- PO File ---- Purchase Order
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIPODYpath]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("POD" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no
                            tablename = RI.RItemp_RawPOload;
                            ImportRI(filename);

                            new DRI().ImportRI2CosPODetails(conn, trans, runNo, reRun, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "POD" + fileDate + "*.fte"), runNo);

                        //---- OHQ File ---- On Hand Quantity
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIOHQYpath]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("OHQ" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no
                            tablename = RI.RItemp_RawStkQtyload;
                            ImportRI(filename);

                            new DRI().ImportRI2CosOnHandQty(conn, trans, runNo, reRun, repo, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "OHQ" + fileDate + "*.fte"), runNo);

                        //---- CTX File ---- Product Heirachy Info
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RICTXpath]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("CTX" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no 
                            tablename = RI.RItemp_RawProductHeirarchy;
                            ImportRI(filename);

                            new DRI().ImportProductHeirachy(conn, trans, runNo, reRun, repo, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "CTX" + fileDate + "*.fte"), runNo);


                        repo = true;

                        //---- ABC File ---- Product Info Repo
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIABCpathRepo]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("ABC" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no 
                            tablename = RI.RItemp_RawProductImportRepo;
                            ImportRI(filename);

                            new DRI().ImportRI2CosProductInfo(conn, trans, runNo, reRun, repo, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "ABC" + fileDate + "*.fte"), runNo);

                        //IP - 26/08/11 - #4621 - Process Kit Products for repo

                        //---- KIT File ----
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIKITpathRepo]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("KIT" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no
                            tablename = RI.RItemp_RawKitload;
                            ImportRI(filename);

                            new DRI().ImportRI2CosKitProductImport(conn, trans, runNo, reRun, interfaceName,repo);          //IP - 26/08/11 - #4621 - Added repo flag
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "KIT" + fileDate + "*.fte"), runNo);

                        //---- OHQ File ---- On Hand Quantity Repo
                        sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIOHQYpathRepo]);

                        fileInfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("OHQ" + fileDate + "*.fte", SearchOption.TopDirectoryOnly);
                        if (fileInfo.Any())
                        {
                            filename = fileInfo.Last().Name;       //choose file with highest run no 
                            tablename = RI.RItemp_RawStkQtyloadRepo;
                            ImportRI(filename);

                            new DRI().ImportRI2CosOnHandQty(conn, trans, runNo, reRun, repo, interfaceName);
                        }
                        else
                            ReportDataFileMissing(conn, trans, interfaceName, Path.Combine(sysDriveDirectory, "OHQ" + fileDate + "*.fte"), runNo);

                        
                        trans.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                Progress("Finished RI2CoSACS");
            }

            return eodResult;
        }

        private void ImportRI(string filename)
        {
            Progress("Importing " + filename + " ...");

            try
            {
                string server = string.Empty;
                string username = string.Empty;
                string password = string.Empty;
                string dbname = string.Empty;
                string integrated = null;

                //Retrieve the connection string 
                string conn = Connections.Default;
                string[] connParts = conn.Split(';');
                foreach (string connPart in connParts)
                {
                    string[] splitPart = connPart.Split('=');

                    switch (splitPart[0].ToLower().Trim())
                    {
                        case "server":
                        case "data source":
                            server = splitPart[1];
                            break;
                        case "uid":
                        case "user id":
                            username = splitPart[1];
                            break;
                        case "pwd":
                        case "password":
                            password = splitPart[1];
                            break;
                        case "database":
                        case "initial catalog":
                            dbname = splitPart[1];
                            break;
                        case "integrated security":
                            integrated = splitPart[1];
                            break;
                    }
                }

                var bcpPath = Convert.ToString(Country[CountryParameterNames.BCPpath]);

                var proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.WorkingDirectory = @bcpPath;
                proc.StartInfo.FileName = "bcp.exe";

                var path = Path.Combine(sysDriveDirectory, filename);
                var security = (!string.IsNullOrEmpty(integrated) && integrated.ToLower() == "true") ? " -T " : " -U" + username + " -P" + password;

                proc.StartInfo.Arguments = dbname + @".." + tablename + " in " + path + " -c -q -t," + security + " -S" + server;
                proc.StartInfo.UseShellExecute = false;

                proc.Start();
                proc.WaitForExit();
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        private void SendToMQ(string interfaceName, MSGQArgument argument, int runNo)
        {
            try
            {
                var batScriptPath = Convert.ToString(Country[CountryParameterNames.RIFTEBatchScriptPath]);
                batScriptPath = Path.GetFullPath(batScriptPath);
                
                //Command : FTESender.bat "-D fromPath=C:\\DTF11041555.FTE" "-D toPath=/QIBM/UserData/WMQFTE/V7/RICARIBE/IN/RI21DB/DEL_TRAN/"

                var proc = new System.Diagnostics.Process();

                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(batScriptPath);
                proc.StartInfo.FileName = batScriptPath;
                proc.StartInfo.Arguments = argument.ToString();

                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;

                proc.Start();

                var output = proc.StandardOutput.ReadToEnd();
                var error = proc.StandardError.ReadToEnd();

                Console.WriteLine(output);
                Console.WriteLine(error);

                if (!String.IsNullOrWhiteSpace(error))
                    ReportFTETransferError(interfaceName, argument.FileName, error, runNo);

                proc.WaitForExit();
                proc.Close();
            }
            catch (Exception ex)
            {
                logMessage(ex.ToString(), "", EventLogEntryType.Error);
                ReportFTETransferError(interfaceName, argument.FileName, ex.ToString(), runNo);
                //throw ex;
            }
        }

        private void ReportFTETransferError(string interfaceName, string fileName, string error, int runNo)
        {
            string msg = "Error FTE Transfering file: " + fileName + Environment.NewLine + error;

            new BInterfaceError(null, null, interfaceName, runNo,
                                DateTime.Now, msg, "E");
        }

        private void ReportDataFileMissing(SqlConnection conn, SqlTransaction trans, string interfaceName,
                                            string fileName, int runNo)
        {
            string msg = "Unable to open file " + fileName;

            new BInterfaceError(conn,
                                trans,
                                interfaceName,
                                runNo,
                                DateTime.Now,
                                msg,
                                "W");
        }

        private void ReportDirPathMissing(string interfaceName, string directoryName, int runNo)
        {
            string msg = "Directory path " + directoryName + " does not exist";

            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    new BInterfaceError(conn, trans, interfaceName, runNo, DateTime.Now, msg, "E");
                    trans.Commit();
                }
            }
        }

        private string CheckRIFiles(string filedate)
        {
            string eodResult = EODResult.Pass;
            
            Progress("Checking for RI Import files ...");

            try
            {
                DateTime startTime = DateTime.Now;

                //---- ABC File ----
                sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIABCpath]);
                var fileinfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("ABC" + filedate + "*.fte", SearchOption.TopDirectoryOnly);

                while (fileinfo.Count() == 0)
                {
                    fileinfo = new DirectoryInfo(sysDriveDirectory).EnumerateFiles("ABC" + filedate + "*.fte", SearchOption.TopDirectoryOnly);
                  
                    if (fileinfo.Any())
                        break;
                    else
                    {
                        //waiting 80 minutes and then will exit anyway
                        Thread.Sleep(10000); //sleep for 60 seconds
                        DateTime stopTime = DateTime.Now;
                        TimeSpan duration = stopTime - startTime;
                        if ((duration.Minutes * 60) % 300 == 0 && duration.Seconds <= 10)
                        {
                            Progress("Continuing to check for " + Convert.ToString(80 - duration.Minutes) + " minutes.");
                        }

                        if (duration.Minutes >= 80 && !fileinfo.Any())
                        {
                            throw new STLException("RI Stock system Timeout - Aborting RI interface import as no response after 80 minutes - re-Run interface");
                        }
                    }
                }

                Progress("RI Interface files found");
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);
                //Console.WriteLine("Path : {0}", sysDriveDirectory);
                //Console.WriteLine("file : {0}", filename);
                throw;
            }

            Progress("Processing RI files ...");
            
            return eodResult;
        }

        private string CheckSourceDirectories(string interfaceName, int runNo)
        {
            string eodResult = EODResult.Pass;
            Progress("Checking Source Directories ...");

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIABCpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIABCpathRepo]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIOHQYpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIOHQYpathRepo]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIKITpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIRPOpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            return eodResult;
        }

        private string CheckDestinationDirectories(string interfaceName, int runNo)
        {
            string eodResult = EODResult.Pass;
            Progress("Checking Destination Directories ...");
         
            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RISARpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RISARpathRepo]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIDTFpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIDTFpathRepo]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIQTYpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.RIRPOpath]);
            if (!Directory.Exists(sysDriveDirectory))
            {
                eodResult = EODResult.Fail;
                Progress("File directory " + sysDriveDirectory + " does not exist..");
                ReportDirPathMissing(interfaceName, sysDriveDirectory, runNo);
            }

            return eodResult;
        }

        private void Progress(string message)  //Can be inline
        {
            progress = message;
            Console.WriteLine(progress);
        }

        private struct MSGQArgument
	    {
            public string Value { private get; set; }
            public string Path { get; set; }
            public string FileName { get; set; }

            public override string ToString()
            {
                var fullPath = String.IsNullOrWhiteSpace(Path) ? "" : System.IO.Path.GetFullPath(Path);
                var fileNameOnly = String.IsNullOrWhiteSpace(FileName) ? "" : System.IO.Path.GetFileName(FileName);

                return (Value ?? "")
                        .Replace("{PATH}", fullPath)
                        .Replace("{FILE}", fileNameOnly);
            }
	    }
    }
}
