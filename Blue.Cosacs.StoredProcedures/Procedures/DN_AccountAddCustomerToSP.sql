SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountAddCustomerToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountAddCustomerToSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_AccountAddCustomerToSP]    Script Date: 11/05/2007 11:45:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE  [dbo].[DN_AccountAddCustomerToSP]
--------------------------------------------------------------------------------
--
-- Project      : 
-- File Name    : DN_AccountAddCustomerToSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 18/01/13 ip #12038 - UAT181 - Linking existing account to a new Customer incorrectly
--					 created new proposal record. Now updating Customer ID to the new 
--					 Customer ID on all relevant tables.
--------------------------------------------------------------------------------
            @acctNo varchar(12),
            @custId varchar(26),
            @relationship varchar(1),
            @user int,
            @return int OUTPUT

AS
 SET NOCOUNT ON
    SET     @return = 0            --initialise return code

    DECLARE @dateprop datetime
    SET     @dateprop = getdate()
    DECLARE @accttype char(1)
    
    DECLARE @latest datetime
    DECLARE @lastDate datetime
    DECLARE @maritalstat char(1)
    DECLARE @nationality char(4)
    DECLARE @empname varchar(26)
    DECLARE @empaddr1 varchar(50)
    DECLARE @empaddr2 varchar(50)
    DECLARE @empcity varchar(50)
    DECLARE @empdept varchar(26)
    DECLARE @emppostcode char(10)
    DECLARE @rowCount integer




    SET     @maritalstat = ''
    SET     @nationality = ''

    SET     @empaddr1 = ''
    SET     @empaddr2 = ''
    SET     @empcity = ''
    SET     @empdept = ''
    SET     @emppostcode = ''
    SET     @empname = ''

	--IP - 18/01/13 - #12038
	DECLARE @prevCustId VARCHAR(20)
		SELECT @prevCustId = custid FROM custacct WHERE acctno = @acctno
									AND hldorjnt = 'H'

    IF(@relationship = 'H')        --ensure there is only ever one holder
    BEGIN
        DELETE
        FROM   custacct
        WHERE  acctno = @acctNo
        AND    hldorjnt = 'H'
    END        

    DELETE
    FROM   custacct
    WHERE  acctno = @acctNo
    AND    custid  = @custid

    INSERT    
    INTO   custacct
           (origbr, custid, acctno, hldorjnt)
    VALUES (null, @custId, @acctNo, @relationship)

    SELECT @accttype = AT.accttype 
    FROM   acct A INNER JOIN accttype AT
    ON     A.accttype = AT.genaccttype
    WHERE  A.acctno = @acctno 

    /* May need to create a Proposal for a main holder of an HP or RF account */
    IF   @relationship = 'H'
    AND  @accttype NOT IN ('C', 'S', 'L')
    BEGIN
    
        SELECT  @lastDate = MAX(DateProp)
        FROM    Proposal
        WHERE   acctno = @acctno 
        AND     custid = @custid

        SET @rowCount = @@ROWCOUNT
		
		--IP - 18/01/13 - #12038
		-- If linking the account to a different customer
		IF(@prevCustId IS NOT NULL and @prevCustId != @custId and (@rowCount = 0 OR @lastDate IS NULL)) 
		BEGIN
			
			UPDATE proposal 
			SET custid = @custid
			WHERE acctno = @acctno
			AND custid = @prevCustId
			
			UPDATE proposalflag
			SET custid = @custid
			WHERE Acctno = @acctno
			AND custid = @prevCustId

		END
		ELSE
		BEGIN --IP - 18/01/13 - #12038 - no existing customer therefore create new proposal 
		
				IF (@rowCount = 0 OR @lastDate IS NULL)
				BEGIN

					/* There is no proposal record for this custid/acctno */
					/* so create a dummy one and add the proposal flags   */

					SELECT  @latest = MAX(dateprop)
					FROM    proposal
					WHERE   custid = @custid

					IF (@latest is not null)
					BEGIN
						SELECT  @maritalstat = maritalstat,
								@nationality =  nationality,
								@empname = empname,
								@empaddr1 = empaddr1,
								@empaddr2 = empaddr2,
								@empcity = empcity,
								@empdept = empdept,
								@emppostcode = emppostcode
						FROM    proposal
						WHERE   custid = @custid
						AND     dateprop = @latest
					END
					ELSE
					BEGIN
						SELECT  @empaddr1 = isnull(cusaddr1, ''),
								@empaddr2 = isnull(cusaddr2, ''),
								@empcity = isnull(cusaddr3,''),
								@emppostcode = isnull(cuspocode,'')
						FROM    custaddress
						WHERE   custid = @custid 
						AND     addtype = 'W'
						AND     datemoved is null
					END

					INSERT INTO proposal 
							(origbr,        custid,          dateprop,        origbranchno,    sanctserno,
							empeenoprop,    maritalstat,     dependants,      yrscuremplmt,
							mthlyincome,    jobslstyrs,      health,          otherpmnts,
							scorecardno,    points,          propresult,      reason,
							yrscurraddr,    yrsprevaddr,     bankaccttype,    yrsbankachld,
							acctno,         propnotes,       hasstring,       AddIncome,
							AppStatus,      CCardNo1,        CCardNo2,        CCardNo3,
							CCardNo4,       Commitments1,    Commitments2,    Commitments3,
							EmpAddr1,       EmpAddr2,        EmpCity,         EmpDept,
							EmpPostCode,    EmpName,         Location,        Nationality,
							NoOfRef,        PEmpMM,          PEmpYY,          ProofAddress,
							ProofId,        ProofIncome,     S1Comment,       S2Comment,
							SpecialPromo,   A2MthlyIncome,   A2AddIncome,     A2MaritalStat,
							TransactIdNo,   PAddress1,       PAddress2,       PCity,
							PPostCode,      PAddYY,          PAddMM,          PResStatus,
							empeenochange,  datechange,      A2Relation,
							RFCategory)
					VALUES (0  ,           @custid ,      @dateprop  ,    0  ,
							null  ,        0  ,           @maritalstat  , 0  ,            
							0  ,           null  ,        0  ,            ''  ,
							null  ,        null  ,        0  ,            ''  ,
							null  ,        0  ,           0  ,            ''  ,
							0  ,           @acctno ,      ''  ,          0  ,
							null  ,        ''  ,         ''  ,          ''  ,
							''  ,         ''  ,         null ,          null  ,
							null  ,        @empaddr1  ,   @empaddr2  ,    @empcity  ,
							@empdept  ,    @emppostcode , @empname  ,     ''  ,
							@nationality , 0  ,           0  ,            0  ,            
							''  ,         ''  ,         ''  ,          ''  ,
							''  ,         ''  ,         null  ,         null  ,
							''  ,         '',           ''  ,          ''  ,
							''  ,         ''  ,         0  ,            0  ,
							''  ,         0  ,           getdate(),      '',
							0  )

					INSERT INTO proposalflag
							(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
					VALUES  (0, @custid, @dateprop, 'S1', null, @user, @acctNo)

					INSERT INTO proposalflag
							(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
					VALUES  (0, @custid, @dateprop, 'S2', null, @user, @acctNo)

					INSERT INTO proposalflag
							(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
					VALUES  (0, @custid, @dateprop, 'DC', null, @user, @acctNo)

					/* call procedure to add any DA flags */
					EXEC DN_ProposalFlagAddDA @return out, @dateprop, @custid, @user, @acctno
				END
				ELSE
				BEGIN
					/* There is a proposal record for this custid/acctno */
					/* but make sure all the proposal flags exist        */

					IF NOT EXISTS (SELECT 1 FROM ProposalFlag
								   WHERE acctno = @acctNo AND CheckType = 'S1')
					BEGIN
						INSERT INTO proposalflag
								(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
						VALUES  (0, @custid, @lastDate, 'S1', null, @user, @acctNo)
					END

					IF NOT EXISTS (SELECT 1 FROM ProposalFlag
								   WHERE acctno = @acctNo AND CheckType = 'S2')
					BEGIN
						INSERT INTO proposalflag
								(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
						VALUES  (0, @custid, @lastDate, 'S2', null, @user, @acctNo)
					END

					IF NOT EXISTS (SELECT 1 FROM ProposalFlag
								   WHERE acctno = @acctNo AND CheckType = 'DC')
					BEGIN
						INSERT INTO proposalflag
								(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
						VALUES  (0, @custid, @lastDate, 'DC', null, @user, @acctNo)
					END
				END
		END --IP - 18/01/13 - #12038
    END
        

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END


