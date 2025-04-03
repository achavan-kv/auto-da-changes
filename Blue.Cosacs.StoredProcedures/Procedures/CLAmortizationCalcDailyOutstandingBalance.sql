GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].CLAmortizationCalcDailyOutstandingBalance') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].CLAmortizationCalcDailyOutstandingBalance
GO
GO
/****** Object:  StoredProcedure [dbo].[CLAmortizationCalcDailyOutstandingBalance]    Script Date: 2019-06-21 15:23:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ashish Padwal
-- Create date: 13-06-2019
-- Description:	This procedure will return daily (till current date)outstandingbalance. 
-- =============================================
CREATE PROCEDURE [dbo].[CLAmortizationCalcDailyOutstandingBalance]
	-- Add the parameters for the stored procedure here
	@acctno VARCHAR(12),
	@outstandingBalance MONEY OUTPUT
AS
BEGIN
	 EXEC @outstandingBalance = [fn_CLAmortizationCalcDailyOutstandingBalance] @acctno
		
END

