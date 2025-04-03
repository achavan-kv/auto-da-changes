SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerCodesGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerCodesGetSP]
GO

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerCodesGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve codes on the CustCatCode table for a Customer.
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve codes from the CustCatCode table for a Customer.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/09/09   IP  5.2 UAT(823) - Only return codes that currently apply to the Customer.
-- ================================================



CREATE PROCEDURE 	dbo.DN_CustomerCodesGetSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@custid = custid 
	FROM		customer 
	WHERE	custid = @custid
		
	IF(@@rowcount >0)
	BEGIN
		SELECT	A.code as Code,
				A.datecoded as 'Date Added',
				A.empeenocode as 'Added By',
				C.codedescript as Description,
				A.reference as 'Reference'
		FROM		custcatcode A, code C
		WHERE	A.custid = @custid
		AND		A.code = C.code
		AND		A.datedeleted is NULL --IP - 01/09/09 - UAT(823)
		AND		C.category in ('CC1', 'CC2')
	END
	ELSE
	BEGIN
		SET	@return = -1
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

