-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns
			where name = 'VariancePct'
			and object_name(id) = 'WTRReport')
BEGIN
	ALTER TABLE WTRReport
	ADD		[VariancePct] float NULL
END
GO


IF NOT EXISTS(select * from syscolumns
			where name = 'YTDVariancePct'
			and object_name(id) = 'WTRReport')
BEGIN
	ALTER TABLE WTRReport
	ADD		[YTDVariancePct] float NULL
END
GO


IF NOT EXISTS(select * from syscolumns
			where name = 'VariancePct'
			and object_name(id) = 'TradingSummaryHdr')
BEGIN
	ALTER TABLE TradingSummaryHdr
	ADD 	[VariancePct] varchar(20) NULL
END
GO

IF NOT EXISTS(select * from syscolumns
			where name = 'YTDVariancePct'
			and object_name(id) = 'TradingSummaryHdr')
BEGIN
	ALTER TABLE TradingSummaryHdr
	ADD 	[YTDVariancePct] varchar(20) NULL
END
GO


update TradingSummaryHdr
set 	[VariancePct] ='VariancePct',
		[YTDVariancePct] ='YTDVariancePct'

GO