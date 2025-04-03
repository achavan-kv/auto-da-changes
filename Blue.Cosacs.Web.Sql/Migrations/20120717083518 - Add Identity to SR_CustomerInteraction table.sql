-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'id' and object_name(id) = 'SR_CustomerInteraction')
BEGIN

	alter table SR_CustomerInteraction add [ID] int identity(1,1) not null
	
	alter table SR_CustomerInteraction add constraint pk_SR_CustomerInteraction primary key(ID)

END
