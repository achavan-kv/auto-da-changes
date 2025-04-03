-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns
			where name = 'ParentItemID'
			and object_name(id) = 'summary2_non')
BEGIN
	alter table summary2_non add ParentItemID int not null default 0
END

IF NOT EXISTS(select * from syscolumns
			where name = 'ParentItemID'
			and object_name(id) = 'summary2_sec')
BEGIN
	alter table summary2_sec add ParentItemID int not null default 0
END