-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'ActualCost' and object_name(id) = 'WTRreport')
BEGIN
	ALTER TABLE WTRreport ADD ActualCost MONEY
	ALTER TABLE WTRreport ADD ActualCostLY MONEY
	ALTER TABLE WTRreport ADD YTDCost MONEY
	ALTER TABLE WTRreport ADD YTDCostLY MONEY
	ALTER TABLE WTRreport ADD ActualValueMTD MONEY
	ALTER TABLE WTRreport ADD ActualCostMTD MONEY
	ALTER TABLE WTRreport ADD MTDSalesLY MONEY
	ALTER TABLE WTRreport ADD MTDCostLY MONEY

End

IF NOT EXISTS(select * from syscolumns where name = 'ActualCost' and object_name(id) = 'TradingSummaryHdr')
BEGIN
	ALTER TABLE TradingSummaryHdr ADD ActualCost VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD ActualCostLY VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD YTDCost VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD YTDCostLY VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD ActualValueMTD VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD ActualCostMTD VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD MTDSalesLY VARCHAR(20)
	ALTER TABLE TradingSummaryHdr ADD MTDCostLY VARCHAR(20)

End

IF NOT EXISTS(select * from syscolumns where name = 'ActualCost' and object_name(id) = 'TradingSummary')
BEGIN
	ALTER TABLE TradingSummary ADD ActualCost Money
	ALTER TABLE TradingSummary ADD YTDCost Money
	ALTER TABLE TradingSummary ADD ActualValueMTD Money
	ALTER TABLE TradingSummary ADD ActualCostMTD Money

End

go

UPDATE TradingSummaryHdr
set ActualCost='ActualCost',
	ActualCostLY='ActualCostLY',
	YTDCost='YTDCost',
	YTDCostLY='YTDCostLY',
	ActualValueMTD='ActualValueMTD',
	ActualCostMTD='ActualCostMTD',
	MTDSalesLY='MTDSalesLY',
	MTDCostLY='MTDCostLY'

