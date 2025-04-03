
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeDetailsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeDetailsGetSP]
GO


CREATE PROCEDURE  dbo.DN_EmployeeDetailsGetSP
    @empeeno            int,
    @fACTEmpeeNo        varchar(4)     OUTPUT,
    @branchno           smallint        OUTPUT,
    @empeename          varchar(20)    OUTPUT,
    @maxrow             integer         OUTPUT,
    @CommissionType     SMALLINT        OUTPUT,
    @return             int             OUTPUT

AS

    SET @return = 0    --initialise return code

    SELECT  @branchno       = u.branchno,
            @fACTEmpeeNo    = u.ExternalLogin,
            @empeename      = u.FullName,
            @maxrow         = isnull(c.maxrow,0),
            @CommissionType = case when admin.CheckPermission(@empeeno,381) = 1 then 3 
							  when admin.CheckPermission(@empeeno,380) = 1 then 1
							  else 0 end,
            @empeeno = u.id
    FROM Admin.[User] u
    INNER JOIN Admin.UserRole ur ON ur.UserId = u.id
    INNER JOIN dbo.courtsperson c ON c.UserId = u.Id
    WHERE   u.Id = @empeeno

    IF (@@rowcount < 1)
    BEGIN
        SET @return = -1
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

