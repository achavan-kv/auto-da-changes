IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Service].[Holiday]') AND name = N'IX_UserId')
DROP INDEX [IX_UserId] ON [Service].[Holiday] WITH ( ONLINE = OFF )
GO

ALTER TABLE Service.Holiday
DROP COLUMN Approved
GO

ALTER TABLE Service.Holiday
DROP COLUMN Date
Go

ALTER TABLE Service.Holiday
ADD StartDate SMALLDATETIME NOT NULL
GO


ALTER TABLE Service.Holiday
ADD EndDate SMALLDATETIME NOT NULL
GO


CREATE NONCLUSTERED INDEX IX_HolidayDates 
ON Service.Holiday (UserId) 
INCLUDE (StartDate,EndDate)
GO

