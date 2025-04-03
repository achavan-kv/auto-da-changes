SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagGetUnclearedStageSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagGetUnclearedStageSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagGetUnclearedStageSP]    Script Date: 11/05/2007 12:02:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE 	[dbo].[DN_ProposalFlagGetUnclearedStageSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalFlagGetUnclearedStageSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Proposal Flag Get Uncleared Stage
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve uncleared flags
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/02/10 jec CR1072 Malaysia v4 merge
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@checktype varchar(4) OUT,
			@dateprop smalldatetime OUT,	
			@acctnoOut varchar(12) OUT,
			@propresult varchar(4) OUT,
			@points int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @custid varchar(20)
	DECLARE @accttype char(1)

--get the account type
	SELECT	@accttype = AT.accttype
	FROM		accttype AT, acct A
	WHERE	A.acctno = @acctno
	AND		A.accttype = AT.genaccttype

--get the customer ID
	SELECT	@custid = custid 
	FROM		custacct
	WHERE	acctno = @acctno
	AND		hldorjnt = 'H'
--SELECT @accttype 
	IF(@accttype = 'R')
	BEGIN
			--get the most recent proposal for this customer for an RF account
		SELECT	TOP 1
				@dateprop = P.dateprop,
				@acctnoOut = P.acctno,
				@propresult = P.propresult
		FROM		custacct CA INNER JOIN acct A
		ON		CA.acctno = A.acctno  INNER JOIN proposal P
		ON		CA.custid = P.custid 
		AND		CA.acctno = P.acctno
		AND		CA.acctno = @acctno		--CR1072 - LW70131  jec
		WHERE	CA.custid = @custid
		AND		A.accttype = 'R'
		ORDER BY	P.dateprop DESC
		--SELECT @acctnoOut
	END
	ELSE		--this will be an HP account therefore just get the most recent proposla for this account
	BEGIN
		SELECT	TOP 1 
		        @points = isnull(p.points,0),
				@dateprop = P.dateprop,
				@acctnoOut = P.acctno,
				@propresult = P.propresult
		FROM		proposal P
		WHERE	P.acctno = @acctno
		ORDER BY	P.dateprop DESC
	END

--get the checktype of the first uncleared flag
	SELECT	TOP 1 
			@checktype = p.checktype
	FROM		proposalflag p
	INNER JOIN 	stage s on p.checktype = s.checktype
	WHERE 	p.acctno = @acctno
	AND 		datecleared is null
	ORDER BY	 s.id asc

--format the context menu string according to the checktype
	IF(@checktype = '')
		SET @checktype = 'S1'

	/* don't set the text in the stored proc because then it can't be translated 
	also wrong to set to S1 unless S1 is open becuase rescoring may then be done
	at the wrong times

	IF(@checktype = 'S1')
		SET @stage = 'Sanction Stage 1'
	IF(@checktype = 'S2')
		SET @stage = 'Sanction Stage 2'
	IF(@checktype = 'DC')
		SET @stage = 'Document Confirmation'
	IF(@checktype = 'AD')
	BEGIN
		SET @stage = 'View Proposal'
		SET @checktype = 'S1'
	END
	IF(@checktype = 'R')
	BEGIN
		SET @stage = 'View Proposal'
		SET @checktype = 'S1'
	END
	*/
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
