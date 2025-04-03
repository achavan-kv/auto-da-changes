-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM  sys.objects o INNER JOIN sys.schemas s ON o.schema_id = s.schema_id where o.name = 'perioddata' and s.name = 'Merchandising')
DROP TABLE [Merchandising].[PeriodData]


CREATE TABLE [Merchandising].[PeriodData](
	[id] [smallint] IDENTITY(1,1) NOT NULL,
	[year] [smallint] NOT NULL,
	[period] [tinyint] NOT NULL,
	[week] [tinyint] NOT NULL,
	[startdate] [date] NULL,
	[enddate] [date] NULL,
 CONSTRAINT [PK_perioddata] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO