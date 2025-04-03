-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec DN_SRCheckCallsPerDaySP 5,3,0

/****** Object:  StoredProcedure [dbo].[DN_SRCheckCallsPerDaySP]    Script Date: 01/30/2007 14:23:23 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRCheckCallsPerDaySP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRCheckCallsPerDaySP]

GO

-- =============================================
-- Author:		J.Hemans
-- Create date: 30/01/2007
-- Description:	Returns Slot Date(s) where the number of slots is greater than the new calls per day for a technician
-- =============================================

CREATE PROCEDURE DN_SRCheckCallsPerDaySP
	-- Add the parameters for the stored procedure here
	@TechnicianID INT,
	@Calls SMALLINT,
	@Return INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @Return = 0

    SELECT SlotDate FROM dbo.SR_TechnicianDiary t JOIN dbo.SR_ServiceRequest s ON t.ServiceRequestNo = s.ServiceRequestNo
	WHERE TechnicianId = @TechnicianID AND Status IN ('E','A','T', 'H') -- FA 23-11 UAT 703 Changed filter as it was only considering status = 'A'.
	GROUP BY SlotDate having MAX(SlotNo) > @Calls -- FA 23-11 UAT 703 Wrong slot reallocation if number does not consider also blank slots.

	SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return

END
GO
