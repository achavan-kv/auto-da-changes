SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodeGetByCategorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodeGetByCategorySP]
GO

CREATE PROCEDURE 	dbo.DN_CodeGetByCategorySP
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : DN_CodeGetByCategorySP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : 
-- Date         : ??  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 07/12/11 ip   CR1234 - Retrieve Additional2 column
-- ================================================  
			@category varchar(12),
			@flag varchar(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code,
			codedescript,
			reference,
			isnull(additional, '') as additional, --IP - 11/02/10 - CR1048 (Ref:3.1.2.2/3.1.2.3) Merged - Malaysia Enhancements (CR1072)
			isnull(additional2, '') as additional2	--IP - 07/12/11 - CR1234
	FROM		code
	WHERE	category = @category
	AND		statusflag = @flag
	--AND		code != ''
	order by sortorder, codedescript	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

