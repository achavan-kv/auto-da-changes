SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetProductCategoriesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetProductCategoriesSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetProductCategoriesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	c.category,
			c.code,
			c.codedescript,
			cc.catdescript
	FROM	code c,codecat cc
	WHERE	c.category IN
			('PCE','PCF','PCW','PCO','PCD')
	  AND   cc.category = c.category					

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

