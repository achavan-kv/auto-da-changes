SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbpercentcalc]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbpercentcalc]
GO



CREATE procedure dbpercentcalc @acctno   char(12) = " " 
as  
declare
    @outstbal money, 
    @agrmttotal money, 
    @chargetot money, 
    @deltot money, 
    @paidpcent smallint, 
    @status integer, 
    @paytot money 

    set @outstbal = 0;
    set @agrmttotal = 0;
    set @paidpcent = 0;
    set @status = 0;
    set @paytot = 0;

    select  @deltot = sum(transvalue)
    from  fintrans 
    where  transtypecode in ('add', 
        'del', 
        'grt') 
    and acctno = @acctno; 

    select @chargetot = sum(transvalue)
    from  fintrans 
    where  transtypecode in ('adm', 
        'int', 
        'fee') 
    and acctno = @acctno; 

    select @outstbal = isnull(outstbal, 0),
         @agrmttotal = isnull(agrmttotal, 0)  
    from  acct 
    where acctno = @acctno; 

    if @deltot is null
    BEGIN
        set @deltot = 0; 
    END

    if @chargetot is null
    BEGIN
        set @chargetot = 0; 
    END

    if  @outstbal >0 
    and @agrmttotal >0 
    and @deltot >= @agrmttotal
    BEGIN
        set @paidpcent = cast(((@agrmttotal -@outstbal + @chargetot)/ @agrmttotal ) * 100 as smallint); 

        if @paidpcent > 100
        BEGIN
            set @paidpcent = 100; 
        END
        ELSE 
        BEGIN
            IF  @paidpcent < 0 
            BEGIN
                set @paidpcent = 0; 
            END
        END

        return @paidpcent; 
    END
    ELSE
    BEGIN

        if @outstbal = 0 
        and  @deltot >= @agrmttotal
        BEGIN
            set @paidpcent = 100; 
            return @paidpcent;
        END

        if  @deltot = 0 
        and @outstbal < 0 
        and @agrmttotal != 0
        BEGIN
            set @paidpcent = cast(((@outstbal + @chargetot)/ @agrmttotal)*100 as smallint); 
            return @paidpcent; 
        END

        if @deltot > 0
        BEGIN
            if @status = 1
            BEGIN
                set @paidpcent = 0; 
            END
            ELSE
            BEGIN
                set @paidpcent = cast(((-@paytot + @chargetot)/@deltot) * 100 as smallint); 
            END
        END
    END

    return @paidpcent; 



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

