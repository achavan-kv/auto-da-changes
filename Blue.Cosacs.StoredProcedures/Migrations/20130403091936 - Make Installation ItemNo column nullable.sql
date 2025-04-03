-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- #Related to: #12116

IF EXISTS(select * from syscolumns
			where name = 'ItemNo'
			and object_name(id) = 'Installation')
BEGIN

	alter table Installation alter column ItemNo varchar(18) null
	
END