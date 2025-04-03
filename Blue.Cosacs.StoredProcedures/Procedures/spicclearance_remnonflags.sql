SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[spicclearance_remnonflags]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[spicclearance_remnonflags]
GO


CREATE procedure [dbo].[spicclearance_remnonflags](@holdflags varchar(4)  ) as
begin
    declare
    @status integer 
	
    if @holdflags !=N''
        delete from #temp_acct1
        where not exists (select f.acctno from instantcreditflag f
				 where  f.acctno = #temp_acct1.acctno
				 and    (f.datecleared = ''
				 or		f.datecleared is null
				 or     f.datecleared = '1-jan-1900')
				 and    f.checktype = @holdflags)
			
			
   
            delete from #temp_acct1 where
            exists
                    (select c.acctno from proposalflag b, proposal c, code d
                     where #temp_acct1.acctno = c.acctno and
                     b.acctno = c.acctno and
                     b.datecleared  is  null and
                     d.code=b.checktype and
                     d.category = 'PH1' AND d.code != 'DC') -- ic accounts don't need to be dc'
            set @status =@@error
  
        return @status
end



GO 



