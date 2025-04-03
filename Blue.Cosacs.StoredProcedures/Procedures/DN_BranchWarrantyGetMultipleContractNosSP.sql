SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchWarrantyGetMultipleContractNosSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchWarrantyGetMultipleContractNosSP]
GO

CREATE PROCEDURE dbo.DN_BranchWarrantyGetMultipleContractNosSP
			@branchno   	SMALLINT,
   			@number 	INT,
			@last		INT OUTPUT,
			@return		INT OUTPUT
 
AS

BEGIN	

	SET 	@return = 0

	UPDATE 	branchwarranty
	SET 		warrantyno = warrantyno + @number
	WHERE	branchno = @branchno

    	SELECT 	@last = warrantyno 
	FROM 		branchwarranty
	WHERE 	branchno = @branchno

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

