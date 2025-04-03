SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalSaveDocConfirmationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalSaveDocConfirmationSP]
GO


-- **********************************************************************
-- Title: DN_ProposalSaveDocConfirmationSP.sql
-- Developer: ?
-- Date: ?
-- Purpose: Saves changes from Document Confirmation

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/12/10  IP  Update ProofOfBank abd ProofOfBankTxt for Store Card accounts
--				 in the Document Confirmation screen.
-- 11/06/13  IP  #13821 - Proposal is cleared in BProposal.SaveDocConfirmation
-- **********************************************************************

CREATE PROCEDURE 	dbo.DN_ProposalSaveDocConfirmationSP
			@custid varchar(20),
			@acctno varchar(12),
			@dateprop smalldatetime,
			@proofID char(4),
			@proofAddress char(4),
			@proofIncome char(4),					
			@proofOfBank char(4),						--IP - 14/12/10 - Store Card
			@dctext1 varchar(350),
			@dctext2 varchar(350),
			@dctext3 varchar(350),
			@proofOfBankTxt varchar(350),				--IP - 14/12/10 - Store Card
			@empeeno int,
			@return int OUTPUT


AS


	DECLARE @deposit money,
			@instalment money,
			@InstantCredit varchar(1),
			@agreementtotal money,
			@outstbal money,
			@instalpredel varchar(1),
			@paid money,
			@dateauth smalldatetime

	SET 	@return = 0			--initialise return code
	SET		@dateauth = getdate()

	UPDATE	proposal
	SET		ProofId = @proofID,
			ProofAddress = @proofAddress,
			ProofIncome = @proofIncome,
			ProofOfBank = @proofOfBank,				--IP - 14/12/10 - Store Card
			DCText1 = @dctext1,
			DCText2 = @dctext2,
			DCText3 = @dctext3,
			ProofOfBankTxt = @proofOfBankTxt		--IP - 14/12/10 - Store Card
	WHERE	acctno = @acctno
	AND		custid = @custid 
	AND		dateprop = @dateprop


	select	@deposit = ag.deposit,
			@instalment = i.instalamount,
			@InstantCredit = i.InstantCredit,
			@agreementtotal = a.agrmttotal,
			@outstbal= a.outstbal,
			@instalpredel=t.instalpredel

	From agreement ag inner join instalplan i on ag.acctno=i.acctno 
						inner join acct a on i.acctno=a.acctno inner join termstype t on a.termstype=t.termstype
		where ag.acctno=@acctno
			and ag.agrmtno=1

	declare @cheqdays int
	
	select @cheqdays = cheqdays
	from country

	set @paid = (
					select isnull(sum(-transvalue), 0)
					from fintrans
					where acctno = @acctno
						and transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX')
						AND datetrans + @cheqdays > getdate()
						AND	(paymethod%10) = 2 
				)
	set @paid = @paid + 
				(
					select isnull(sum(-transvalue), 0)
					from fintrans
					where acctno = @acctno
						and transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX')
						AND	(paymethod%10) != 2 
				)


	
	IF(@instalpredel = 'Y')
		set @deposit = @deposit + @instalment
		
	--#13821
	--IF(@deposit <= @paid and @instantcredit = 'Y')
	--BEGIN
		--exec  DN_ProposalClearSP @acctno = @acctno, @empeeno = @empeeno, @source = 'Auto', @return = @return out
	--END


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

