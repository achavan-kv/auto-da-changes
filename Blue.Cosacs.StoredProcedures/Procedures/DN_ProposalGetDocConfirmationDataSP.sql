SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetDocConfirmationDataSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetDocConfirmationDataSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalGetDocConfirmationDataSP
			@acctno varchar(12),
			@custid varchar(20),
			@dateprop smalldatetime,
			@return int OUTPUT
-- **********************************************************************
-- Title: DN_ProposalGetDocConfirmationDataSP.sql
-- Developer: ?
-- Date: ?
-- Purpose: Retrieve data from Proposal to populate the Document Confirmation screen

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/12/10  IP  Select two new columns added for Store Card - ProofOfBank
--				 and ProofOfBankTxt
-- **********************************************************************

AS

	SET 	@return = 0			--initialise return code

	SELECT	P.custid as 'CustomerID',
			C.firstname as 'FirstName',
			C.name as 'LastName',
			P.dateprop as 'DateProp',
			P.ProofAddress as 'ProofOfAddress',
			P.ProofId as 'ProofOfID',
			P.ProofIncome as 'ProofOfIncome',
			p.ProofOfBank,						--IP - 14/12/10 - Store Card
			P.acctno as 'Account No.',
			P.DCText1,
			P.DCText2,
			P.DCText3,
			p.ProofOfBankTxt					--IP - 14/12/10 - Store Card
	FROM		customer C INNER JOIN
			proposal P ON
			C.custid = P.custid
	WHERE	C.custid = @custid
	AND		P.acctno = @acctno
	AND		P.dateprop = @dateprop

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

