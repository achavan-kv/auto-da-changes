SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_Courtspersonget]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_Courtspersonget]
GO

CREATE procedure DN_CourtspersonGet
     @employeeNumber int, 
     @return int OUTPUT

AS
      SET 	@return = 0			--initialise return code  
      
      SELECT    u.branchno,
                u.ExternalLogin AS FACTEmployeeNo,
                FullName AS EmployeeName,
				u.firstname,
				u.lastname,
                ur.roleid,
                password,
                serialno,
                maxrow,
				UpliftCommissionRate			-- CR36 enhancements jec 21/06/07
      FROM courtsperson
      INNER JOIN Admin.[User] u ON dbo.courtsperson.UserId = u.Id
      INNER JOIN Admin.UserRole ur ON u.Id = ur.UserId
      WHERE     u.id = @employeeNumber
      AND       u.id != 0
       	
      SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

