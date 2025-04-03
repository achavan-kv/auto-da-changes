-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RetItemID'
               AND OBJECT_NAME(id) = 'Delivery')
BEGIN
  ALTER TABLE Delivery ADD RetItemID INT not null default 0
END
go

UPDATE Delivery
	set RetItemID=i.ID 
From Delivery d INNER JOIN Stockinfo i on i.itemno=d.RetItemNo
where ISNULL(d.RetItemID,0)=0 and d.RetItemNo!=''
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RetItemID'
               AND OBJECT_NAME(id) = 'Schedule')
BEGIN
  ALTER TABLE Schedule ADD RetItemID INT not null default 0
END
go

UPDATE Schedule
	set RetItemID=i.ID 
From Schedule d INNER JOIN Stockinfo i on i.itemno=d.RetItemNo
where ISNULL(d.RetItemID,0)=0 and d.RetItemNo!=''
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RetItemID'
               AND OBJECT_NAME(id) = 'ScheduleAudit')
BEGIN
  ALTER TABLE ScheduleAudit ADD RetItemID INT not null default 0
END
go

UPDATE ScheduleAudit
	set RetItemID=i.ID 
From ScheduleAudit d INNER JOIN Stockinfo i on i.itemno=d.RetItemNo
where ISNULL(d.RetItemID,0)=0 and d.RetItemNo!=''
go

