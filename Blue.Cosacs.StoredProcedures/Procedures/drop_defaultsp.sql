SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[drop_defaultsp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[drop_defaultsp]
GO


create procedure drop_defaultsp @column_name varchar (32),@table_name varchar (32) as
declare @cname varchar (32),@id integer, @statement SQLText,@parentid integer
select @id=cdefault from
syscolumns
where name =@column_name
and cdefault in (select id  from 
sysobjects where parent_obj in (select id from 
sysobjects where name =@table_name))
select @cname=
name from
sysobjects
where id=@id
print 'dropping constraint ' + @cname
set @statement = 'Alter table  ' + @table_name + ' drop constraint ' + @cname
execute sp_executesql @statement


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

