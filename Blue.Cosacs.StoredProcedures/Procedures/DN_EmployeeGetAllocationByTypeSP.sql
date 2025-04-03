IF EXISTS (SELECT * FROM sysobjects
           WHERE xtype = 'p'
           AND name = 'DN_EmployeeGetAllocationByTypeSP')
BEGIN
	DROP PROCEDURE DN_EmployeeGetAllocationByTypeSP
END
GO

CREATE PROCEDURE  dbo.DN_EmployeeGetAllocationByTypeSP  
   @employeeType int,  
   @branchNo smallint ,  
   @return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
 SELECT ur.userid,  
   empeename + ' (' + convert(varchar,alloccount) + ')'  
 FROM  courtsperson  
 INNER JOIN Admin.UserRole ur ON dbo.CourtsPerson.UserID = ur.UserId
 WHERE ur.RoleId = @employeeType  
 AND  (branchno = @branchNo or @branchno = 0)  
  
 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END  