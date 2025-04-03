SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRebateForecastReportD_BrnSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRebateForecastReportD_BrnSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRebateForecastReportD_BrnSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_GetRebateForecastReportD_BrnSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rebate Forecast Report D by Branch
-- Author       : John Croft
-- Date         : 04/04/2008
--
-- This procedure will get the Rebate Forecast report D by branch.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
			@branchno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	CONVERT(VARCHAR(11), period_end, 113) as periodend, branchno as BranchNo,
		year_end_forecast, agreement_revised_so_rebate_recalculated,
		account_settled_early, due_date_changed, agreement_revised_and_due_date_changed,
		unaccounted, actual, sequence, outstanding_balance_below_1, date_last_changed,
		below_delivery_threshold
	INTO	#reportd
	FROM	RebateforecastD
	Where @branchno=branchno or @branchno=0

	UPDATE	#reportd 
	SET	periodend = REPLACE(periodend, ' ', '-')

	SELECT	periodend as PeriodEnd,
		branchno as BranchNo, 
		year_end_forecast as Forecast, 
		agreement_revised_so_rebate_recalculated as Revised,
		due_date_changed as DueDate, 
		agreement_revised_and_due_date_changed as AgreementDueDate,
		account_settled_early as Settled, 
		outstanding_balance_below_1 as OutstBal, 
		date_last_changed as DateLastChanged,
		below_delivery_threshold as Threshold,
		unaccounted as Unaccounted,
		actual as Actual
	FROM	#reportd
	ORDER BY sequence ASC, BranchNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO