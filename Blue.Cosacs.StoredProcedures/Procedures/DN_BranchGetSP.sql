SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchGetSP]
GO

CREATE PROCEDURE DN_BranchGetSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_BranchGetSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Branch Details
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the basic Branch Details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/08/07  jec CR903 changes
-- 23/02/10  ip  CR1072 - Malaysia 3PL for Version 5.2 - 
-- 21/10/10  jec UAT88 - Branch Maintenance error.
-- ================================================
	-- Add the parameters for the stored procedure here
			@branchno SMALLINT, 
			@return INT OUTPUT


AS

	SELECT	branchno,
			branchname,
			branchaddr1,
		       	ISNULL(branchaddr2,'') as 'branchaddr2',
			ISNULL(branchaddr3,'') as 'branchaddr3',
			ISNULL(branchpocode,'') as 'branchpocode',
		       	ISNULL(telno,'') as 'telno',
			servpcent,
			countrycode,
			croffno,
			daterun,
			weekno,
			oldpctype,
			newpctype,
		       	ISNULL(datepcchange,'1/1/1900') as 'datepcchange',
			batchctrlno,
			hissn,
			hibuffno,
			warehouseno,
			ISNULL(as400exp,'') as 'as400exp',
			hirefno,
			as400branchno,
			codreceipt,
			region,
			ServiceLocation,
			defaultdeposit,
			depositscreenlocked,
			warehouseregion,
			Fact2000BranchLetter,
			StoreType,							--CR903		jec 03/08/07
			CreateRFAccounts,					--CR903		jec 03/08/07
			CreateCashAccounts,					--CR903		jec 03/08/07
			ScoreHPbefore,						--CR903		jec 03/08/07
			CreateHPAccounts, 					--CR903		jec 03/08/07
			ServiceRepairCentre,					--CR1056
			--BehaviouralScoring,				--IP - 08/04/10 - CR1034 - Removed
			ISNULL(defaultPrintLocation,0) as defaultPrintLocation,				-- UAT88
			ThirdPartyWarehouse,					--IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
			BehaviouralScoring,					-- UAT88 
			ISNULL(CreateStore,0) as CreateStoreAccounts,
            CashLoanBranch,
            ISNULL(LuckyDollarStore,0) as LuckyDollarStore,
            ISNULL(AshleyStore,0) as AshleyStore
	FROM 		branch
	WHERE 	branchno = @branchno

	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

