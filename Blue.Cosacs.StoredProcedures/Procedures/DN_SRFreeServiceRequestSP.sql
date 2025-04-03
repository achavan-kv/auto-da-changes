SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRFreeServiceRequestSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRFreeServiceRequestSP]
GO


CREATE PROCEDURE dbo.DN_SRFreeServiceRequestSP
    @TechnicianId       INTEGER,
    @SlotDate           SMALLDATETIME,
    @SlotNo             SMALLINT,
    @Return             INTEGER OUTPUT

AS DECLARE
    @SROrInstallationNo INTEGER,
    @BookingType CHAR(1)

    SET NOCOUNT ON
    SET @Return = 0

    -- Get the unique SR id
    SELECT 
	@SROrInstallationNo =	CASE
								WHEN BookingType IN ('R', 'E') THEN ServiceRequestNo
								WHEN BookingType IN ('I') THEN InstallationNo
								ELSE NULL
							END,
	@BookingType = BookingType
    FROM   SR_TechnicianDiary
    WHERE  TechnicianId = @TechnicianId
    AND    SlotDate = @SlotDate
    AND    SlotNo = @SlotNo


    -- Delete the booking
    -- This will also delete multiple slots on the same day
    DELETE FROM SR_TechnicianDiary
    WHERE  TechnicianId = @TechnicianId
    AND    SlotDate = @SlotDate
    AND    ((@BookingType IN ('R', 'E') AND ServiceRequestNo = @SROrInstallationNo) OR 
			(@BookingType IN ('I') AND InstallationNo = @SROrInstallationNo))
			
   
	IF(@BookingType IN ('R', 'E'))
		-- Update the Allocation for this SR
		UPDATE  SR_Allocation
		SET     DateAllocated = '',
				Zone = '',
				TechnicianId = 0,
				RepairDate = ''
		WHERE   ServiceRequestNo = @SROrInstallationNo


    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
