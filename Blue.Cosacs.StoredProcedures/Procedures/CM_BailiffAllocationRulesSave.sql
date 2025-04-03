
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_BailiffAllocationRulesSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_BailiffAllocationRulesSave]
GO

CREATE PROCEDURE [dbo].[CM_BailiffAllocationRulesSave]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CM_BailiffAllocationRulesSave.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Bailiff Allocation Rules
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the Bailiff Allocation Rules.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- =================================================================================
	-- Add the parameters for the stored procedure here
		   @return INTEGER OUT,
		   @empeeno INT,
		   @empeetype VARCHAR(2),
		   @BranchorZone VARCHAR(10),
		   @IsZone BIT,
		   @AllocationOrder SMALLINT,
		   @empeenochange INT, 
		   @datechange DATETIME,
		   @reallocate bit
AS 


	INSERT INTO CMBailiffAllocationRules
	 (empeeno,
		   empeetype,
		   BranchorZone,
		   IsZone,
		   AllocationOrder,
		   empeenochange,
		   datechange,
		   reallocate) 
    VALUES (@empeeno, 
	@empeetype,
	@BranchorZone,
	@IsZone,
	@AllocationOrder,
	@empeenochange,
	@datechange,
	@reallocate)
	
	SET @return = @@ERROR

	RETURN @return

go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End

