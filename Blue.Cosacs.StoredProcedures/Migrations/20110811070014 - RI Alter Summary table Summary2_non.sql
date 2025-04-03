-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from syscolumns
			where name = 'itemdescr1'
			and object_name(id) = 'summary2_non')
BEGIN

	alter table summary2_non alter column itemdescr1 varchar(35) null
	
END