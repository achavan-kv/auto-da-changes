SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountPopulateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountPopulateSP]
GO

CREATE PROCEDURE DN_AccountPopulateSP

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_AccountPopulateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Account Details with GenAcctType translation
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/11/02  DSR Load Country Code from AcctType so that translation of AcctType
--               will work when saving Account Details.
-- 07/04/04  DSR New column 'Securitised'
--
--------------------------------------------------------------------------------

    @origBr smallint OUT,
    @acctNo varchar(12),
    @acctType char(1) OUT,
    @dateAcctOpen datetime OUT,
    @creditDays smallint OUT,
    @agreementTotal money OUT,
    @dateLastPaid datetime OUT,
    @as400bal money OUT,
    @outstandingBal money OUT,
    @arrears money OUT,
    @highestStatus char(1) OUT,
    @currentStatus char(1) OUT,
    @highStatusDays smallint OUT,
    @branchNo smallint OUT,    
    @paidPcent smallint OUT,
    @termsType varchar(2) OUT,
    @repossArrears money OUT,
    @repossValue money OUT,
    @dateIntoArrears datetime OUT,
    @country varchar(2) OUT,
    @custid varchar(20) OUT,
    @bdwbalance money OUT,
    @bdwcharges money OUT,
    @securitised varchar(1) OUT,
    @return int OUTPUT

AS

    SET     @return = 0            --initialise return code

    SELECT  @origBr             =    A.origbr,    
            @acctNo             =    A.acctno,    
            @acctType           =    AT.accttype,    
            @dateAcctOpen       =    A.dateacctopen,    
            @creditDays         =    A.creditdays,    
            @agreementTotal     =    A.agrmttotal,    
            @dateLastPaid       =    A.datelastpaid,    
            @as400bal           =    A.as400bal,    
            @outstandingBal     =    A.outstbal,    
            @arrears            =    A.arrears,    
            @highestStatus      =    A.highststatus,    
            @currentStatus      =    A.currstatus,    
            @highStatusDays     =    A.histatusdays,    
            @branchNo           =    A.branchno,    
            @paidPcent          =    A.paidpcent,    
            @termsType          =    A.termstype,    
            @repossArrears      =    A.repossarrears,    
            @repossValue        =    A.repossvalue,    
            @dateIntoArrears    =    A.dateintoarrears,
            @country            =    AT.countrycode,
	@custid 	= 	isnull(CA.custid, ''),
	@bdwbalance	=	A.bdwbalance,
	@bdwcharges	=	A.bdwcharges,
        @securitised    =       A.securitised
    FROM    acct A INNER JOIN accttype AT	
	ON	A.accttype = AT.genaccttype LEFT OUTER JOIN
		custacct CA ON	 A.acctno = CA.acctno
		AND	CA.hldorjnt = 'H' 
    WHERE   A.acctno = @acctNo

    IF(@@rowcount = 0)
    BEGIN
        SET @return = -1
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

