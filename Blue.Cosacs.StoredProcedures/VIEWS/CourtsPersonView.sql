
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].CourtsPerson'))
DROP VIEW [dbo].CourtsPerson
GO


CREATE VIEW [dbo].CourtsPerson
AS

SELECT Id AS EmpeeNo,
       ID as UserID,
        BranchNo ,
        commndue ,
        alloccount ,
        serialno ,
        datelstaudit ,
        maxrow ,
        lstcommn ,
        UpliftCommissionRate ,
        MinAccounts ,
        MaxAccounts ,
        AllocationRank ,
        RICashierCode
        LastChangePassword ,
        FirstName ,
        LastName ,
        ExternalLogin ,
        LegacyPassword ,
        eMail ,
        Locked ,
        FullName AS EmployeeName,
        FullName AS empeename,
        RICashierCode,
        FACTEmployeeNo			--#15273
FROM Admin.[User] u
INNER JOIN CourtsPersonTable c ON c.UserId = u.Id
GO
    