-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11201


IF NOT EXISTS(select * from syscolumns
			where name = 'RequireChangePassword'
			and object_name(id) = 'user')
BEGIN

	alter table admin.[user] add RequireChangePassword bit NOT NULL DEFAULT 0
	
END

