SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetTechnicianDiarySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetTechnicianDiarySP]
GO


CREATE PROCEDURE dbo.DN_SRGetTechnicianDiarySP
	@TechnicianId	    INTEGER,
    @Return             INTEGER OUTPUT

AS   
    SET NOCOUNT ON  
    SET @Return = 0  
  
    -- Load all of the diary slots booked for a technician  
    SELECT  td.TechnicianId,  
            td.SlotDate,  
            td.SlotNo, 
            CASE 
            WHEN td.BookingType IN ('R', 'E') THEN
				td.ServiceRequestNo
			WHEN td.BookingType IN ('I') THEN
				td.InstallationNo
            END as ServiceRequestNo,  
            ISNULL(sr.ServiceBranchNo, 0) AS ServiceBranchNo,
            CASE 
            WHEN td.BookingType IN ('R', 'E') THEN
				CONVERT(VARCHAR,sr.ServiceBranchNo) + CONVERT(VARCHAR,sr.ServiceRequestNo) 
			WHEN td.BookingType IN ('I') THEN
				CONVERT(VARCHAR,td.InstallationNo)
			END AS ServiceRequestNoStr,  
            td.BookingType  
    FROM    SR_TechnicianDiary td
    LEFT JOIN SR_ServiceRequest sr ON sr.ServiceRequestNo = td.ServiceRequestNo 
    WHERE td.TechnicianId = @TechnicianId  
     --Restrict to about last 3 months worth of data  
    AND (DATEDIFF(MONTH,td.SlotDate,GETDATE()) <= 3 OR DATEDIFF(DAY,GETDATE(),td.SlotDate) >= 0)    
  
    SET @Return = @@error  
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
