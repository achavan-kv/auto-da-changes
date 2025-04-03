/*
    Author:	 John Croft
    Date:    18 May 2006

        This procedure will save the dynamic SQL scripts generated in dn_CustomerMailing
*/

if exists (select * FROM sysobjects where  name ='dn_CustomerMailingScriptSave')
drop procedure dn_CustomerMailingScriptSave
go
create procedure dn_CustomerMailingScriptSave 
		@script varchar(3000),
		@scriptTag varchar(20) 

as

-- create the table if not exist - zz_CustomerMailingScript for storing sql script
-- jec 
IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_name = 'zz_CustomerMailingScript')
create table .dbo.zz_CustomerMailingScript
(
ScriptDateTime datetime,
Script varchar(100),
ScriptTag varchar(20)
)

delete zz_CustomerMailingScript
    where scriptdatetime < getdate()-30

declare @s smallint, @e smallint
set @s=1 
set @e=@s+79

--select @final_statement
while @e< len(@script)+ 79
begin
insert into zz_CustomerMailingScript (scriptdatetime,script,scriptTag)
select getdate(),substring(@script,@s,80),@scriptTag
set @s=@e+1
set @e=@s+79
end

-- end end end end save of SQL script end end end end end end end end end end end 
