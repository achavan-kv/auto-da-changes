using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Blue.Cosacs;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.Common;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using STL.DAL;
using STL.BLL;

[WebService(Namespace = "http://schemas.bluebridgeltd.com/cosacs/installation/2011/01/")]
public class WInstallation : CommonService
{

    public WInstallation () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<InstResult> GetPendingItems(string acctNo, DateTime? deliveryAuthorisedAfter, DateTime? deliveryAuthorisedBefore,
                                                            int? stockLocation, bool authorisedOnly, bool deliveredOnly, int? top, out string err)
    {
        err = "";

        try
        {
            var result = new InstallationRepository()
                        .GetPendingItems(acctNo, deliveryAuthorisedAfter, deliveryAuthorisedBefore, stockLocation, authorisedOnly, deliveredOnly, top);

            return result.ToList();
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetPendingItems()", ref err);
        }

        return new List<InstResult>();
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<InstResult> Search(InstSearchParameter searchParam, out string err)
    {
        err = "";

        try
        {
            //var result = new InstallationRepository().Search(searchParam);

            //return result.ToList();

            return new InstallationRepository().Search(searchParam);        //6.5
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetPendingItems()", ref err);
        }

        return new List<InstResult>();
    }
    
    [WebMethod]
    [SoapHeader("authentication")]
    public Installation BookTechnician(Blue.Cosacs.Shared.Installation record, int technicianId, DateTime slotDate, 
                                            short[] slots, int user, string rebookingReason, out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository()
                        .BookTechnician(record, technicianId, slotDate, slots, user, rebookingReason);
        }
        catch (InvalidOperationException ex) //todo define specific exception
        {
            err = ex.Message;
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::BookTechnician()", ref err);
        }

        return null;
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public Installation ReleaseTechnician(int technicianId, DateTime slotDate, short slotNoAny, int user, out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository()
                            .ReleaseTechnician(technicianId, slotDate, slotNoAny, user);
        }
        catch (InvalidOperationException ex) //todo define specific exception
        {
            err = ex.Message;
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::ReleaseTechnician()", ref err);
        }

        return null;
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<InstBookingResult> GetBookings(int installationNo, out string err, bool withHistory = false)
    {
        err = "";

        try
        {
            return new InstallationRepository().GetBookings(installationNo, withHistory);
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetBookings()", ref err);
        }

        return new List<InstBookingResult>();
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<InstTechPrintResult> GetBookingForPrint(int? installationNoFrom, int? installationNoTo,
                                                                DateTime? installationDateFrom, DateTime? installationDateTo,
                                                                DateTime? allocationDateFrom, DateTime? allocationDateTo,
                                                                int? technicianId, out string err)
    {
        err = "";

        try
        {
            var result = new InstallationRepository().GetBookingForPrint(installationNoFrom, installationNoTo, 
                                                                        installationDateFrom, installationDateTo,
                                                                        allocationDateFrom, allocationDateTo,
                                                                        technicianId);

            return result.ToList();
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetBookingForPrint()", ref err);
        }

        return new List<InstTechPrintResult>();
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public bool IsBooked(int installationNo, out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository().IsBooked(installationNo);
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::IsBooked()", ref err);
        }

        return false;
    }   
 
    [WebMethod]
    [SoapHeader("authentication")]
    public StockItemDetails GetSparePartDetail(string partNo, out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository().GetSparePartDetail(partNo);
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetSparePartDetail()", ref err);
        }

        return null;
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public int UpdateResolution(int installationNo, short branchNo, InstallationResolution resolution, List<InstallationSparePart> spareParts, int user, out bool conflictOccurred, out string err)
    {
        err = "";
        conflictOccurred = false;

        SqlConnection conn = new SqlConnection(Connections.Default);

        try
        {
            conn.Open();

            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            {

                var analyses = new InstallationRepository().UpdateResolution(installationNo, resolution, spareParts, user, conn, trans);

                if (analyses != null)
                    new BServiceRequest().PostInstallationCharges(conn, trans, analyses, installationNo, branchNo, spareParts.ToDataTable());   //#12116

                trans.Commit();
            }
        }
        catch (STLException ex) //todo catch ChangeConflictException instead
        {
            if (ex.Data["IsConflict"] != null && Convert.ToBoolean(ex.Data["IsConflict"])) 
            {
                err = "Records has been updated by another user";
                conflictOccurred = true;
            }
            else
            {
                Catch(ex, "WInstallation::UpdateResolution()", ref err);
            }            
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::UpdateResolution()", ref err);
        }
        finally
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }

        return 0;
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public InstResolutionResult GetResolution(int installationNo, out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository().GetResolution(installationNo);
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::GetResolution()", ref err);
        }

        return null;
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<InstChargeAnalysisResult> BuildEmptyChargeAnalysis(out string err)
    {
        err = "";

        try
        {
            return new InstallationRepository()
                        .BuildInitialChargeAnalysis();
        }
        catch (Exception ex)
        {
            Catch(ex, "WInstallation::BuildEmptyChargeAnalysis()", ref err);
        }

        return new List<InstChargeAnalysisResult>();
    }
}
