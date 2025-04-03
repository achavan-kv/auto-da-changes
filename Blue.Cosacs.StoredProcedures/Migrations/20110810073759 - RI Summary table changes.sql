-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Summary2_non')
BEGIN
  ALTER TABLE Summary2_non ADD ItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Summary2_sec')
BEGIN
  ALTER TABLE Summary2_sec ADD ItemID INT not null default 0
END
go

alter table Summary2_sec alter column ItemNo VARCHAR(18)
alter table Summary2_non alter column ItemNo VARCHAR(18)
