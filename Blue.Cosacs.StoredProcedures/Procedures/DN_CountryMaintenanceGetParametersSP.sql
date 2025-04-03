SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CountryMaintenanceGetParametersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CountryMaintenanceGetParametersSP]
GO

CREATE PROCEDURE 	dbo.DN_CountryMaintenanceGetParametersSP
			 --@country char(1),
			 --@return int OUTPUT

AS

	--SET 	@return = 0			--initialise return code

	SELECT	ParameterID,
			CountryCode,
			ParameterCategory,
			[Name],
			Value,
			Type,
			[Precision],
			OptionCategory,
			OptionListName,
			[Description],
			CodeName
	FROM		CountryMaintenance
	ORDER BY	ParameterCategory, [Name]

	--IF (@@error != 0)
	--BEGIN
	--	SET @return = @@error
	--END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

