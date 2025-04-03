SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalSetResultSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalSetResultSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalSetResultSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalSetResultSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11  IP  5.13 - LW73619 - #3751 - If holdprop = 'N' (accounts sanction stage 1 was re-opened and not revised
--				 need to remove entry from delauthorise which would mean it will no longer appear in Incomplete Credit screen
-- 28/11/11 jec  Set CashLoan.LoanStatus if referral accepted
--------------------------------------------------------------------------------
			@custid varchar(20),
			@acctno varchar(12),
			@dateprop smalldatetime,
			@notes varchar(1000),
			@propResult char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	proposal
	SET		propnotes = @notes,
			propresult = @propResult,
			datechange = getdate()
	WHERE	custid = @custid
	AND		acctno = @acctno
	AND		dateprop = @dateprop

-- 68994 If the account is an RF account and has been approved then any other RF accounts for the same customer (currently referred) should also be approved
    IF (SELECT accttype FROM acct WHERE acctno = @acctno)  = 'R' AND @propResult = 'A'
    BEGIN
		UPDATE	proposal
	    SET		propnotes = @notes,
			    propresult = @propResult,
				datechange = getdate()
        FROM    proposal P INNER JOIN acct A ON P.acctno = A.Acctno
	    WHERE	custid = @custid
	    AND		accttype = 'R'
        AND     propresult = 'R'
	END

	IF (SELECT accttype FROM acct WHERE acctno = @acctno)  = 'R' AND @propResult = 'R'
    	BEGIN
		UPDATE	proposal
	    SET		propnotes = @notes,
			    propresult = @propResult,
				datechange = getdate()
        FROM    proposal P INNER JOIN acct A ON P.acctno = A.Acctno
	    WHERE	custid = @custid
	    AND		accttype = 'R'
        AND     propresult = 'A'
	END
	
	--IP - 5.13 - LW73619 - #3751 - If holdprop = 'N' (accounts sanction stage 1 was re-opened and not revised
	--need to remove entry from delauthorise which would mean it will no longer appear in Incomplete Credit screen
	declare @holdprop char(1)		
	
	select @holdprop = holdprop from agreement where acctno = @acctno
	
	if(@propresult = 'A' and @holdprop = 'N')
	begin
		exec dbremoveauth @acctno
	end
	
	if(@propresult = 'A')
	begin
		-- Update Cashloan status  - #8744	
		UPDATE CashLoan
		set LoanStatus=' '
		where AcctNo=@acctno and LoanStatus='C'
	end

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End