
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[DN_SRGetTechniciansForPaymentScreenSP]    Script Date: 02/06/2007 13:59:23 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetTechniciansForPaymentScreenSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetTechniciansForPaymentScreenSP]
GO
--exec DN_SRGetTechniciansForPaymentScreenSP 0

-- =============================================
-- Author:		Jez Hemans
-- Create date: 02/02/2007
-- Description:	Returns Technician ID,Name & Total due
-- =============================================

CREATE PROCEDURE DN_SRGetTechniciansForPaymentScreenSP
@Return             INTEGER OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @Return = 0

	SELECT TechnicianId AS TechnicianID,Title + ' ' + FirstName + ' ' + LastName AS TechnicianName, CAST (0.00 AS MONEY) AS TotalDue
    INTO #Technicians
    FROM dbo.SR_Technician
    WHERE Deleted = 0 AND Internal <> 'Y'		-- UAT 485 Internal technicians are not to be shown on the Technician Payment screen.

    SELECT TechnicianId,SUM(tp.TotalCost)AS Cost 
    INTO #Payments
	FROM	dbo.SR_TechnicianPayment tp 
	WHERE	 Status <> 'P' AND Status <> 'D' AND Status <> 'H'
    GROUP BY TechnicianId

    UPDATE #Technicians
    SET TotalDue = Cost
    FROM #Payments P 
    WHERE P.TechnicianId = #Technicians.TechnicianId

    SELECT * FROM #Technicians

	SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
END
GO
