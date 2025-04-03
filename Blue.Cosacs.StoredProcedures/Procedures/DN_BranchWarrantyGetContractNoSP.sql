SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchWarrantyGetContractNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchWarrantyGetContractNoSP]
GO

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2002 Strategic Thought Ltd.
-- File Name   : DN_BranchWarrantyGetContractNoSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title           : Add auto contract number for warranty in New Account
-- Author       : Rupal Desai
-- Date         : 10 December 2002
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
--
--------------------------------------------------------------------------------

CREATE PROCEDURE dbo.DN_BranchWarrantyGetContractNoSP
			@branchno   	SMALLINT,
   			@contractno 	INT OUTPUT,
			@return		INT OUTPUT
 
AS

BEGIN	

	SET 	@return = 0

	DECLARE	@warrantyno VARCHAR(10)	

 	SET  @contractno = 0   --initialise return code
	
	UPDATE branchwarranty
	SET warrantyno = warrantyno + 1 
	WHERE branchno = @branchno

    	SELECT @contractno = warrantyno 
	FROM branchwarranty
	 WHERE branchno = @branchno

	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

