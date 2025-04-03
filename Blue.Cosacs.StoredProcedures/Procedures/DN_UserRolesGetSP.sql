
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UserRolesGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UserRolesGetSP]
GO



CREATE PROCEDURE  dbo.DN_UserRolesGetSP
            @user VARCHAR(10),
            @return int OUTPUT

AS

    SET     @return = 0            --initialise return code

    -- Return one list for both possible employee numbers but the
    -- CoSACS employee number is preferred to the FACT employee number.
    -- (A FACT employee no could be the same as someone elses CoSACS employee no.)
    
    SELECT RoleId 
    FROM Admin.UserRole
    WHERE UserId = @user

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

