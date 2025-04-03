SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRebateForecastReportDSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRebateForecastReportDSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRebateForecastReportDSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_GetRebateForecastReportDSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rebate Forecast Report D 
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the Rebate Forecast report D.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/05/09 jec  Order by descending sequence
-- 14/05/09 jec  error - remove above change
-- ================================================
	-- Add the parameters for the stored procedure here
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	CONVERT(VARCHAR(11), period_end, 113) as periodend, 
		year_end_forecast, agreement_revised_so_rebate_recalculated,
		account_settled_early, due_date_changed, agreement_revised_and_due_date_changed,
		unaccounted, actual, sequence, outstanding_balance_below_1, date_last_changed,
		below_delivery_threshold
	INTO	#reportd
	FROM	RebateforecastD

	UPDATE	#reportd 
	SET	periodend = REPLACE(periodend, ' ', '-')
	
	--IP - 07/04/08 - CR931 - Sum all the values for the periodend selected and group by periodend
	-- to give the company total for each period end.
	SELECT	periodend as PeriodEnd, 
		SUM(year_end_forecast) as Forecast, 
		SUM(agreement_revised_so_rebate_recalculated) as Revised,
		SUM(due_date_changed) as DueDate, 
		SUM(agreement_revised_and_due_date_changed) as AgreementDueDate,
		SUM(account_settled_early) as Settled, 
		SUM(outstanding_balance_below_1) as OutstBal, 
		SUM(date_last_changed) as DateLastChanged,
		SUM(below_delivery_threshold) as Threshold,
		SUM(unaccounted) as Unaccounted,
		SUM(actual) as Actual
	FROM	#reportd
	GROUP BY PeriodEnd, sequence
	ORDER BY sequence

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End