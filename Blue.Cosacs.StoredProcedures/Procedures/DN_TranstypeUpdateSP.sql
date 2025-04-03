SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TranstypeUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TranstypeUpdateSP]
GO


CREATE PROCEDURE 	dbo.DN_TranstypeUpdateSP
-- **********************************************************************
-- Title: DN_TranstypeUpdateSP.PRC
-- Developer: ??
-- Date: ??
-- Purpose: Update TransType table

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/04/12	 IP  CR9863 - #9885 - Save Store Card Debtors account and
--				 Store Card Balancing account
-- **********************************************************************
			@transtypecode varchar(3),
			@tccodedr int,
			@tccodecr int,
			@description varchar(40),
			@balordue char(1),
			@exportfilesuffix char(1),
			@batchtype varchar(3),
			@includeingft smallint,
			@interfacesecaccount varchar(10),
			@interfaceaccount varchar(10),
			@branchsplit smallint,
			@isdeposit smallint,
			@interfacebalancing varchar(10),
			@ismandatory smallint,
			@isunique smallint,
			@user int,
			@interfacesecbalancing varchar(10),
			@branchsplitbalancing smallint,	
			@scInterfaceAccount varchar(10),						--IP - 11/04/12 - CR9863 - #9885
			@scInterfaceBalancing varchar(10),						--IP - 11/04/12 - CR9863 - #9885
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE 	Transtype
			set description		= @description,
			includeingft 		= @includeingft,
			interfacesecaccount 	= @interfacesecaccount,
			interfaceaccount 	= @interfaceaccount,
			branchsplit 		= @branchsplit,
			isdeposit 		= @isdeposit,
			interfacebalancing 	= @interfacebalancing,
			empeenochange	= @user,
			referencemandatory	= @ismandatory,
			referenceunique		= @isunique,
			interfacesecbalancing 	= @interfacesecbalancing,
			branchsplitbalancing	= @branchsplitbalancing,
			SCInterfaceAccount	= @scInterfaceAccount,				--IP - 11/04/12 - CR9863 - #9885
			SCInterfaceBalancing = @scInterfaceBalancing			--IP - 11/04/12 - CR9863 - #9885

	WHERE	transtypecode = @transtypecode

	IF(@@rowcount = 0)
	BEGIN
		INSERT INTO transtype
		(
			transtypecode,
			tccodedr,
			tccodecr,
			description,
			balordue,
			exportfilesuffix,
			batchtype,
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
			SCInterfaceAccount,										--IP - 11/04/12 - CR9863 - #9885
			SCInterfaceBalancing									--IP - 11/04/12 - CR9863 - #9885
		)
		VALUES
		(
			@transtypecode,
			@tccodedr,
			@tccodecr,
			@description,
			@balordue,
			@exportfilesuffix,
			@batchtype,
			@includeingft,
			@interfacesecaccount,
			@interfaceaccount,
			@branchsplit,
			@isdeposit,
			@interfacebalancing,
			@user,
			@ismandatory,
			@isunique,
			@interfacesecbalancing,
			@branchsplitbalancing,
			@scInterfaceAccount,									--IP - 11/04/12 - CR9863 - #9885
			@scInterfaceBalancing									--IP - 11/04/12 - CR9863 - #9885
		)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

