SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchUpdateSP]
GO

CREATE PROCEDURE [DN_BranchUpdateSP] 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : [DN_BranchUpdateSP].prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update Branch Details
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the basic Branch Details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/08/07  jec CR903 changes
-- 23/02/10  ip  CR1072 - Malaysia 3PL for Version 5.2 - Save defaultPrintLocation, and ThirdPartyWarehouse
-- 21/10/10  jec UAT88  - Branch Maintenance error.
-- ================================================
	-- Add the parameters for the stored procedure here

(
	@branchno smallint, 
	@branchname varchar(20), 
	@branchaddr1 varchar(26), 
	@branchaddr2 varchar(26), 
	@branchaddr3 varchar(26), 
	@branchpocode varchar(10),  
	@telno varchar(13), 
	@countrycode char(2), 
	@croffno int, 
	@oldpctype char(1),
	@newpctype char(1), 
	@datepcchange datetime, 
	@hissn int, 
	@hibuffno int, 
	@warehouseno varchar(3),
	@hirefno int, 
	@as400branchno smallint, 
	@region varchar(3), 
	@depositscreenlocked bit, 
	@warehouseregion varchar(12), 
	@Fact2000BranchLetter char(1),
	@storeType char(1),@createRF bit,@createCash bit,@scoreHPbefore bit,@createHP bit,		-- CR903
	@serviceRepairCentre BIT, -- CR 1056
	@behavioural BIT,		--UAT88 jec 21/10/10 reinstated	--IP - 08/04/10 - CR1034 - Removed
	@defaultPrintLocation INT = null, --CR 1072 - Malaysia 3PL for Version 5.2
	@isThirdPartyWarehouse CHAR(1), --CR 1072 - Malaysia 3PL for Version 5.2
	@createStore bit,
    @isCashLoanBranch bit,
    @luckyDollarStore bit = null,
    @ashleyStore bit = null,
	@return int output
)
AS

	IF EXISTS (SELECT 1 FROM [branch] WHERE [branchno] = @branchno)
	BEGIN
		UPDATE [branch]
		SET [branchname]=@branchname,
		    [branchaddr1]=@branchaddr1,
	            [branchaddr2]=@branchaddr2,
	            [branchaddr3]=@branchaddr3,
	            [branchpocode]=@branchpocode,
	            [telno]=@telno,
	            [countrycode]=@countrycode,
	            [croffno]=@croffno,
	            [oldpctype]=@oldpctype,
	            [newpctype]=@newpctype,
	            [datepcchange]=@datepcchange,
	            [hissn]=@hissn,
	            [hibuffno]=@hibuffno,
	            [warehouseno]=@warehouseno, 
	            [hirefno]=@hirefno,
	            [as400branchno]=@as400branchno,
	            [region]=@region,
				depositscreenlocked = @depositscreenlocked,
				warehouseregion = @warehouseregion,
				Fact2000BranchLetter = @Fact2000BranchLetter,
				storeType=@storeType,						-- CR903
				CreateRFAccounts=@createRF,					-- CR903
				CreateCashAccounts=@createCash,				-- CR903
				ScoreHPbefore=@scoreHPbefore,				-- CR903
				CreateHPAccounts=@createHP,					-- CR903
				ServiceRepairCentre = @serviceRepairCentre, -- CR1056
				BehaviouralScoring = @behavioural,		--UAT88 jec 21/10/10 reinstated IP - 08/04/10 - CR1034 - Removed
				defaultPrintLocation = @defaultPrintLocation, --CR1072 - Malaysia 3PL for Version 5.2
				ThirdPartyWarehouse = @isThirdPartyWarehouse,  --CR1072 - Malaysia 3PL for Version 5.2
				createstore = @createstore,
                CashLoanBranch = @isCashLoanBranch,
                LuckyDollarStore = @luckyDollarStore,
                AshleyStore = @ashleyStore
		WHERE [branchno] = @branchno
	END
	ELSE
	BEGIN
		INSERT INTO [branch] ([branchno], [branchname], [branchaddr1], [branchaddr2],
		 	          [branchaddr3], [branchpocode], [telno], [countrycode],
	            	  [croffno], [oldpctype], [newpctype], [datepcchange],
				      [hissn], [hibuffno], [warehouseno], [hirefno], [as400branchno],
	 		          [region], [batchctrlno], [weekno], [servpcent], depositscreenlocked, warehouseregion, Fact2000BranchLetter,
	 		          storeType, CreateRFAccounts, CreateCashAccounts, ScoreHPbefore, CreateHPAccounts, ServiceRepairCentre,BehaviouralScoring, --UAT88 jec 21/10/10 reinstated IP - 08/04/10 - CR1034 - Removed, -- CR903
	 		          defaultPrintLocation, ThirdPartyWarehouse,createstore, CashLoanBranch,
                      LuckyDollarStore, AshleyStore)--CR1072 - Malaysia 3PL for Version 5.2			
		VALUES (@branchno, @branchname, @branchaddr1, @branchaddr2, @branchaddr3, @branchpocode,
	                @telno, @countrycode, @croffno, @oldpctype, @newpctype, @datepcchange, @hissn,
	                @hibuffno, @warehouseno, @hirefno, @as400branchno, @region, 0, 0, 0, @depositscreenlocked, @warehouseregion, @Fact2000BranchLetter,
	                @storeType, @createRF, @createCash, @scoreHPbefore, @createHP, @serviceRepairCentre,@behavioural, --UAT88 jec 21/10/10 reinstated IP - 08/04/10 - CR1034 - Removed, -- CR903
	                @defaultPrintLocation, @isThirdPartyWarehouse, @createstore, @isCashLoanBranch,
                    @luckyDollarStore, @ashleyStore)	--CR1072 - Malaysia 3PL for Version 5.2		
	END
	
	IF @@error = 0
	BEGIN
		IF EXISTS (SELECT 1 FROM [branchregion] WHERE [branchno] = @branchno)
		BEGIN
			UPDATE [branchregion]
			SET [region]= @warehouseregion
			WHERE [branchno] = @branchno
		END
		ELSE
		BEGIN
			INSERT INTO [branchregion] ([branchno], [region])
			VALUES (@branchno, @warehouseregion)
		END
	END	
	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End