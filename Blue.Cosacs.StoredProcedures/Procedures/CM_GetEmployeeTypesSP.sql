
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetEmployeeTypesSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetEmployeeTypesSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 24/03/2007
-- Description:	Returns all the employee types listed in the courtsperson table for a code category of ET1
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetEmployeeTypesSP]
	@return	int	OUTPUT
AS
BEGIN
    SET @return = 0    --initialise return code
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

     SELECT DISTINCT r.name,
					CONVERT(bit,0) as assigned,
--                    CASE WHEN WorkList IS NULL THEN CONVERT(bit,0) ELSE CONVERT(bit,1) END AS assigned,
					roleid
    FROM Admin.UserRole ur
    INNER JOIN Admin.Role r ON r.id = ur.RoleId
    LEFT OUTER JOIN dbo.CMWorkList w ON ur.roleid = w.EmpeeType

    SET @return = @@error
END
GO
