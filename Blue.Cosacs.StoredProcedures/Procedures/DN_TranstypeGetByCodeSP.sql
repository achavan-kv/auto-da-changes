SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TranstypeGetByCodeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TranstypeGetByCodeSP]
GO

CREATE PROCEDURE 	dbo.DN_TranstypeGetByCodeSP
			@transtypecode varchar(3),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	transtypecode,
			description,
			includeingft,
			interfacesecaccount,
			interfaceaccount,
			branchsplit,
			isdeposit,
			interfacebalancing,
			empeenochange,
			referencemandatory,
			referenceunique
	FROM		Transtype where batchtype='FIN' or isdeposit > 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

