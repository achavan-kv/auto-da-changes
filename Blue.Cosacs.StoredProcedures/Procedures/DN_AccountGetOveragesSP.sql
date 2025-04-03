
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountGetOveragesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetOveragesSP]
GO

CREATE PROCEDURE dbo.DN_AccountGetOveragesSP
    @branchno smallint,
    @acctno varchar(12) OUT,
    @return int OUTPUT

AS

    SET     @return = 0
    DECLARE @branchname varchar(20)

    IF EXISTS (SELECT   1
               FROM     customer 
               WHERE    custid = 'OVERAGE' + CONVERT(varchar, @branchno))
    BEGIN
        SELECT  @acctno = CA.acctno 
        FROM    custacct CA, acct A
        WHERE   CA.custid = 'OVERAGE' + CONVERT(varchar, @branchno) 
        AND     A.acctno = CA.acctno
        AND     A.currstatus != 'S'
    END
    ELSE
    BEGIN
        /* The customer does not exist so create a customer record */
        SELECT  @branchname = branchname
        FROM    branch
        WHERE   branchno = @branchno

        INSERT INTO customer 
            (origbr, custid, otherid, branchnohdle, name, 
            firstname, title, alias, addrsort, namesort, 
            dateborn, sex, ethnicity, morerewardsno, 
            effectivedate, iDNumber, IdType, RFCreditLimit, 
            RFCardSeqNo, RFCardPrinted, creditblocked, 
            datelastscored, RFDateReminded, empeenochange, 
            datechange, maidenname)
        VALUES
            (null, 'OVERAGE' + convert ( varchar, @branchno ),'',0,
            'OVERAGE' + @branchname, '','MR','','','OVERAGE',
            dateadd(year, -20, getdate()),'M','X','',NULL,'','',0,0,'N',0,NULL,
            NULL,0,NULL,NULL)

        /* we still need to create the OVERAGE account and tie it to the new customer 
            but this will be done in the BL layer to make use of existing code */
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

