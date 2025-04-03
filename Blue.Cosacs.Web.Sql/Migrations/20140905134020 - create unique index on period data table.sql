-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT object_name(object_id), * FROM  sys.indexes o  where o.name = 'IX_perioddata_unique')
DROP INDEX [IX_perioddata_unique] ON [Merchandising].[PeriodData] 


CREATE UNIQUE NONCLUSTERED INDEX [IX_perioddata_unique] ON [Merchandising].[PeriodData]
(
	[year] ASC,
	[period] ASC,
	[week] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]