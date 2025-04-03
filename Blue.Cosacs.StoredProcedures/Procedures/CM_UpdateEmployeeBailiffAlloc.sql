SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_UpdateEmployeeBailiffAlloc]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_UpdateEmployeeBailiffAlloc]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 18/02/2009
-- This will update the courtsperson columns related to BailiffZoneAllocation
-- =============================================
CREATE PROCEDURE [dbo].[CM_UpdateEmployeeBailiffAlloc] 
    @empeeNo int,
    @minAccounts int,
	@maxAccounts int,
	@allocationRank	smallint,
    @return INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    SET @return = 0
    UPDATE dbo.courtsperson
    SET MinAccounts = @minAccounts , MaxAccounts = @maxAccounts, AllocationRank = @allocationRank
    WHERE userid = @empeeNo

     SET @return = @@error
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO