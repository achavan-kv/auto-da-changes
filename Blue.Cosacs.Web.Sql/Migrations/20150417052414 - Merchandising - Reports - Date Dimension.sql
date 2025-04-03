-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Merchandising.Dates (
	DateKey int NOT NULL,
	FullDateAlternateKey date NOT NULL,
	DayNumberOFWeek tinyint NOT NULL,
	DayNameOfWeek nvarchar(10) NOT NULL,
	DayNumberOfMonth tinyint NOT NULL,
	DayOfFiscalYear smallint NOT NULL,
	DayOfCalendarYear smallint NOT NULL,
	CalendarWeek tinyint NOT NULL,
	FiscalWeek tinyint NOT NULL,
	MonthName nvarchar(10) NOT NULL,
	CalendarPeriod tinyint NOT NULL,
	FiscalPeriod tinyint NOT NULL,
	CalendarYear smallint NOT NULL,
	FiscalYear smallint NOT NULL
	 CONSTRAINT [PK_Date_DateKey] PRIMARY KEY CLUSTERED 
(
	[DateKey] ASC
), CONSTRAINT [AK_Date_FullDateAlternateKey] UNIQUE NONCLUSTERED 
(
	[FullDateAlternateKey] ASC
)
) ON [PRIMARY]
