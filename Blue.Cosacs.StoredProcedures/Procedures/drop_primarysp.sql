SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[drop_primarysp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[drop_primarysp]
GO


create procedure drop_primarysp @primary varchar (32),@table_name varchar (32) as
declare @cname varchar (32), @statement SQLText,@parentid integer
set @primary = @primary +'%'
select @cname=
name from
sysobjects
where name like @primary 
print 'dropping primary key' + @cname
set @statement = 'Alter table  ' + @table_name + ' drop constraint ' + @cname
execute sp_executesql @statement

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

