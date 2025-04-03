-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from syscolumns
			where name = 'parentitemno'
			and object_name(id) = 'summary2_non')
BEGIN
	alter table summary2_non drop column parentitemno 
END

IF EXISTS(select * from syscolumns
			where name = 'parentitemno'
			and object_name(id) = 'summary2_sec')
BEGIN
	alter table summary2_sec drop column parentitemno 
END