SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- This sproc has been renamed
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineitemAccountAddedToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineitemAccountAddedToSP]
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_HasAddToOrDeliverySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_HasAddToOrDeliverySP]
GO

CREATE PROCEDURE 	dbo.DN_HasAddToOrDeliverySP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/08/11  IP  RI - System Integration changes
-- =============================================
			@acctno varchar(12),
			@addto smallint OUT,
			@return int OUTPUT

AS

	SET @return = 0
	SET	@addto = 0

    -- Cannot Add-To an account already with an ADDDR item
	SELECT 	@addto = count(*)
	--FROM	lineitem 
	FROM	lineitem l inner join stockinfo si on l.ItemID = si.ID				--IP - 03/08/11 - RI
	WHERE	acctno = @acctno
	--AND		itemno = 'ADDDR'
	AND		isnull(si.iupc,'') = 'ADDDR'										--IP - 03/08/11 - RI
	
	-- Cannot Add-To an account with any items scheduled or delivered
	IF (@addto = 0)
	BEGIN
	    -- Check for any items delivered
		SELECT 	@addto = count(*)
		FROM	delivery d
		WHERE	d.acctno = @acctno
	END
		
	IF (@addto = 0)
	BEGIN
	    -- Check for any items scheduled
	    SELECT 	@addto = count(*)
		FROM	schedule s
		WHERE	s.acctno = @acctno	
	END

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

