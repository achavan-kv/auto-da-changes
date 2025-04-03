SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_getacctsforalloccodes_sp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_getacctsforalloccodes_sp]
GO

CREATE procedure dn_getacctsforalloccodes_sp 
@code varchar(4),-- one of the following: AC account code,CC customer code,NAC no account code,NCC no customer code 
@coderestriction varchar (3)
as declare @statement SQLText

begin
   
   set @statement = 'delete from #allocquery where '
    
   if @coderestriction='AC' or @coderestriction='CC'
       set @statement= @statement + ' not ' --as we are removing we have to use this counterintuitive logic
	if @coderestriction ='AC' or   @coderestriction ='NA'  --account code
		set @statement = @statement + ' exists (select * from acctcode where acctcode.acctno ' +
												'	=#allocquery.Acctno and code = ''' + @code + ''' and datedeleted is null) '
   else
      if @coderestriction='CC' or  @coderestriction ='NC'  --customer code
          set @statement = @statement + ' exists (select * from custcatcode where custcatcode.custid ' +
												'	=#allocquery.custid and code = ''' + @code + ''' and datedeleted is null) '

	 execute sp_executeSQL  @statement
    --SELECT @STATEMENT
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

