-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'OrigTransRefNo' and object_name(id) = 'finxfr')
BEGIN
	alter table finxfr add OrigTransRefNo int null
END
GO