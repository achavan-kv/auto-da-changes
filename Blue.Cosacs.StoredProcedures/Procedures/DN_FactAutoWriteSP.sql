SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FactAutoWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FactAutoWriteSP]
GO

CREATE PROCEDURE 	dbo.DN_FactAutoWriteSP
					@effectivedate datetime,
					@fullproduct char(1),
					@excludezerostock char(1),
					@processEOD char(1),
					@processEOW char(1),
					@processEOP char(1),
					@processCINT char(1),
					@return int OUTPUT

AS

	DECLARE @effdate varchar(10)

	SET 	@return = 0			--initialise return code
	SET 	@effdate = CONVERT(VARCHAR(10), @effectivedate, 103)	

	DELETE
	FROM	FactAuto

	INSERT	
	INTO	FactAuto(EffectiveDate, FullProduct, ExcludeZeroStock, ProcessEOD,
        			 ProcessEOW, ProcessEOP, ProcessCINT, DateSaved)
	VALUES	(@effdate, @fullproduct, @excludezerostock, @processEOD,
			 @processEOW, @processEOP, @processCINT, GETDATE())

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

