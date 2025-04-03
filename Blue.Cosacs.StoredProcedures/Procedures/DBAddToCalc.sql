SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DBAddToCalc]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DBAddToCalc]
GO




CREATE PROCEDURE DBAddToCalc

--------------------------------------------------------------------------------
--
-- Addto Calculation for Cosacs need to pass in rebate,account and value byref.
--
-- Change Control
-- --------------
-- Date     By   Description
-- ----     --   -----------
-- 18/07/01 AA   Returning 0 if row not found ie. not delivered.
-- 08/08/01 AA   Making sure grant is on correctly
-- 18/06/02 DSR  CR294 Return zero potential for IF Accounts
--
--
--------------------------------------------------------------------------------

    @acctno     char(12),
    @value      money = 0   OUTPUT,
    @rebate     money
as
begin
declare
    @status  integer,  @rowcount  integer ,  @none  integer ,

    @currstatus  char(1),  @agrmttotal  money,  @outstbal money,
    @accttype  char(1),  @termstype  char(2),  @arrears money, /*acct table variables */
    @instalamount money, @datefirst datetime, @datelast  datetime, /*instalment */
    @deposit money, /* agreement variable*/
    @deltotal  money, @paytotal money,
    @dateadded datetime, /* date variable for spa */
    @addtomin money,  @addtoterm smallint,  @servpcent float /* country variables */
    set @value = 0       
    set @none = 0    
    set @status = 0

    select @currstatus = currstatus,
           @agrmttotal = agrmttotal,
           @outstbal = outstbal,
           @accttype = accttype,
           @arrears = arrears,
           @termstype = termstype
    from   acct
    where  acctno = @acctno
        
    set @status = @@error
    if @accttype is null
        set @status = 12386308

    if @status = 0
    BEGIN
        /* CR294 DSR IF Accounts */
        IF @termstype = 'IF' 
        BEGIN
            SET @value = 0
            RETURN 0
        END
        
        select @instalamount = i.instalamount,
               @datefirst = i.datefirst,
               @datelast = i.datelast,
               @deposit = g.deposit
        from   instalplan i, agreement g 
        where  i.acctno = g.acctno
        and    i.agrmtno = 1 and g.agrmtno = 1
        and    i.acctno = @acctno
        
        set @status = @@error

        if @datefirst is null
            set @status = 12386308
    END
       
    if @status = 0 
    BEGIN
        select @deltotal = sum(transvalue) from fintrans
        where  acctno = @acctno
        and    transtypecode in ('DEL','GRT','ADD')
        
        set @status =@@error
        if @deltotal is null
            set @status = 12386308
    END
    
    if @status = 0 
    BEGIN
        /* PAYMENT AMOUNT - EASIER IF YOU just exclude non payment transactions */
        select @paytotal = sum(transvalue)
        from   fintrans
        where  acctno = @acctno
        and    transtypecode NOT in ('DEL','GRT','ADD','INT','ADM','FEE')
        
        set @status =@@error

        if @paytotal is null
            set @status = 12386308
    END

    if @status = 0
    BEGIN
        select @addtomin = addtomin,
               @addtoterm = addtoterm,
               @servpcent = servpcent
        from   country
        
        set @status = @@error
        if @addtoterm is null 
            set @status = 12386308
    END

    if @status = 0
    BEGIN
        if @currstatus not in ('1','2','3')  /* must be in this status */
        begin
            set @none = 1
        end
        if @accttype not in ('0','O','B','D','G')
        begin
            set @none = 1
        end
        if abs(@agrmttotal - @deltotal) > .1 /* must be fully delivered */
        begin
            set @none = 1
        end
        if  @outstbal < .1 and @outstbal is not null /* balance must be > 0 */
        begin
            set @none = 1
        end
        if -@paytotal < (6 * @instalamount + @deposit) /* must have paid 6 insts + deposit */
        begin
            set @none = 1
        end
        if @arrears > @instalamount /* arrears cannot be > 1 instalment */ 
        begin
            set @none = 1
        end
        if (@agrmttotal - @deposit) < 8 * @instalamount /* agrmt must be > 8 instalments */
        begin
            set @none = 1
        end
        
        select @dateadded = dateadded
        from   spa
        where  acctno = @acctno /* no spa in force for this account */ 
        and    (dateexpiry > getdate() or dateexpiry is null or dateexpiry =N'1-jan-1900')
        
        set @status = @@error
        if @dateadded is not null
            set @none = 1
    END
      
    if @status = 0 and @none = 0 
    BEGIN
        SET @VALUE = (@ADDTOTERM *100 * @INSTALAMOUNT)/((@servpcent * @addtoterm/12) + 100)
        set @value = @value - @outstbal + @rebate
    END
      
    if @status = 12386308 /* this is row not found */
        set @status = 0
        
    RETURN @status
END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

