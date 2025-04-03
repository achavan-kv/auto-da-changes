GO
/****** Object:  StoredProcedure [dbo].[DN_SRUpdateAllTechnicianDetails]    Script Date: 10/17/2006 16:13:19 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRUpdateAllTechnicianDetails]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRUpdateAllTechnicianDetails]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 17-Oct-2006
-- Description:	This procedure will update all the active technicians with the same details
-- =============================================
CREATE PROCEDURE DN_SRUpdateAllTechnicianDetails
	@HoursFrom char(5), 
	@HoursTo char(5), 
	@CallsPerDay int,
	@NoTechniciansExceedingCallsPerDay int output,  --For error reporting (a count of technicians exceeding new calls per day
	@TechnicianWithHighestNoSlots int output,
	@return int output
AS
BEGIN
	SET NOCOUNT ON;
	
	SET @TechnicianWithHighestNoSlots = -1
	
	DECLARE @TableVar TABLE(
    TechnicianID int NOT NULL,
    SlotDate datetime,
    CallsPerDay int);

	
	INSERT INTO @TableVar
	SELECT D.TechnicianID, SlotDate, count(*) [CallsPerDay]
	FROM SR_TechnicianDiary D JOIN 
		SR_Technician S ON S.TechnicianId = D.TechnicianId AND  S.Deleted = 0
	WHERE DateDiff(d, getdate(), SlotDate ) >= 0
	GROUP BY D.TechnicianID, SlotDate
	HAVING Count(*) > @CallsPerDay
	
	SELECT @NoTechniciansExceedingCallsPerDay = Count(*) 
	FROM @TableVar
	
	IF @NoTechniciansExceedingCallsPerDay = 0

		UPDATE [SR_Technician]
		SET 
		  [HoursFrom] = @HoursFrom
		  ,[HoursTo] = @HoursTo
		  ,[CallsPerDay] = @CallsPerDay
		WHERE Deleted = 0

	ELSE
		
		SELECT TOP 1 @TechnicianWithHighestNoSlots = TechnicianID
		FROM @TableVar	
		GROUP BY TechnicianId, CallsPerDay
		HAVING [CallsPerDay] = MAX(CallsPerDay)
	
	SET @return = @@ERROR
END
GO

