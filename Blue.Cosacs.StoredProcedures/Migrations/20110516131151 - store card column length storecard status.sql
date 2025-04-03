
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from information_schema.tables where table_name = 'StoreCardStatus')
BEGIN
	alter table StoreCardStatus alter column Description varchar(50)
END
GO


IF EXISTS(select * from information_schema.tables where table_name = 'StoreCardStatus')
begin 
declare @statement sqltext
set @statement =
	'IF EXISTS(select * from StoreCardStatus where [status] = ''NA'')
		update StoreCardStatus
		set [Description] = ''No active cards issued''
		where [status] = ''NA'' '
	exec SP_EXECUTESQL @STATEMENT 
end 
GO
