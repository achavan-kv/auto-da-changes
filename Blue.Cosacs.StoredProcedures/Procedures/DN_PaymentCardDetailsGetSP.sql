SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentCardDetailsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentCardDetailsGetSP]
GO


CREATE PROCEDURE 	dbo.DN_PaymentCardDetailsGetSP
			@custid varchar(20),
			@acctno varchar(12),
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	declare	@available_credit money
	declare	@cardprinted char(1)
	declare	@total_arrears money
	declare @total_balances money
	declare	@total_credit money
	declare	@total_delivered_instalments money

	declare	@agreementTotal money
	declare	@instalAmount money
	declare @deposit money
	declare @address1 varchar(50)
	declare @address2 varchar(50)
	declare @address3 varchar(50)
	declare @postCode varchar(10)
	declare @title varchar(25)	
	declare @name varchar(60)
	declare @dateFirst datetime
	declare @newPCType char(1)
	declare @datedel datetime

	declare @acctType char(1)

	SET	@deposit = 0

	--select the account type for this account
	SELECT	@acctType = AT.accttype
	FROM	accttype AT INNER JOIN
		acct A
	ON	AT.genaccttype = A.accttype
	WHERE	A.acctno = @acctno

	IF(@acctType = 'R')
	BEGIN
		EXEC	DN_CustomerGetRFCombinedForPrintSP @custid = @custid,
							@available_credit = @available_credit OUT,
							@cardprinted = @cardprinted OUT,
							@total_agreements = @agreementTotal OUT,
							@total_arrears = @total_arrears OUT,
							@total_balances = @total_balances OUT,
							@total_credit = @total_credit OUT,
							@total_delivered_instalments = @total_delivered_instalments OUT,
							@total_all_instalments = @instalAmount OUT,
							@return = @return OUT
		SELECT	TOP 1
				@dateFirst = I.datefirst,
				@datedel = datedel
		FROM		acct A INNER JOIN instalplan I
		ON		A.acctno = I.acctno  INNER JOIN 
				custacct CA
		ON		CA.acctno = A.acctno INNER JOIN 
				agreement AG
		ON		A.acctno = AG.acctno
		WHERE		CA.custid = @custid
		AND		A.accttype = 'R'
		AND		CA.hldorjnt = 'H'
		ORDER BY	A.dateacctopen ASC	
	END
	ELSE		--all other account types
	BEGIN
		SELECT		@instalAmount = I.instalamount,
				@agreementTotal = A.agrmttotal,
				@dateFirst = I.datefirst,
				@deposit = AG.deposit,
				@datedel = AG.datedel
		FROM		acct A INNER JOIN instalplan I
		ON		A.acctno = I.acctno INNER JOIN agreement AG
		ON		A.acctno = AG.acctno	
		WHERE	A.acctno = @acctno
	END

	--select the customer information
	SELECT	@name = C.name,
			@title = C.title,
			@address1 = CA.cusaddr1,
			@address2 = CA.cusaddr2,
			@address3 = CA.cusaddr3,
			@postCode = CA.cuspocode
	FROM		customer C INNER JOIN custaddress CA
	ON		C.custid = CA.custid 
	WHERE	C.custid = @custid 
	AND		CA.addtype = 'H'

	--select the payment card type
	SELECT	@newPCType = newpctype
	FROM		branch
	WHERE	branchno = @branchno


	--select all the required parameters
	SELECT	@title as 'Title',
		@name as 'LastName',
		@address1 as 'Address1',
		@address2 as 'Address2',
		@address3 as 'Address3',
		@postCode as 'PostCode',
		@newPCType as 'NewPCType', 
		@instalAmount as 'Instal Amount',
		@agreementTotal as 'AgreementTotal',
		@deposit as 'Deposit',
		@dateFirst as 'DateFirst',
		@total_credit as creditlimit,
		@datedel as datedel,
		@acctType as accttype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

