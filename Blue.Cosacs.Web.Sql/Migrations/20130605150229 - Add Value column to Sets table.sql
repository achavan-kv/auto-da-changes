-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'Value' and object_name(id) = 'sets')
BEGIN
	alter table [sets] add [Value] money default 0
END

go

update [sets] set [Value] =0

