SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerCodeAddSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerCodeAddSP]
GO

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerCodeAddSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save details to CustCatCode
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the details to the CustCatCode table.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/09/09   IP  5.2 UAT(823) - If the code being added exists then update the datedeleted to null
-- ================================================




CREATE PROCEDURE 	dbo.DN_CustomerCodeAddSP
			@custid varchar(20),
			@code varchar(4),
			@date datetime,
			@codedby int,
			@reference varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	custcatcode
	SET		custid = @custid,
			code = @code,
			datecoded = @date,
			empeenocode = @codedby,
			reference = @reference,
			datedeleted = null --IP - 01/09/09 - 5.2 UAT(823)
	WHERE	custid = @custid
	AND		code = @code

	IF(@@rowcount = 0)
	BEGIN
		INSERT INTO custcatcode
		(custid, code, datecoded, empeenocode, reference)
		VALUES (@custid, @code, @date, @codedby, @reference)
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

