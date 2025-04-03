-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'PartID'
               AND OBJECT_NAME(id) = 'InstallationSparePart')
BEGIN
  ALTER TABLE InstallationSparePart ADD PartID INT not null default 0
END


go

UPDATE InstallationSparePart
	set PartID=i.id
from InstallationSparePart p INNER JOIN Stockinfo i on i.itemno=p.partno
where ISNULL(p.PartID,0)=0 and p.IsNonCourts =0

go

DECLARE @name VARCHAR(50)				    
				    
SET @name =(SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InstallationSparePart]') AND name like N'PK__Installa%')


if @name is not null
	EXEC ('ALTER TABLE InstallationSparePart DROP CONSTRAINT ' + @name)
	
go

ALTER TABLE InstallationSparePart alter column PartNo VARCHAR(18) not null 

ALTER TABLE [dbo].[InstallationSparePart] ADD PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC,
	[PartID] ASC,
	[StockLocation] ASC,
	[PartNo] asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

