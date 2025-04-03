SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_customerandaddressdetailssp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_customerandaddressdetailssp]
GO

CREATE procedure dn_customerandaddressdetailssp @table_name varchar (32),
@getaddress smallint,@gethomephone smallint,@getbasic smallint,@getallphones smallint= 0
as declare
@where varchar (1000), @columns varchar (1000),@tables varchar (300),
@already smallint,@status integer,@join varchar (500),@statement SQLText
set @columns = 'update ' +@table_name +' set '
set @join= ''     
set @where = ' where '
if @getbasic = 1
begin
    set @columns =@columns + ' Title=c.title,  firstname=c.firstname, '+
'name =c.name,ethnicity=c.ethnicity '
    set @tables = 'from customer c'
    set @where = @where + 'c.custid = '+ @table_name + '.[custid] '
    set @already = 1
end

if @getaddress= 1
begin
    --if already set to true then need to put, then before clauses
    if @already = 1
    begin
        set @columns =@columns + ', ' 
        set @tables =@tables + ', '
        set @where =@where + ' and '
    end
    set @columns =@columns + ' cusaddr1 =ca.cusaddr1, cusaddr2= CA.cusaddr2,'
            + ' cusaddr3 = CA.cusaddr3, postcode= CA.cuspocode '
    set @where =@where + ' CA.custid = '+ @table_name + '.custid ' +
            ' and CA.addtype = ''H'' '  +
            ' and (CA.datemoved is null or CA.datemoved = ''1-jan-1900'') '
    set @tables =@tables + ' custaddress ca '
end

    set @statement = @columns +@tables +@join +@where
    execute sp_executeSQL  @statement  
     --       delete from Alex
            --insert Alex values (@statement)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

