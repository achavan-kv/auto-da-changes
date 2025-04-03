
-- **********************************************************************
-- Title: trig_sr_resolution_update
-- Developer: Ilyas Parker
-- Date: 28/09/11
-- Purpose: Create trigger to capture instances when date resolved is saved prior to date logged
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/09/11  IP  #4304 - LW73630 - Capture instances where Resolution Date is saved prior to Date Logged on Service Requests
-- **********************************************************************
IF EXISTS(SELECT name FROM sysobjects WHERE name = 'trig_sr_resolution_update' 
  	  		AND type = 'TR')

BEGIN
	DROP TRIGGER trig_sr_resolution_update
END
GO

CREATE TRIGGER trig_sr_resolution_update
ON  sr_resolution 
FOR update
AS
	
   DECLARE @serviceRequestNo int,
	   @dateClosed smalldatetime,
	   @dateLogged smalldatetime,
	   @error varchar (256)

   SELECT  @serviceRequestNo = servicerequestno,
           @dateClosed = dateclosed
   FROM    inserted

   SELECT @dateLogged = DateLogged 
   FROM	  SR_servicerequest
   WHERE  ServiceRequestNo = @serviceRequestNo
   
   IF((cast(@dateLogged as date) > cast(@dateClosed as date)) and @dateClosed !='01-01-1900')
   BEGIN
	SET @error = 'Date Resolved cannot be prior to Date Logged'
	
	ROLLBACK
	RAISERROR(@error, 16, 1) 
   END
 
   
GO
