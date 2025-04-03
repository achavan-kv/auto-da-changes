SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sploadclearance_remnonflags]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sploadclearance_remnonflags]
GO


/****** Object:  StoredProcedure [dbo].[sploadclearance_remnonflags]    Script Date: 11/05/2007 12:25:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[sploadclearance_remnonflags](@holdflags varchar(4) ) as
begin
    declare
    @status integer ,@transactenabled smallint

/*	select @transactenabled=transactenabled from 
	country*/
	
    if @holdflags !=N''
        delete from #temp_acct1
        where exists (select c.acctno from proposalflag b, proposal c
             where  c.acctno = #temp_acct1.acctno
             and    b.acctno = c.acctno
             and    b.datecleared != ''
             and    b.datecleared is not null
             and    b.datecleared != '1-jan-1900'
             and    b.checktype = @holdflags)
   
     set @status =@@error
     begin
     /*removing all accounts with outstanding transact flags -PH1 on them*/
/*         if @transactenabled= 1 this is being implemented for ready finance
         begin         */
            delete from #temp_acct1 where
            exists
                    (select c.acctno from proposalflag b, proposal c, code d
                     where #temp_acct1.acctno = c.acctno and
                     b.acctno = c.acctno and
                     b.datecleared  is  null and
                     d.code=b.checktype and
                     d.category = 'PH1')
            set @status =@@error
/*        end*/
     end   
        return @status
end




