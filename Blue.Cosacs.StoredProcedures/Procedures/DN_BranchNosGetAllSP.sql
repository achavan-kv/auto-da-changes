SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchNosGetAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchNosGetAllSP]
GO



CREATE PROCEDURE 	dbo.DN_BranchNosGetAllSP

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : [DN_BranchNosGetAllSP].prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Branch Details for dropdowns etc
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the basic Branch Details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/08/07  jec CR903 changes
-- 23/03/10  ip  CR1072 - Malaysia 3PL for Version 5.2 - Return column which identifies if this is a Third Party Deliveries Warehouse.
-- ================================================
	-- Add the parameters for the stored procedure here


			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	branchno,
	        branchname,
		storetype,
		ScoreHPbefore,
		CreateRFAccounts,				-- CR903 jec
		CreateCashAccounts,				-- CR903 jec
		CreateHPAccounts,				-- CR903 jec
		ThirdPartyWarehouse	,			-- CR1072 - Malaysia 3PL for Version 5.2
		ISNULL(CreateStore,0) as CreateStoreAccounts
	FROM		branch

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

