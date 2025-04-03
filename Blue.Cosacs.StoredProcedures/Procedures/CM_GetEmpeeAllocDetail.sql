SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetEmpeeAllocDetail]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetEmpeeAllocDetail]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 16/03/2009
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetEmpeeAllocDetail] 
	@empeeNo int,
	@return int output
AS
BEGIN
    SET @return = 0    --initialise return code

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Select u.Id, RoleId, alloccount, maxAccounts, minAccounts, AllocationRank 
    from courtsperson c
	INNER JOIN Admin.[User] u ON u.id = c.UserId
	INNER JOIN admin.UserRole ur ON u.id = ur.Userid
	where u.id = @empeeNo

    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
