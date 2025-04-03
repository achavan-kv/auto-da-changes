SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSoleOrJointSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSoleOrJointSP]
GO





CREATE PROCEDURE 	dbo.DN_CustomerSoleOrJointSP
			@custid varchar(20),
			@linked varchar(20) OUT,
			@type varchar(1) ,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	TOP 1
			@linked = linked,
			@type = relationship
	FROM		customerlinks
	WHERE	relationship = @type
	AND		holder = @custid

	IF(@@rowcount = 0)
		SET	@return = -1

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

