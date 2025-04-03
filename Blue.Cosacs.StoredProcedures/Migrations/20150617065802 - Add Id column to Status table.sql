-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'id' and object_name(id) = 'Status')
BEGIN

	alter table Status add [ID] int identity(1,1) not null
	
	alter table Status add constraint pk_Status primary key(ID)

END
GO