SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FactAutoGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FactAutoGetSP]
GO

CREATE PROCEDURE 	dbo.DN_FactAutoGetSP
					@return int OUTPUT

AS


	SELECT	EffectiveDate, 
			FullProduct, 
			ExcludeZeroStock, 
			ProcessEOD,
        	ProcessEOW, 
			ProcessEOP, 
			ProcessCINT
	FROM	FactAuto

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

