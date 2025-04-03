/****** Object:  StoredProcedure [dbo].[DN_SRReportAwaitingDepositSP]    Script Date: 10/23/2006 13:18:45 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportAwaitingDepositSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportAwaitingDepositSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:			Peter Chong
-- Create date:		23-Oct-2006
-- Description:		Awaiting Deposit Report
-- =============================================
CREATE PROCEDURE DN_SRReportAwaitingDepositSP
-- =============================================
-- Author:		Peter Chong	
-- Create date: 23-Oct-2006
-- Title:	DN_SRReportAwaitingDepositSP
--
--	This procedure will retrieve the details for Awaiting Deposit report.
-- 
-- Change Control
-----------------
-- 21/01/11 jec Restrict rows returned   
-- =============================================
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@return int output	
AS
BEGIN
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
	
	SET NOCOUNT ON;

   SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, DateLogged
		-- UAT 342 Deposit no longer required
		--, DepositAmount
	FROM SR_ServiceRequest SR LEFT OUTER JOIN
	SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo 
	WHERE 
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
		(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
		(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
		(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND
		ISNULL(DateClosed, '1900-01-01') = '1900-01-01' AND
		-- UAT 342 Deposit required determined by charge to and resolution
		--DepositAmount <> 0 AND  DepositPaid <> 'Y' AND 
		Status <> 'N' AND ChargeTo = 'CUS' AND (Resolution = 'MIS' OR Resolution = 'EVC')

	SET @return = @@error
END
GO
