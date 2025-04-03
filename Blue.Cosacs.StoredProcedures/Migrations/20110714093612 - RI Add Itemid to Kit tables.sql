-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT OBJECT_NAME(id),* FROM syscolumns 
			   WHERE name = 'KitId'
			   AND OBJECT_NAME(id) = 'KitClineItem')
BEGIN
	ALTER TABLE KitClineItem ADD KitId INT not NULL default 0
END

IF NOT EXISTS (SELECT OBJECT_NAME(id),* FROM syscolumns 
			   WHERE name = 'ComponentId'
			   AND OBJECT_NAME(id) = 'KitClineItem')
BEGIN
	ALTER TABLE KitClineItem ADD ComponentId INT not NULL default 0
END

Alter TABLE KitClineItem alter column kitNo VARCHAR(18) not null
Alter TABLE KitClineItem alter column ComponentNo VARCHAR(18) not null

go

IF NOT EXISTS (SELECT OBJECT_NAME(id),* FROM syscolumns 
			   WHERE name = 'KitId'
			   AND OBJECT_NAME(id) = 'KitlineItem')
BEGIN
	ALTER TABLE KitlineItem ADD KitId INT not NULL default 0
END

Alter TABLE KitlineItem alter column kitNo VARCHAR(18) not null

go

UPDATE KitClineItem
	set KitId=s.id 
From KitClineItem k INNER JOIN StockInfo s on k.Kitno=s.itemno
where KitId=0

UPDATE KitClineItem
	set ComponentId=s.id 
From KitClineItem k INNER JOIN StockInfo s on k.ComponentNo=s.itemno
where ComponentId=0

UPDATE KitlineItem
	set KitId=s.id 
From KitlineItem k INNER JOIN StockInfo s on k.Kitno=s.itemno
where KitId=0

