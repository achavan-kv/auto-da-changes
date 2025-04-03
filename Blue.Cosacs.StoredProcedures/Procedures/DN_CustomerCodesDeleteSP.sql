SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerCodesDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerCodesDeleteSP]
GO

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerCodesDeleteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delete a code from the CustCatCode table for a Customer.
-- Author       : ??
-- Date         : ??
--
-- This procedure will delete a code from the CustCatCode table for a Customer.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/09/09   IP  5.2 UAT(823) - Update the datedeleted for the code being deleted as previously was deleting the records.
-- ================================================


CREATE PROCEDURE 	dbo.DN_CustomerCodesDeleteSP
			@custid varchar(20),
			@custCode varchar(4), --IP - 01/09/09 - 5.2 UAT(823)
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--DELETE
	--FROM		custcatcode
	--WHERE	custid = @custid
	
	UPDATE custcatcode
	SET datedeleted = getdate()
	WHERE custid = @custid
	AND code = @custCode
	AND datedeleted is NULL

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

