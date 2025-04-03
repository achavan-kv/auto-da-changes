SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TranstypeLoadCodesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TranstypeLoadCodesSP]
GO

CREATE PROCEDURE 	dbo.DN_TranstypeLoadCodesSP
-- **********************************************************************
-- Title: DN_TranstypeLoadCodesSP.PRC
-- Developer: ??
-- Date: ??
-- Purpose: Return Trans Type codes to be displayed in the Transaction Type Maintenance screen

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/04/12	 IP  CR9863 - #9885 - Return Store Card Debtors account and
--				 Store Card Balancing account
-- **********************************************************************
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
			referenceunique,
			interfacesecbalancing,
			branchsplitbalancing,
			SCInterfaceAccount,								--IP - 11/04/12 - CR9863 - #9885
			SCInterfaceBalancing							--IP - 11/04/12 - CR9863 - #9885
	FROM		Transtype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

