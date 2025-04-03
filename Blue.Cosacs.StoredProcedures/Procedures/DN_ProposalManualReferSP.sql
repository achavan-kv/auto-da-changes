SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalManualReferSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalManualReferSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalManualReferSP]    Script Date: 11/05/2007 12:17:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[DN_ProposalManualReferSP]

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_ProposalManualReferSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update a Proposal to be Referred
-- Author       : D Richardson
-- Date         : 19 Feb 2003
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 19/11/08 jec 70435 Update/Write proposal flag for correct account.
--
--------------------------------------------------------------------------------

    -- Parameters
    @CustId     varchar(20),
    @AcctNo     varchar(12),
    @DateProp   SMALLDATETIME,
    @empeeno	INT,
    @reason		char(2),		
    
    @Return     INTEGER OUTPUT

AS
BEGIN

   declare @preason varchar(2),@reason2 varchar(2),@reason3 varchar(2),@reason4 varchar(2),@reason5 varchar(2)

   select @preason = isnull(reason,''),
			 @reason2 = isnull(reason2,''),
			 @reason3 = isnull(reason3,''),
			 @reason4 = isnull(reason4,''),
			 @reason5 = isnull(reason5,'')
   from proposal
    WHERE   CustId      = @CustId
    AND     DateProp    = @DateProp
    AND     AcctNo      = @AcctNo
   
   if @preason != '' -- retaining real reason for referral if manually referring after rejection.
   begin
      while 1 = 1
      begin
   		if @reason2=''
         begin
           set @reason2 =@preason
                UPDATE
                    Proposal 
                SET
                    reason2 =  @reason2
                 WHERE CustId = @CustId
                 AND     DateProp    = @DateProp
                 AND     AcctNo      = @AcctNo      
    	     break
         end
   		if @reason3=''
         begin
           set @reason3 =@preason
                UPDATE
                    Proposal 
                SET
                    reason3 =  @reason3
                WHERE CustId = @CustId
                    AND     DateProp    = @DateProp
                    AND     AcctNo      = @AcctNo    
    	     break
         end
   		if @reason4=''
         begin
           set @reason4 =@preason
                UPDATE
                    Proposal 
                SET
                    reason4 =  @reason4
                WHERE CustId = @CustId
                    AND     DateProp    = @DateProp
                    AND     AcctNo      = @AcctNo   
    	     break
         end

                 UPDATE
                    Proposal 
                SET
                    reason5 =  @preason
                WHERE CustId = @CustId
                    AND     DateProp    = @DateProp
                    AND     AcctNo      = @AcctNo   
    	     break
      end
   end

    SET @Return = 0;
    
    UPDATE  Proposal
    SET     PropResult  = 'R',
			Reason = @reason
    WHERE   CustId      = @CustId
    AND     DateProp    = @DateProp
    AND     AcctNo      = @AcctNo

    IF(@reason = 'MN')
    BEGIN
		UPDATE  PropResult
		SET     ManualRefer = 'Y'
		WHERE   AcctNo      = @AcctNo
	END	

-- 68994 If the account is an RF account and has been referred then any other RF accounts for the same customer should also be referred
-- AA UAT Malaysia - unless account fully delivered then don't refer
    IF (SELECT accttype FROM acct WHERE acctno = @AcctNo)  = 'R'
    BEGIN
		UPDATE	proposal
	    SET		PropResult  = 'R',
			    Reason = @reason
        FROM    proposal P INNER JOIN acct A ON P.acctno = A.Acctno
	    WHERE	custid = @custid
	    AND		accttype = 'R'
        AND     propresult = 'A'
		AND NOT EXISTS (SELECT acctno,SUM(transvalue) FROM dbo.fintrans f WHERE f.acctno =a.acctno 
		AND transtypecode IN ('del','grt','add')
		GROUP BY acctno 
		HAVING SUM(transvalue) = a.agrmttotal )
	END

    UPDATE	proposalflag 
	SET		datecleared = null
	WHERE	custid = @CustId
	--AND		dateprop = @DateProp
	AND		checktype = 'R'
	AND acctno=@acctno		-- 19/11/08 jec 70435

	IF(@@rowcount = 0)
	BEGIN
	--set the propresult for the max dateprop for the custID
    	SET @DateProp = ISNULL((SELECT MAX(dateprop) FROM proposalflag WHERE custid = @CustId), @DateProp)
		INSERT
		INTO		proposalflag
				(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
		VALUES	(0, @CustId, @DateProp, 'R', null, @empeeno, @acctno)
	END

	/* if it's being manually referred it may have been cancelled, in which 
	case it needs to be uncancelled */
	DELETE
	FROM		cancellation
	WHERE	acctno = @AcctNo

	IF(@@rowcount = 0)
	BEGIN
		UPDATE	acct
		SET 		acct.agrmttotal = agreement.agrmttotal
		FROM 		agreement
		WHERE	acct.acctno = @AcctNo
		AND		acct.acctno = agreement.acctno
	END

	
	IF NOT EXISTS (SELECT * FROM delauthorise
			       WHERE acctno = @acctno)
	BEGIN
		INSERT INTO delauthorise
		        ( acctno, datein )
		VALUES  ( @acctno, -- acctno - char(12)
				  GETDATE()  -- datein - datetime
		          )
	END

    SET @Return = @@ERROR
    
END

GO  
