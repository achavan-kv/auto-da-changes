-- =============================================
-- Author:		Ilyas Parker
-- Create date: 2007-Oct-02
-- Description:	Select details from the 'Courtspersonauditlog' table.
-- Modification:
--				
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
-- =============================================

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeGetLogonHistorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeGetLogonHistorySP]
GO

CREATE PROCEDURE 	dbo.DN_EmployeeGetLogonHistorySP
			@empeeno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	*
	FROM	Courtspersonauditlog
	WHERE	empeeno = @empeeno
	ORDER BY dateaction Desc

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO