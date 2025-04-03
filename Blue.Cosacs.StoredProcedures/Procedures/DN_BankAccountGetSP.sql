
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BankAccountGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BankAccountGetSP]
GO



CREATE PROCEDURE dbo.DN_BankAccountGetSP
    @custid varchar(20),
    @acctno varchar(12),
    @bankacctno varchar(20) OUT,
    @bankcode varchar(6) OUT,
    @dateopened datetime OUT,
    @code char(1) OUT,
    @ismandate smallint OUT,
    @duedayid int OUT,
    @acctname varchar(40) OUT,
    @bankname varchar(20) OUT,
    @return int OUTPUT

AS

    DECLARE @ddenabled smallint
	
    SET @return = 0            --initialise return code
    SET @ismandate = 0
    
    SELECT  @ddenabled = ddenabled
    FROM    country

    IF (@acctno != '')        --we are sanctioning an account
    BEGIN
        -- First see if there is a current DDMandate record for this account. It must either have
        -- a blank end date or an end date that is both after today and after the start date.
        SELECT @duedayid   = duedayid,
               @acctname   = bankacctname,
               @bankacctno = bankacctno,
               @bankcode   = bankcode
        FROM   DDMandate 
        WHERE  acctno = @acctno 
        AND    status = 'C'
        AND    (EndDate IS NULL OR (DATEDIFF(Day, GETDATE(), EndDate) > 0 AND DATEDIFF(Day, StartDate, EndDate ) > 0) )

                
        IF (@@rowcount > 0 AND @ddenabled != 0)
        BEGIN
            SET @ismandate = 1

            -- get the remaining info from the BankAcct table for this bankacctno
            -- DSR 8/12/03 - No longer joins on bankacctno  = @bankacctno because there is
            -- no longer any history records on BankAcct. So just get the current record for
            -- this customer.
         
            SELECT TOP 1 
                   @dateopened = dateopened,
                   @code       = code,
                   @bankname   = bankname
            FROM   bankacct ba
            LEFT OUTER JOIN bank bk ON  ba.bankcode = bk.bankcode  
            WHERE   ba.custid   =  @custid
            ORDER BY dateopened DESC  

            IF(@@rowcount = 0)        --this is a problem, it means there is no bankacct record which corresponds
            BEGIN                     --to the DDMandate record
                SET @return = -1
                RAISERROR('DD Mandate found with no corresponding BankAcct record.', 16, 1)        
            END
        END
    END
        
    IF(@ismandate = 0)    --we need to get the details exclusively from the bankacct table
    BEGIN    
        SELECT  TOP 1
                @bankacctno  = bankacctno,
                @bankcode    = ba.bankcode,
                @dateopened  = dateopened,
                @code        = code,
                @bankname    = bankname
        FROM    bankacct ba
        INNER JOIN bank bk ON  ba.bankcode = bk.bankcode
        WHERE   ba.custid   =  @custid
        ORDER BY dateopened DESC
    END

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO