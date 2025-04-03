/****** Object:  StoredProcedure [dbo].[DN_SRReportUnallocatedSP]    Script Date: 10/23/2006 13:05:54 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportUnallocatedSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportUnallocatedSP]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 23-Oct-2006
-- Description:	Unallocated service requests report
-- Modified by: J.Hemans
-- Modified date: 03/01/2008
-- Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340
-- =============================================
CREATE PROCEDURE DN_SRReportUnallocatedSP --@return = 0
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Title:	DN_SRReportUnallocatedSP
--
--	This procedure will retieve the details for Unallocated SR's.
-- 
-- Change Control
-----------------
-- 29/11/10 jec CR1030 - add new parameter - Days Outstanding
-- 07/01/11 jec CR1030 - return Status and date change
-- 21/01/11 jec Restrict rows returned 
-- 10/02/11 IP Sprint 5.10 - #2978 - Changed procedure to return new columns 'ChargeToAcct', 'DepositPaid',
--							 'DepositAmt', 'AllowCancel'. The 'AllowCancel' will be marked as 1 if the
--							 conditions are met for the SR Charge To Account to be cancelled.
-- 25/02/11 IP Sprint 5.11 - #3231 - Exclude settled service requests
-- =============================================
	-- Add the parameters for the function here  
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@DaysOutstanding INT,	--CR1030
	@return int output	
AS


--IP - 10/02/11 - Sprint 5.10 - #2978
declare @daysLastUpdated int
select @daysLastUpdated = value from countrymaintenance where codename = 'DaysSinceSRLastUpdated'

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	--Create temporary table --IP - 10/02/11 - Sprint 5.10 - #2978
	create table #unallocated
	(
		ServiceRequestNo int,
		ServiceRequestNoStr varchar(20),
		Status char(1),
		DateLogged smalldatetime,
		DaysOutstanding int,
		DateChange datetime,
		ChargeToAcct varchar(12),
		DepositPaid char(1),
		DepositAmount decimal,
		AllowCancel bit
	)
	
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
    -- Insert statements for procedure here
    INSERT INTO #unallocated	--IP - 10/02/11 - Sprint 5.10 - #2978
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, SR.Status
		, DateLogged
		, DATEDIFF(d,SR.DateLogged,getdate()) as 'Days Outstanding'
		, SR.DateChange	
		--, DepositPaid			-- UAT 341 field to be removed
		, isnull(SRC.AcctNo,'')			--IP - 10/02/11 - Sprint 5.10 - #2978
		,'N'							--IP - 10/02/11 - Sprint 5.10 - #2978
		,0								--IP - 10/02/11 - Sprint 5.10 - #2978
		,0								--IP - 10/02/11 - Sprint 5.10 - #2978
	FROM SR_ServiceRequest SR LEFT OUTER JOIN
	SR_Allocation SA ON SR.ServiceRequestNo = SA.ServiceRequestNo LEFT OUTER JOIN
	SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo LEFT OUTER JOIN
	SR_ChargeAcct SRC ON SR.ServiceRequestNo = SRC.ServiceRequestNo	--IP - 10/02/11 - Sprint 5.10 - #2978
	WHERE 
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
		(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
		(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
		(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND
		ISNULL(TechnicianId, 0) = 0 AND
		ISNULL(DateClosed, '1900-01-01') = '1900-01-01'
		-- days outstanding > parameter days --CR1030 jec
		and DATEDIFF(d,SR.DateLogged,getdate())> @DaysOutstanding 
		and SR.Status!='C'		--IP - 25/02/11 - #3231

	--Update DepositPaid column --IP - 10/02/11 - Sprint 5.10 - #2978
	UPDATE #unallocated
	SET DepositPaid = 'Y'
	WHERE EXISTS (Select sum(f.transvalue)
					from fintrans f
					where f.transtypecode in ('PAY', 'REF', 'COR', 'XFR', 'SCX')
					and f.acctno = #unallocated.ChargeToAcct
					having abs(sum(f.transvalue)) > 0)
					
	--Update DepositAmt --IP - 10/02/11 - Sprint 5.10 - #2978
	UPDATE #unallocated
	SET DepositAmount = (select abs(isnull(sum(f.transvalue),0))
					from fintrans f
					where f.transtypecode in ('PAY', 'REF', 'COR', 'XFR', 'SCX')
					and f.acctno = #unallocated.ChargeToAcct)
					
	--Update AllowCancel --IP - 10/02/11 - Sprint 5.10 - #2978
	UPDATE #unallocated
	SET AllowCancel = case when #unallocated.Status in ('N', 'T') and #unallocated.ChargeToAcct !='' and #unallocated.DepositAmount > 0
							and (case when #unallocated.DateChange = '01/01/1900' then datediff(d, #unallocated.DateLogged, getdate()) 
									else datediff(d, #unallocated.DateChange, getdate())end) >  @daysLastUpdated then 1 else 0 end
	
	--IP - 10/02/11 - Sprint 5.10 - #2978				
	select ServiceRequestNo,
		   ServiceRequestNoStr,
		   Status,
		   DateLogged,
		   DaysOutstanding as 'Days Outstanding',
		   DateChange,
		   ChargeToAcct,
		   DepositPaid,
		   DepositAmount,
		   AllowCancel
	from #unallocated
	
	SET @return = @@error	
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End