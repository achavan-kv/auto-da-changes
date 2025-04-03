-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_code_update]'))
DROP TRIGGER [dbo].[trig_code_update]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[taxitem]') AND name = N'PK_taxitem')
ALTER TABLE [dbo].[taxitem] DROP CONSTRAINT [PK_taxitem]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'TaxItem')
BEGIN
  ALTER TABLE TaxItem ADD ItemID INT not null default 0
END

ALTER TABLE TaxItem alter column ItemNo VARCHAR(18) not null 

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'TaxItemHist')
BEGIN
  ALTER TABLE TaxItemHist ADD ItemID INT not null default 0
END
go

ALTER TABLE TaxItemHist alter column ItemNo VARCHAR(18) not null 

UPDATE TaxItem
	set ItemID=i.id
from TaxItem t INNER JOIN Stockinfo i on i.itemno=t.ItemNo
where ISNULL(t.ItemID,0)=0 

UPDATE TaxItemHist
	set ItemID=i.id
from TaxItemHist t INNER JOIN Stockinfo i on i.itemno=t.ItemNo
where ISNULL(t.ItemID,0)=0 

delete TaxItem  where ItemID=0
go



ALTER TABLE [dbo].[taxitem] ADD  CONSTRAINT [PK_taxitem] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

UPDATE Codecat 
	set CodeHeaderText='Item No', DescriptionHeaderText='Special Tax Rate'
Where Category='TXR' 

UPDATE Code 
	set additional=''
Where Category='TXR'

Alter TABLE code alter column code VARCHAR(18) not null






