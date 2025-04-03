-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradingSummaryDatesHdr]') AND type in (N'U'))
		DROP TABLE [dbo].[TradingSummaryDatesHdr]
	go
	
	select '' as EmptyText,'CurrYearStart' as CurrYearStart,'CurrYearEnd' as CurrYearEnd,'PrevYearStart' as PrevYearStart,'PrevYearEnd' as PrevYearEnd
	into  TradingSummaryDatesHdr
	
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WTRReportDates]') AND type in (N'U'))
		DROP TABLE [dbo].[WTRReportDates]
	go

Create TABLE WTRReportDates
(
	WTRtext			varchar(20),
	CurrYearStart	CHAR(12),
	CurrYearEnd		CHAR(12),
	PrevYearStart	CHAR(12),
	PrevYearEnd		CHAR(12)
)