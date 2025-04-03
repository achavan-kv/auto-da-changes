/****** Object:  StoredProcedure [dbo].[DN_SRReportAwaitingPaymentSP]    Script Date: 10/23/2006 14:23:17 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportAwaitingPaymentSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportAwaitingPaymentSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Description:	Awaiting Payment Service Report
-- =============================================
CREATE PROCEDURE DN_SRReportAwaitingPaymentSP
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Title:	DN_SRReportAwaitingPaymentSP
--
--	This procedure will retieve the details for Awaitng Payment.
-- 
-- Change Control
-----------------
-- 07/01/11 jec CR1030 - return Status and date change
-- 10/01/11 jec CR1030 - return 'Charge To' account
-- 21/01/11 jec Restrict rows returned & return customer account & 1900 dates as null
-- 08/02/11 ip  Sprint 5.10 - #2977 - CR1030 - Awaiting Payment Screen - write off account - indicate if the charge account can be written off
-- 15/02/11 ip  Sprint 5.10 - #3182 - When working out if the number of months since resolution exceeds the country parameter, now using DATEADD for better accuracy
--							  previously used DATEDIFF
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@Return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
	
	declare @monthsSinceResolution int	--IP - 08/02/11 - Sprint 5.10 - #2977
	select @monthsSinceResolution = value from countrymaintenance where codename = 'MonthsSinceSRResolved'
	
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, sr.AcctNo as 'AccountNo'
		, SR.Status
		, DateLogged
		, case when SA.DateAllocated ='1900-01-01' then null else SA.DateAllocated end as DateAllocated
		, SA.RepairDate
		, ca.AcctNo as ChargeToAcct		--CR1030
		, AC.outstbal
		, case when SR.DateChange ='1900-01-01' then null else SR.DateChange end as 'DateChange' 
		, case when SR.Status = 'R' AND AC.outstbal > 0 AND (SRE.DateClosed < dateadd(m, -@monthsSinceResolution, getdate())) then cast(1 as bit)else cast(0 as bit)end as 'AllowWriteOff' --IP - 15/02/11 - #3182 - Changed from using datediff to dateadd for accuracy
	FROM SR_ServiceRequest SR INNER JOIN
		SR_Allocation SA ON SR.ServiceRequestNo = SA.ServiceRequestNo LEFT OUTER JOIN
		SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo INNER JOIN
		SR_ChargeAcct CA ON  SR.ServiceRequestNo = CA.ServiceRequestNo INNER JOIN
		acct AC ON	AC.AcctNo = CA.AcctNo AND isnull(AC.outstbal, 0) > 0
	WHERE
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
		(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
		(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
		(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND
		-- UAT 344 If there is an outstanding balance it is irrelevant whether the SR is resolved or open
		--ISNULL(SRE.DateClosed, '1900-01-01') = '1900-01-01' AND 
		CA.ChargeType IN ('C', 'D') -- Only cash or delivers
		
	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End