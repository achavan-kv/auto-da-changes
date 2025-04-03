-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


Alter TABLE Delivery alter column itemno VARCHAR(18) not null
go
Alter TABLE Delivery alter column parentitemno VARCHAR(18) not null
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Schedule')
BEGIN
  ALTER TABLE Schedule ADD ItemID INT not null default 0
END
go


UPDATE Schedule 
	set ItemId=ISNULL(s.ID,0) 
from Schedule l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go
