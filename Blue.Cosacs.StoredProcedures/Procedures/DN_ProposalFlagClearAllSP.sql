SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagClearAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagClearAllSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagClearAllSP]    Script Date: 11/05/2007 11:47:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE  [dbo].[DN_ProposalFlagClearAllSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalFlagClearAllSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Clear all proposal flags
-- Author       : ??
-- Date         : ??
--
-- This procedure will clear all proposal flags that are not already clear.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/11/07  jec UAT 112 fix
-- ================================================
	-- Add the parameters for the stored procedure here

			@empeeno INT,
			@datecleared datetime,
			@acctno char(12),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

	UPDATE	proposalflag 
	SET	datecleared	= @datecleared,
		empeenopflg	= @empeeno
	WHERE	acctno = @acctno
		and datecleared is null			-- jec 20/11/07

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End