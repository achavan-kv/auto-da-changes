-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'Delivery')
BEGIN
  ALTER TABLE Delivery drop column ID
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Delivery')
BEGIN
  ALTER TABLE Delivery ADD ItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ParentItemID'
               AND OBJECT_NAME(id) = 'Delivery')
BEGIN
  ALTER TABLE Delivery ADD ParentItemID INT not null default 0
END
go

UPDATE Delivery
	set ItemID=i.ID 
From Delivery d INNER JOIN Stockinfo i on i.itemno=d.itemno
where ISNULL(d.ItemID,0)=0
go

UPDATE Delivery
	set ParentItemID=i.ID 
From Delivery d INNER JOIN Stockinfo i on i.itemno=d.ParentItemNo
where ISNULL(d.ParentItemID,0)=0 and d.ParentItemNo!=''
go

