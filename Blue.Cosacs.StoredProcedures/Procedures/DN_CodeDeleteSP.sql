SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodeDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodeDeleteSP]
GO

CREATE PROCEDURE dbo.DN_CodeDeleteSP @category VARCHAR(12), 
				     @code VARCHAR(12),
				     @return int OUTPUT			     
AS
	DELETE FROM [dbo].[code]
	WHERE [category] = @category
	AND [code] = @code

	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

