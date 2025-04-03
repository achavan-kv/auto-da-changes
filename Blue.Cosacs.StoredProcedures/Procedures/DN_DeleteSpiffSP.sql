

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeleteSpiffSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeleteSpiffSP]
GO

CREATE PROCEDURE 	dbo.DN_DeleteSpiffSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeleteSpiffSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DeleteSpiff Items
-- Author       : Pa
-- Date         : ??
--
-- This procedure retrieve linked items that have spiffs.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/07/11  jec CR1254 RI Changes
-- ================================================
	-- Add the parameters for the stored procedure here
				@acctno varchar(12),
				@itemno varchar(18),				-- RI
				@stocklocn smallInt,
				@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM	SalesCommissionExtraSpiffs
	WHERE	AcctNo = @acctno
	AND		ItemNo = @itemNo
	AND		StockLocn = @stocklocn

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

